Shader "ConRoi/R22/WorldMist"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            ZWrite Off ZTest Always Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Atmosphere
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            float4 _MistCorridors[24];
            int _MistCorridorCount;
            int _MistDebug;
            float4 _MistProfile;
            float4 _MistColor;
            TEXTURE2D_X_FLOAT(_MistDepthTexture);
            float4x4 _MistInverseVP;
            float3 _MistCamera;
            float RouteDistance(float2 p)
            {
                float nearest=10000;
                [loop] for(int i=0;i<_MistCorridorCount;i++)
                {
                    float2 a=_MistCorridors[i].xy,b=_MistCorridors[i].zw;
                    float2 ab=b-a;
                    nearest=min(nearest,length(p-a-ab*saturate(dot(p-a,ab)/max(dot(ab,ab),.001))));
                }
                return nearest;
            }
            half4 Atmosphere(Varyings input):SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv=input.texcoord;
                half4 source=SAMPLE_TEXTURE2D_X(_BlitTexture,sampler_LinearClamp,uv);
                float depth=SAMPLE_TEXTURE2D_X(_MistDepthTexture,sampler_PointClamp,uv).r;
                #if UNITY_REVERSED_Z
                    if(depth<.00001)return source;
                #else
                    if(depth>.99999)return source;
                    depth=lerp(UNITY_NEAR_CLIP_VALUE,1,depth);
                #endif
                float3 end=ComputeWorldSpacePosition(uv,depth,_MistInverseVP);
                float3 origin=_MistCamera;
                float distance=length(end-origin);
                if(_MistDebug>0)return half4(saturate(distance/160).xxx,1);
                if(distance<7)return source;
                float3 direction=(end-origin)/max(distance,.001);
                float span=min(distance,650)-7;
                float opticalDepth=0;
                // Integral samples world density, with a clear immediate layer.
                // Smooth corridor falloff has no moving camera-centered edge.
                [unroll] for(int j=0;j<12;j++)
                {
                    float along=7+span*(j+.5)/12;
                    float3 p=origin+direction*along;
                    float outer=smoothstep(_MistProfile.x,_MistProfile.x+_MistProfile.y,RouteDistance(p.xz));
                    float density=lerp(_MistProfile.z,_MistProfile.w,outer);
                    float height=lerp(1,.55,smoothstep(8,45,p.y));
                    opticalDepth+=density*height*span/12;
                }
                float amount=1-exp(-opticalDepth);
                return half4(lerp(source.rgb,_MistColor.rgb,amount),source.a);
            }
            ENDHLSL
        }
    }
}
