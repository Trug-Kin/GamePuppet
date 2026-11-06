Shader "ConRoi/R55/LocalTerrainDetail"
{
 Properties { _BaseColor("Color",Color)=(.32,.32,.30,1) _Tiling("Paving metres (zero for terrain)",Float)=.4 _Urban("Urban ground",Float)=0 _BaseMap("Base",2D)="white"{} _Cutoff("Cutoff",Float)=0 }
 SubShader {
 Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"}
 Pass {
 Tags {"LightMode"="UniversalForward"}
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 float4 _BaseColor;float _Tiling;float _Urban;
 struct A {float4 vertex:POSITION;float3 normal:NORMAL;};
 struct V {float4 clip:SV_POSITION;float3 world:TEXCOORD0;float3 normal:TEXCOORD1;};
 V vert(A i){V o;o.world=TransformObjectToWorld(i.vertex.xyz);o.clip=TransformWorldToHClip(o.world);o.normal=TransformObjectToWorldNormal(i.normal);return o;}
 half4 frag(V i):SV_Target {
 float2 uv=i.world.xz/max(_Tiling,.4);float2 cell=floor(uv);float2 f=frac(uv);
 float grain=frac(sin(dot(floor(i.world.xz*45),float2(12.9898,78.233)))*43758.5453);
 float coarse=sin(i.world.x*.43)*sin(i.world.z*.37)*.06;
 float3 c=_BaseColor.rgb*(.94+grain*.12+coarse);
 if(_Urban>.5){float plaza=(1-smoothstep(23,25,abs(i.world.x-791)))*smoothstep(153,159,i.world.z)*(1-smoothstep(270,280,i.world.z));c=lerp(float3(.15,.18,.13),float3(.32,.31,.27),plaza)*(.94+grain*.12+coarse);}
 // Remove the R22 grid only in the bounded TTC/Langfarm review region.
 float inRegion=smoothstep(715,718,i.world.x)*(1-smoothstep(752,755,i.world.x))*smoothstep(282,285,i.world.z)*(1-smoothstep(342,345,i.world.z));
 if(_Tiling>0){float2 aa=max(fwidth(uv),.001);float2 edge=smoothstep(0,aa+.009,min(f,1-f));float tiled=lerp(.74,1,min(edge.x,edge.y))*(.95+.08*frac(sin(dot(cell,float2(23.2,41.3)))*1471));c*=lerp(tiled,1,inRegion);}
 Light l=GetMainLight(TransformWorldToShadowCoord(i.world));float3 n=normalize(i.normal);
 float3 color=c*(SampleSH(n)+l.color*(.2+.8*saturate(dot(n,l.direction)))*lerp(.65,1,l.shadowAttenuation));
 return half4(color,1);
 }
 ENDHLSL
 }
 UsePass "Universal Render Pipeline/Lit/ShadowCaster"
 UsePass "Universal Render Pipeline/Lit/DepthOnly"
 }
}
