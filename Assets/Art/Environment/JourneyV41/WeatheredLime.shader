Shader "ConRoi/V41/WeatheredLime"
{
 Properties { _BaseColor("Limewash",Color)=(.6,.5,.4,1) }
 SubShader
 {
  Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"}
  Pass
  {
   Tags {"LightMode"="UniversalForward"}
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
   #pragma multi_compile _ _ADDITIONAL_LIGHTS
   #pragma multi_compile_fog
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
   CBUFFER_START(UnityPerMaterial)
   half4 _BaseColor;
   CBUFFER_END
   struct A{float4 p:POSITION;float3 n:NORMAL;};
   struct V{float4 p:SV_POSITION;float3 world:TEXCOORD0;float3 n:TEXCOORD1;float fog:TEXCOORD2;};
   V vert(A a){V o;VertexPositionInputs p=GetVertexPositionInputs(a.p.xyz);o.p=p.positionCS;o.world=p.positionWS;o.n=TransformObjectToWorldNormal(a.n);o.fog=ComputeFogFactor(p.positionCS.z);return o;}
   float hash(float3 p){return frac(sin(dot(p,float3(127.1,311.7,74.7)))*43758.5453);}
   float noise(float3 p){float3 i=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(lerp(hash(i),hash(i+float3(1,0,0)),f.x),lerp(hash(i+float3(0,1,0)),hash(i+float3(1,1,0)),f.x),f.y),lerp(lerp(hash(i+float3(0,0,1)),hash(i+float3(1,0,1)),f.x),lerp(hash(i+float3(0,1,1)),hash(i+1),f.x),f.y),f.z);}
   half4 frag(V i):SV_Target
   {
    float3 n=normalize(i.n);float wear=noise(i.world*1.8)*.18+noise(i.world*19)*.07;
    float damp=smoothstep(.62,.85,noise(i.world*.35))* .22;
    float3 albedo=_BaseColor.rgb*(.82+wear-damp);
    Light main=GetMainLight(TransformWorldToShadowCoord(i.world));float3 illumination=SampleSH(n)+main.color*saturate(dot(n,main.direction))*main.shadowAttenuation;
    #ifdef _ADDITIONAL_LIGHTS
    for(uint k=0;k<GetAdditionalLightsCount();k++){Light l=GetAdditionalLight(k,i.world);illumination+=l.color*saturate(dot(n,l.direction))*l.distanceAttenuation*l.shadowAttenuation;}
    #endif
    return half4(MixFog(albedo*illumination,i.fog),1);
   }
   ENDHLSL
  }
  UsePass "Universal Render Pipeline/Lit/ShadowCaster"
  UsePass "Universal Render Pipeline/Lit/DepthOnly"
 }
}
