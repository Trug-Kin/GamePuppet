// RendererFeature sương không gian; shader gán trong Renderer Data, không gắn GameObject.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): fogShader, material.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SpatialMistFeatureR22")]
public sealed class TinhNangSuongKhongGianR22 : ScriptableRendererFeature
{
    public Shader fogShader;
    // r30 renderer opt-in; retained checkpoints keep their serialized path.
    public bool isolateDepthForOverlayR30;
    Material material;
    MistPass pass;
    public override void Create()
    {
        if(fogShader)material=CoreUtils.CreateEngineMaterial(fogShader);
        pass=new MistPass{renderPassEvent=RenderPassEvent.BeforeRenderingPostProcessing};
        pass.ConfigureInput(ScriptableRenderPassInput.Depth|ScriptableRenderPassInput.Color);
        pass.requiresIntermediateTexture=true;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer,ref RenderingData data)
    {
        var mist=SuongKhongGianR22.Active;
        // Reflection uses its own oblique view. Keep its image clean and apply
        // atmospheric depth when it is seen on the lake, not twice in both views.
        if(!material||!mist||!mist.Applies||data.cameraData.camera.name.Contains("Reflection")||data.cameraData.cameraType==CameraType.Preview)return;
        mist.Configure(material);var camera=data.cameraData.camera;
        material.SetMatrix("_MistInverseVP",(GL.GetGPUProjectionMatrix(camera.projectionMatrix,true)*camera.worldToCameraMatrix).inverse);
        material.SetVector("_MistCamera",camera.transform.position);
        pass.material=material;pass.isolateDepth=isolateDepthForOverlayR30;renderer.EnqueuePass(pass);
    }
    protected override void Dispose(bool disposing){CoreUtils.Destroy(material);}
    sealed class MistPass : ScriptableRenderPass
    {
        public Material material;
        public bool isolateDepth;
        public override void RecordRenderGraph(RenderGraph graph,ContextContainer frame)
        {
            var resources=frame.Get<UniversalResourceData>();
            if(resources.isActiveTargetBackBuffer)return;
            var source=resources.activeColorTexture;
            var desc=source.GetDescriptor(graph);desc.name="WorldMistR22";desc.msaaSamples=MSAASamples.None;desc.clearBuffer=false;
            var destination=graph.CreateTexture(desc);
            var mistDepth=resources.activeDepthTexture;
            if(isolateDepth){
                // UI's UseAllGlobalTextures cannot sample its own depth attachment.
                // Keep the actual forward depth (including shaders without DepthOnly),
                // but expose a separate R32 snapshot instead of that attachment.
                var depthDesc=mistDepth.GetDescriptor(graph);depthDesc.name="R30_MistDepthSnapshot";
                depthDesc.depthBufferBits=DepthBits.None;depthDesc.colorFormat=UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
                depthDesc.msaaSamples=MSAASamples.None;depthDesc.clearBuffer=false;
                var snapshot=graph.CreateTexture(depthDesc);
                graph.AddBlitPass(mistDepth,snapshot,Vector2.one,Vector2.zero,filterMode:RenderGraphUtils.BlitFilterMode.ClampNearest,passName:"R30 copy forward depth for atmosphere");
                mistDepth=snapshot;
            }
            // Several preserved source shaders have no DepthOnly pass. The
            // forward depth attachment includes them; prepass depth does not.
            using(var binding=graph.AddRasterRenderPass<DepthBinding>("R22 bind camera depth",out var bindingData)){
                binding.UseTexture(mistDepth,AccessFlags.Read);
                binding.SetGlobalTextureAfterPass(mistDepth,Shader.PropertyToID("_MistDepthTexture"));
                binding.AllowPassCulling(false);binding.SetRenderFunc((DepthBinding d,RasterGraphContext context)=>{});
            }
            var parameters=new RenderGraphUtils.BlitMaterialParameters(source,destination,material,0);
            using(var builder=graph.AddBlitPass(parameters,"R22 world-space atmosphere",true)){
                builder.UseTexture(mistDepth,AccessFlags.Read);
                builder.UseGlobalTexture(Shader.PropertyToID("_MistDepthTexture"),AccessFlags.Read);
            }
            resources.cameraColor=destination;
        }
        sealed class DepthBinding {}
    }
}
