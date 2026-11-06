Shader "ConRoi/RouteV33/Surface_080"
{
 Properties { _RouteMap("Packed project texture",2D)="white"{} _Cull("Cull",Float)=2 _BaseMap("Shadow compatibility",2D)="white"{} _BaseColor("Shadow compatibility",Color)=(1,1,1,1) _Cutoff("Cutoff",Float)=0 }
 SubShader
 {
  Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
  Pass
  {
   Name "ForwardLit" Tags { "LightMode"="UniversalForward" }
   Cull [_Cull]
   HLSLPROGRAM
   #pragma target 3.5
   #pragma vertex vert
   #pragma fragment frag
   #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
   #pragma multi_compile_fragment _ _SHADOWS_SOFT
   #pragma multi_compile _ _ADDITIONAL_LIGHTS
   #pragma multi_compile_fog
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
   TEXTURE2D(_RouteMap); SAMPLER(sampler_RouteMap);
   float _RouteUseTrilight; float4 _RouteAmbientSky,_RouteAmbientEquator,_RouteAmbientGround;
   CBUFFER_START(UnityPerMaterial)
   float4 _RouteMap_ST; float4 _BaseMap_ST; half4 _BaseColor; float _Cutoff; float _Cull;
   CBUFFER_END
   struct Attributes {float4 positionOS:POSITION;float3 normalOS:NORMAL;float2 uv:TEXCOORD0;float2 coord1:TEXCOORD1;float2 coord2:TEXCOORD2;float2 coord3:TEXCOORD3;};
   struct Varyings {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;float3 normalWS:TEXCOORD1;float2 uv:TEXCOORD2;float3 local:TEXCOORD3;float3 generated:TEXCOORD4;float fog:TEXCOORD5;};
   Varyings vert(Attributes a){Varyings o;VertexPositionInputs p=GetVertexPositionInputs(a.positionOS.xyz);o.positionCS=p.positionCS;o.positionWS=p.positionWS;o.normalWS=TransformObjectToWorldNormal(a.normalOS);o.uv=a.uv;o.local=float3(a.coord1,a.coord2.x);o.generated=float3(a.coord2.y,a.coord3);o.fog=ComputeFogFactor(p.positionCS.z);return o;}
   float RouteHash(float3 p){p=frac(p*.1031);p+=dot(p,p.yzx+33.33);return frac((p.x+p.y)*p.z);}
   float3 RouteGradient(float3 p){return normalize(float3(RouteHash(p+float3(17,3,5)),RouteHash(p+float3(7,19,11)),RouteHash(p+float3(13,23,29)))*2-1+1e-5);}
   float RoutePerlin(float3 p)
   {
    float3 b=floor(p),f=frac(p),u=f*f*f*(f*(f*6-15)+10);
    float a=dot(RouteGradient(b),f),c=dot(RouteGradient(b+float3(1,0,0)),f-float3(1,0,0));
    float d=dot(RouteGradient(b+float3(0,1,0)),f-float3(0,1,0)),e=dot(RouteGradient(b+float3(1,1,0)),f-float3(1,1,0));
    float g=dot(RouteGradient(b+float3(0,0,1)),f-float3(0,0,1)),h=dot(RouteGradient(b+float3(1,0,1)),f-float3(1,0,1));
    float j=dot(RouteGradient(b+float3(0,1,1)),f-float3(0,1,1)),k=dot(RouteGradient(b+float3(1,1,1)),f-1);
    return .5+lerp(lerp(lerp(a,c,u.x),lerp(d,e,u.x),u.y),lerp(lerp(g,h,u.x),lerp(j,k,u.x),u.y),u.z)*.95;
   }
   float RouteNoise(float3 p,float detail,float roughness){float total=RoutePerlin(p),weight=1,normalization=1;[unroll]for(int k=1;k<=5;k++){float use=saturate(detail-(k-1));weight*=roughness;p*=2;total+=RoutePerlin(p)*weight*use;normalization+=weight*use;}return saturate(total/normalization);}
   struct RouteSurface {float3 color;float roughness;float metallic;float3 emission;float height;};
   RouteSurface MakeRouteSurface(float3 c,float r,float m,float3 e,float h){RouteSurface s;s.color=c;s.roughness=r;s.metallic=m;s.emission=e;s.height=h;return s;}
   RouteSurface MixRouteSurface(RouteSurface a,RouteSurface b,float f){f=saturate(f);return MakeRouteSurface(lerp(a.color,b.color,f),lerp(a.roughness,b.roughness,f),lerp(a.metallic,b.metallic,f),lerp(a.emission,b.emission,f),lerp(a.height,b.height,f));}
   RouteSurface EvaluateRoute(Varyings i,float3 sourceWorld,float3 sourceNormal)
   {
    float4 v0=float4(i.local,0);
float v1=RouteNoise((v0).xyz*(float4(190,190,190,190)).x,(float4(1,1,1,1)).x,(float4(0.5,0.5,0.5,0.5)).x);
float4 v2=(v1).xxxx;
float4 v3=v2*(float4(0.000650000002,0.000650000002,0.000650000002,0.000650000002)).x*(float4(0.200000003,0.200000003,0.200000003,0.200000003)).x;
float4 v4=float4(i.local,0);
float v5=RouteNoise((v4).xyz*(float4(2.29999995,2.29999995,2.29999995,2.29999995)).x,(float4(1,1,1,1)).x,(float4(0.5,0.5,0.5,0.5)).x);
float4 v6=(v5).xxxx;
float4 v7=lerp(float4(0.297600001,0.134399995,0.153600007,1),float4(0.322400004,0.145600006,0.1664,1),saturate(((v6).x-0)/1));
RouteSurface v8=MakeRouteSurface((v7).rgb,(float4(0.980000019,0.980000019,0.980000019,0.980000019)).x,(float4(0,0,0,0)).x,(float4(1,1,1,1)).rgb*(float4(0,0,0,0)).x,(v3).x);
return v8;
   }
   half4 frag(Varyings i):SV_Target
   {
    float3 n=normalize(i.normalWS);float3 sourceWorld=float3(-i.positionWS.x,-i.positionWS.z,i.positionWS.y);float3 sourceNormal=float3(-n.x,-n.z,n.y);
    RouteSurface r=EvaluateRoute(i,sourceWorld,sourceNormal);
    float3 dx=ddx(i.positionWS),dy=ddy(i.positionWS),r1=cross(dy,n),r2=cross(n,dx);float det=dot(dx,r1);n=normalize(max(abs(det),1e-10)*n-sign(det)*(ddx(r.height)*r1+ddy(r.height)*r2));
    SurfaceData s=(SurfaceData)0;s.albedo=max(0,r.color);s.metallic=saturate(r.metallic);s.specular=.5;s.smoothness=1-saturate(r.roughness);s.normalTS=half3(0,0,1);s.occlusion=1;s.emission=r.emission;s.alpha=1;
    InputData d=(InputData)0;d.positionWS=i.positionWS;d.normalWS=n;d.viewDirectionWS=GetWorldSpaceNormalizeViewDir(i.positionWS);d.shadowCoord=TransformWorldToShadowCoord(i.positionWS);d.bakedGI=lerp(SampleSH(n),lerp(_RouteAmbientEquator.rgb,n.y>=0?_RouteAmbientSky.rgb:_RouteAmbientGround.rgb,abs(n.y)),_RouteUseTrilight);d.normalizedScreenSpaceUV=GetNormalizedScreenSpaceUV(i.positionCS);d.shadowMask=half4(1,1,1,1);
    half4 color=UniversalFragmentPBR(d,s);color.rgb=MixFog(color.rgb,i.fog);return color;
   }
   ENDHLSL
  }
  UsePass "Universal Render Pipeline/Lit/ShadowCaster"
  UsePass "Universal Render Pipeline/Lit/DepthOnly"
 }
}
