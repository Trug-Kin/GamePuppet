Shader "ConRoi/R78/Terrain176Shadow"
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
 // R55 mask retained. R56 removes only the procedural paving multiplier on park terrain.
 // Bounds are a technical review mask, not a surveyed park/path boundary.
 float inRegion=smoothstep(715,718,i.world.x)*(1-smoothstep(752,755,i.world.x))*smoothstep(282,285,i.world.z)*(1-smoothstep(342,345,i.world.z));
 float parkRegion=smoothstep(750,755,i.world.x)*(1-smoothstep(855,860,i.world.x))*smoothstep(310,315,i.world.z)*(1-smoothstep(440,445,i.world.z));
 inRegion=max(inRegion,parkRegion);
 if(_Tiling>0){float2 aa=max(fwidth(uv),.001);float2 edge=smoothstep(0,aa+.009,min(f,1-f));float tiled=lerp(.74,1,min(edge.x,edge.y))*(.95+.08*frac(sin(dot(cell,float2(23.2,41.3)))*1471));c*=lerp(tiled,1,inRegion);}
 Light l=GetMainLight(TransformWorldToShadowCoord(i.world));float3 n=normalize(i.normal);
 float3 color=c*(SampleSH(n)+l.color*(.2+.8*saturate(dot(n,l.direction)))*lerp(.65,1,l.shadowAttenuation));
 return half4(color,1);
 }
 ENDHLSL
 }
 Pass {
 Name "ShadowCaster" Tags { "LightMode"="ShadowCaster" }
 ZWrite On ZTest LEqual ColorMask 0 Cull Back

 HLSLPROGRAM
 #pragma target 3.5
 #pragma vertex ShadowVertex
 #pragma fragment ShadowFragment
 #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
 float3 _LightDirection; float3 _LightPosition;
 struct ShadowAttributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; };
 float4 ShadowVertex(ShadowAttributes v):SV_POSITION {
  float3 p=TransformObjectToWorld(v.positionOS.xyz);
  float3 n=TransformObjectToWorldNormal(v.normalOS);
  #if defined(_CASTING_PUNCTUAL_LIGHT_SHADOW)
   float3 lightDirectionWS=normalize(_LightPosition-p);
  #else
   float3 lightDirectionWS=_LightDirection;
  #endif
  float4 clip=TransformWorldToHClip(ApplyShadowBias(p,n,lightDirectionWS));
  #if UNITY_REVERSED_Z
   clip.z=min(clip.z,UNITY_NEAR_CLIP_VALUE*clip.w);
  #else
   clip.z=max(clip.z,UNITY_NEAR_CLIP_VALUE*clip.w);
  #endif
  return clip;
 }
 half4 ShadowFragment():SV_TARGET { return 0; }
 ENDHLSL
 }

 UsePass "Universal Render Pipeline/Lit/DepthOnly"
 }
}
