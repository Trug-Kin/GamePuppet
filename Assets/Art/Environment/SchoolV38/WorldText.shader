Shader "ConRoi/WorldText"
{
 Properties { _MainTex("Font atlas",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) }
 SubShader
 {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
  Pass
  {
   Blend SrcAlpha OneMinusSrcAlpha
   ZWrite Off
   ZTest LEqual
   Cull Off
   HLSLPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _MainTex;float4 _Color;
   struct appdata {float4 vertex:POSITION;float2 uv:TEXCOORD0;float4 color:COLOR;};
   struct v2f {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;float4 color:COLOR;};
   v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
   half4 frag(v2f i):SV_Target {half4 c=i.color;c.a*=tex2D(_MainTex,i.uv).a;return c;}
   ENDHLSL
  }
 }
}
