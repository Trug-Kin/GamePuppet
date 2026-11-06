Shader "ConRoi/R65/ApproachLocal"
{
 Properties { _AsphaltMap("R65 Asphalt 4m sRGB",2D)="white"{}  _RouteMap("Packed project texture",2D)="white"{} _Cull("Cull",Float)=2 _BaseMap("Shadow compatibility",2D)="white"{} _BaseColor("Shadow compatibility",Color)=(1,1,1,1) _Cutoff("Cutoff",Float)=0 }
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
   TEXTURE2D(_AsphaltMap); SAMPLER(sampler_AsphaltMap);
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
    RouteSurface v0=MakeRouteSurface((float4(0.140000001,0.150000006,0.159999996,1)).rgb,(float4(0.800000012,0.800000012,0.800000012,0.800000012)).x,(float4(0,0,0,0)).x,(float4(1,1,1,1)).rgb*(float4(0,0,0,0)).x,(float4(0,0,0,0)).x);
float4 v1=float4(sourceWorld,0);
float4 v2=dot((v1).xyz,(float4(-0.273838609,0.961775601,0,0)).xyz).xxxx;
float4 v3=v2-float4(-43.1217041,-43.1217041,-43.1217041,-43.1217041);
float4 v4=v3/max(abs(float4(0.400000006,0.400000006,0.400000006,0.400000006)),1e-12);
float4 v5=floor(v4);
float4 v6=dot((v1).xyz,(float4(0.961775661,0.273838639,0,0)).xyz).xxxx;
float4 v7=v6-float4(-832.184326,-832.184326,-832.184326,-832.184326);
float4 v8=v7/max(abs(float4(0.400000006,0.400000006,0.400000006,0.400000006)),1e-12);
float4 v9=floor(v8);
float4 v10=float4((v5).x,(v9).x,(float4(0,0,0,0)).x,0);
float4 v11=RouteHash((v10).xyz).xxxx;
float4 v12=lerp(float4(0.36500001,0.354999989,0.32100001,1),float4(0.405000001,0.395999998,0.360000014,1),saturate(((v11).x-0)/1));
float4 v13=frac(v4);
float4 v14=float4(1,1,1,1)-v13;
float4 v15=min(v13,v14);
float4 v16=frac(v8);
float4 v17=float4(1,1,1,1)-v16;
float4 v18=min(v16,v17);
float4 v19=min(v15,v18);
float4 v20=float4((v19).x<(float4(0.00499999989,0.00499999989,0.00499999989,0.00499999989)).x,(v19).y<(float4(0.00499999989,0.00499999989,0.00499999989,0.00499999989)).y,(v19).z<(float4(0.00499999989,0.00499999989,0.00499999989,0.00499999989)).z,(v19).w<(float4(0.00499999989,0.00499999989,0.00499999989,0.00499999989)).w);
float4 v21=lerp(v12,float4(0.25,0.25,0.230000004,1),saturate(v20));
float4 v22=float4((v3).x>(float4(-7.5999999,-7.5999999,-7.5999999,-7.5999999)).x,(v3).y>(float4(-7.5999999,-7.5999999,-7.5999999,-7.5999999)).y,(v3).z>(float4(-7.5999999,-7.5999999,-7.5999999,-7.5999999)).z,(v3).w>(float4(-7.5999999,-7.5999999,-7.5999999,-7.5999999)).w);
float4 v23=float4((v3).x<(float4(21.3999996,21.3999996,21.3999996,21.3999996)).x,(v3).y<(float4(21.3999996,21.3999996,21.3999996,21.3999996)).y,(v3).z<(float4(21.3999996,21.3999996,21.3999996,21.3999996)).z,(v3).w<(float4(21.3999996,21.3999996,21.3999996,21.3999996)).w);
float4 v24=v22*v23;
float4 v25=float4((v7).x>(float4(2.5999999,2.5999999,2.5999999,2.5999999)).x,(v7).y>(float4(2.5999999,2.5999999,2.5999999,2.5999999)).y,(v7).z>(float4(2.5999999,2.5999999,2.5999999,2.5999999)).z,(v7).w>(float4(2.5999999,2.5999999,2.5999999,2.5999999)).w);
float4 v26=float4((v7).x<(float4(4.19999981,4.19999981,4.19999981,4.19999981)).x,(v7).y<(float4(4.19999981,4.19999981,4.19999981,4.19999981)).y,(v7).z<(float4(4.19999981,4.19999981,4.19999981,4.19999981)).z,(v7).w<(float4(4.19999981,4.19999981,4.19999981,4.19999981)).w);
float4 v27=v25*v26;
float4 v28=v24*v27;
float4 v29=v7-float4(3.4000001,3.4000001,3.4000001,3.4000001);
float4 v30=abs(v29);
float4 v31=float4((v30).x>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).x,(v30).y>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).y,(v30).z>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).z,(v30).w>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).w);
float4 v32=v28*v31;
float4 v33=float4((v3).x>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).x,(v3).y>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).y,(v3).z>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).z,(v3).w>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).w);
float4 v34=float4((v3).x<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).x,(v3).y<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).y,(v3).z<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).z,(v3).w<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).w);
float4 v35=v33*v34;
float4 v36=float4((v7).x>(float4(2.58999991,2.58999991,2.58999991,2.58999991)).x,(v7).y>(float4(2.58999991,2.58999991,2.58999991,2.58999991)).y,(v7).z>(float4(2.58999991,2.58999991,2.58999991,2.58999991)).z,(v7).w>(float4(2.58999991,2.58999991,2.58999991,2.58999991)).w);
float4 v37=float4((v7).x<(float4(2.73000002,2.73000002,2.73000002,2.73000002)).x,(v7).y<(float4(2.73000002,2.73000002,2.73000002,2.73000002)).y,(v7).z<(float4(2.73000002,2.73000002,2.73000002,2.73000002)).z,(v7).w<(float4(2.73000002,2.73000002,2.73000002,2.73000002)).w);
float4 v38=v36*v37;
float4 v39=v35*v38;
float4 v40=float4(1,1,1,1)-v39;
float4 v41=v32*v40;
float4 v42=v3-float4(21.3999996,21.3999996,21.3999996,21.3999996);
float4 v43=v42*float4(0.196034282,0.196034282,0.196034282,0.196034282);
float4 v44=v7-float4(3.4000001,3.4000001,3.4000001,3.4000001);
float4 v45=v44*float4(0.00963054691,0.00963054691,0.00963054691,0.00963054691);
float4 v46=v43+v45;
float4 v47=v46/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v48=max(float4(0,0,0,0),v47);
float4 v49=min(float4(1,1,1,1),v48);
float4 v50=v49*float4(0.196034282,0.196034282,0.196034282,0.196034282);
float4 v51=v42-v50;
float4 v52=v51*v51;
float4 v53=v49*float4(0.00963054691,0.00963054691,0.00963054691,0.00963054691);
float4 v54=v44-v53;
float4 v55=v54*v54;
float4 v56=v52+v55;
float4 v57=sqrt(max(0,v56));
float4 v58=v3-float4(21.596035,21.596035,21.596035,21.596035);
float4 v59=v58*float4(0.194146365,0.194146365,0.194146365,0.194146365);
float4 v60=v7-float4(3.40963054,3.40963054,3.40963054,3.40963054);
float4 v61=v60*float4(0.0287988931,0.0287988931,0.0287988931,0.0287988931);
float4 v62=v59+v61;
float4 v63=v62/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v64=max(float4(0,0,0,0),v63);
float4 v65=min(float4(1,1,1,1),v64);
float4 v66=v65*float4(0.194146365,0.194146365,0.194146365,0.194146365);
float4 v67=v58-v66;
float4 v68=v67*v67;
float4 v69=v65*float4(0.0287988931,0.0287988931,0.0287988931,0.0287988931);
float4 v70=v60-v69;
float4 v71=v70*v70;
float4 v72=v68+v71;
float4 v73=sqrt(max(0,v72));
float4 v74=min(v57,v73);
float4 v75=v3-float4(21.7901802,21.7901802,21.7901802,21.7901802);
float4 v76=v75*float4(0.190388709,0.190388709,0.190388709,0.190388709);
float4 v77=v7-float4(3.43842936,3.43842936,3.43842936,3.43842936);
float4 v78=v77*float4(0.0476898886,0.0476898886,0.0476898886,0.0476898886);
float4 v79=v76+v78;
float4 v80=v79/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v81=max(float4(0,0,0,0),v80);
float4 v82=min(float4(1,1,1,1),v81);
float4 v83=v82*float4(0.190388709,0.190388709,0.190388709,0.190388709);
float4 v84=v75-v83;
float4 v85=v84*v84;
float4 v86=v82*float4(0.0476898886,0.0476898886,0.0476898886,0.0476898886);
float4 v87=v77-v86;
float4 v88=v87*v87;
float4 v89=v85+v88;
float4 v90=sqrt(max(0,v89));
float4 v91=min(v74,v90);
float4 v92=v3-float4(21.9805698,21.9805698,21.9805698,21.9805698);
float4 v93=v92*float4(0.184797511,0.184797511,0.184797511,0.184797511);
float4 v94=v7-float4(3.48611927,3.48611927,3.48611927,3.48611927);
float4 v95=v94*float4(0.066121608,0.066121608,0.066121608,0.066121608);
float4 v96=v93+v95;
float4 v97=v96/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v98=max(float4(0,0,0,0),v97);
float4 v99=min(float4(1,1,1,1),v98);
float4 v100=v99*float4(0.184797511,0.184797511,0.184797511,0.184797511);
float4 v101=v92-v100;
float4 v102=v101*v101;
float4 v103=v99*float4(0.066121608,0.066121608,0.066121608,0.066121608);
float4 v104=v94-v103;
float4 v105=v104*v104;
float4 v106=v102+v105;
float4 v107=sqrt(max(0,v106));
float4 v108=min(v91,v107);
float4 v109=v3-float4(22.1653671,22.1653671,22.1653671,22.1653671);
float4 v110=v109*float4(0.177426606,0.177426606,0.177426606,0.177426606);
float4 v111=v7-float4(3.55224085,3.55224085,3.55224085,3.55224085);
float4 v112=v111*float4(0.0839165375,0.0839165375,0.0839165375,0.0839165375);
float4 v113=v110+v112;
float4 v114=v113/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v115=max(float4(0,0,0,0),v114);
float4 v116=min(float4(1,1,1,1),v115);
float4 v117=v116*float4(0.177426606,0.177426606,0.177426606,0.177426606);
float4 v118=v109-v117;
float4 v119=v118*v118;
float4 v120=v116*float4(0.0839165375,0.0839165375,0.0839165375,0.0839165375);
float4 v121=v111-v120;
float4 v122=v121*v121;
float4 v123=v119+v122;
float4 v124=sqrt(max(0,v123));
float4 v125=min(v108,v124);
float4 v126=v3-float4(22.3427944,22.3427944,22.3427944,22.3427944);
float4 v127=v126*float4(0.168346986,0.168346986,0.168346986,0.168346986);
float4 v128=v7-float4(3.63615751,3.63615751,3.63615751,3.63615751);
float4 v129=v128*float4(0.100903302,0.100903302,0.100903302,0.100903302);
float4 v130=v127+v129;
float4 v131=v130/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v132=max(float4(0,0,0,0),v131);
float4 v133=min(float4(1,1,1,1),v132);
float4 v134=v133*float4(0.168346986,0.168346986,0.168346986,0.168346986);
float4 v135=v126-v134;
float4 v136=v135*v135;
float4 v137=v133*float4(0.100903302,0.100903302,0.100903302,0.100903302);
float4 v138=v128-v137;
float4 v139=v138*v138;
float4 v140=v136+v139;
float4 v141=sqrt(max(0,v140));
float4 v142=min(v125,v141);
float4 v143=v3-float4(22.5111408,22.5111408,22.5111408,22.5111408);
float4 v144=v143*float4(0.157646105,0.157646105,0.157646105,0.157646105);
float4 v145=v7-float4(3.73706079,3.73706079,3.73706079,3.73706079);
float4 v146=v145*float4(0.116918318,0.116918318,0.116918318,0.116918318);
float4 v147=v144+v146;
float4 v148=v147/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v149=max(float4(0,0,0,0),v148);
float4 v150=min(float4(1,1,1,1),v149);
float4 v151=v150*float4(0.157646105,0.157646105,0.157646105,0.157646105);
float4 v152=v143-v151;
float4 v153=v152*v152;
float4 v154=v150*float4(0.116918318,0.116918318,0.116918318,0.116918318);
float4 v155=v145-v154;
float4 v156=v155*v155;
float4 v157=v153+v156;
float4 v158=sqrt(max(0,v157));
float4 v159=min(v142,v158);
float4 v160=v3-float4(22.668787,22.668787,22.668787,22.668787);
float4 v161=v160*float4(0.145426989,0.145426989,0.145426989,0.145426989);
float4 v162=v7-float4(3.85397911,3.85397911,3.85397911,3.85397911);
float4 v163=v162*float4(0.131807342,0.131807342,0.131807342,0.131807342);
float4 v164=v161+v163;
float4 v165=v164/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v166=max(float4(0,0,0,0),v165);
float4 v167=min(float4(1,1,1,1),v166);
float4 v168=v167*float4(0.145426989,0.145426989,0.145426989,0.145426989);
float4 v169=v160-v168;
float4 v170=v169*v169;
float4 v171=v167*float4(0.131807342,0.131807342,0.131807342,0.131807342);
float4 v172=v162-v171;
float4 v173=v172*v172;
float4 v174=v170+v173;
float4 v175=sqrt(max(0,v174));
float4 v176=min(v159,v175);
float4 v177=v3-float4(22.8142128,22.8142128,22.8142128,22.8142128);
float4 v178=v177*float4(0.131807342,0.131807342,0.131807342,0.131807342);
float4 v179=v7-float4(3.98578644,3.98578644,3.98578644,3.98578644);
float4 v180=v179*float4(0.145426989,0.145426989,0.145426989,0.145426989);
float4 v181=v178+v180;
float4 v182=v181/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v183=max(float4(0,0,0,0),v182);
float4 v184=min(float4(1,1,1,1),v183);
float4 v185=v184*float4(0.131807342,0.131807342,0.131807342,0.131807342);
float4 v186=v177-v185;
float4 v187=v186*v186;
float4 v188=v184*float4(0.145426989,0.145426989,0.145426989,0.145426989);
float4 v189=v179-v188;
float4 v190=v189*v189;
float4 v191=v187+v190;
float4 v192=sqrt(max(0,v191));
float4 v193=min(v176,v192);
float4 v194=v3-float4(22.9460201,22.9460201,22.9460201,22.9460201);
float4 v195=v194*float4(0.116918318,0.116918318,0.116918318,0.116918318);
float4 v196=v7-float4(4.13121367,4.13121367,4.13121367,4.13121367);
float4 v197=v196*float4(0.157646105,0.157646105,0.157646105,0.157646105);
float4 v198=v195+v197;
float4 v199=v198/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v200=max(float4(0,0,0,0),v199);
float4 v201=min(float4(1,1,1,1),v200);
float4 v202=v201*float4(0.116918318,0.116918318,0.116918318,0.116918318);
float4 v203=v194-v202;
float4 v204=v203*v203;
float4 v205=v201*float4(0.157646105,0.157646105,0.157646105,0.157646105);
float4 v206=v196-v205;
float4 v207=v206*v206;
float4 v208=v204+v207;
float4 v209=sqrt(max(0,v208));
float4 v210=min(v193,v209);
float4 v211=v3-float4(23.0629387,23.0629387,23.0629387,23.0629387);
float4 v212=v211*float4(0.100903302,0.100903302,0.100903302,0.100903302);
float4 v213=v7-float4(4.28885937,4.28885937,4.28885937,4.28885937);
float4 v214=v213*float4(0.168346986,0.168346986,0.168346986,0.168346986);
float4 v215=v212+v214;
float4 v216=v215/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v217=max(float4(0,0,0,0),v216);
float4 v218=min(float4(1,1,1,1),v217);
float4 v219=v218*float4(0.100903302,0.100903302,0.100903302,0.100903302);
float4 v220=v211-v219;
float4 v221=v220*v220;
float4 v222=v218*float4(0.168346986,0.168346986,0.168346986,0.168346986);
float4 v223=v213-v222;
float4 v224=v223*v223;
float4 v225=v221+v224;
float4 v226=sqrt(max(0,v225));
float4 v227=min(v210,v226);
float4 v228=v3-float4(23.1638432,23.1638432,23.1638432,23.1638432);
float4 v229=v228*float4(0.0839165375,0.0839165375,0.0839165375,0.0839165375);
float4 v230=v7-float4(4.45720673,4.45720673,4.45720673,4.45720673);
float4 v231=v230*float4(0.177426606,0.177426606,0.177426606,0.177426606);
float4 v232=v229+v231;
float4 v233=v232/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v234=max(float4(0,0,0,0),v233);
float4 v235=min(float4(1,1,1,1),v234);
float4 v236=v235*float4(0.0839165375,0.0839165375,0.0839165375,0.0839165375);
float4 v237=v228-v236;
float4 v238=v237*v237;
float4 v239=v235*float4(0.177426606,0.177426606,0.177426606,0.177426606);
float4 v240=v230-v239;
float4 v241=v240*v240;
float4 v242=v238+v241;
float4 v243=sqrt(max(0,v242));
float4 v244=min(v227,v243);
float4 v245=v3-float4(23.2477589,23.2477589,23.2477589,23.2477589);
float4 v246=v245*float4(0.066121608,0.066121608,0.066121608,0.066121608);
float4 v247=v7-float4(4.63463306,4.63463306,4.63463306,4.63463306);
float4 v248=v247*float4(0.184797511,0.184797511,0.184797511,0.184797511);
float4 v249=v246+v248;
float4 v250=v249/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v251=max(float4(0,0,0,0),v250);
float4 v252=min(float4(1,1,1,1),v251);
float4 v253=v252*float4(0.066121608,0.066121608,0.066121608,0.066121608);
float4 v254=v245-v253;
float4 v255=v254*v254;
float4 v256=v252*float4(0.184797511,0.184797511,0.184797511,0.184797511);
float4 v257=v247-v256;
float4 v258=v257*v257;
float4 v259=v255+v258;
float4 v260=sqrt(max(0,v259));
float4 v261=min(v244,v260);
float4 v262=v3-float4(23.3138809,23.3138809,23.3138809,23.3138809);
float4 v263=v262*float4(0.0476898886,0.0476898886,0.0476898886,0.0476898886);
float4 v264=v7-float4(4.81943083,4.81943083,4.81943083,4.81943083);
float4 v265=v264*float4(0.190388709,0.190388709,0.190388709,0.190388709);
float4 v266=v263+v265;
float4 v267=v266/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v268=max(float4(0,0,0,0),v267);
float4 v269=min(float4(1,1,1,1),v268);
float4 v270=v269*float4(0.0476898886,0.0476898886,0.0476898886,0.0476898886);
float4 v271=v262-v270;
float4 v272=v271*v271;
float4 v273=v269*float4(0.190388709,0.190388709,0.190388709,0.190388709);
float4 v274=v264-v273;
float4 v275=v274*v274;
float4 v276=v272+v275;
float4 v277=sqrt(max(0,v276));
float4 v278=min(v261,v277);
float4 v279=v3-float4(23.3615704,23.3615704,23.3615704,23.3615704);
float4 v280=v279*float4(0.0287988931,0.0287988931,0.0287988931,0.0287988931);
float4 v281=v7-float4(5.00981951,5.00981951,5.00981951,5.00981951);
float4 v282=v281*float4(0.194146365,0.194146365,0.194146365,0.194146365);
float4 v283=v280+v282;
float4 v284=v283/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v285=max(float4(0,0,0,0),v284);
float4 v286=min(float4(1,1,1,1),v285);
float4 v287=v286*float4(0.0287988931,0.0287988931,0.0287988931,0.0287988931);
float4 v288=v279-v287;
float4 v289=v288*v288;
float4 v290=v286*float4(0.194146365,0.194146365,0.194146365,0.194146365);
float4 v291=v281-v290;
float4 v292=v291*v291;
float4 v293=v289+v292;
float4 v294=sqrt(max(0,v293));
float4 v295=min(v278,v294);
float4 v296=v3-float4(23.3903694,23.3903694,23.3903694,23.3903694);
float4 v297=v296*float4(0.00963054691,0.00963054691,0.00963054691,0.00963054691);
float4 v298=v7-float4(5.20396566,5.20396566,5.20396566,5.20396566);
float4 v299=v298*float4(0.196034282,0.196034282,0.196034282,0.196034282);
float4 v300=v297+v299;
float4 v301=v300/max(abs(float4(0.0385221876,0.0385221876,0.0385221876,0.0385221876)),1e-12);
float4 v302=max(float4(0,0,0,0),v301);
float4 v303=min(float4(1,1,1,1),v302);
float4 v304=v303*float4(0.00963054691,0.00963054691,0.00963054691,0.00963054691);
float4 v305=v296-v304;
float4 v306=v305*v305;
float4 v307=v303*float4(0.196034282,0.196034282,0.196034282,0.196034282);
float4 v308=v298-v307;
float4 v309=v308*v308;
float4 v310=v306+v309;
float4 v311=sqrt(max(0,v310));
float4 v312=min(v295,v311);
float4 v313=v3-float4(23.3999996,23.3999996,23.3999996,23.3999996);
float4 v314=v313*float4(0,0,0,0);
float4 v315=v7-float4(5.4000001,5.4000001,5.4000001,5.4000001);
float4 v316=v315*float4(17.117506,17.117506,17.117506,17.117506);
float4 v317=v314+v316;
float4 v318=v317/max(abs(float4(293.009003,293.009003,293.009003,293.009003)),1e-12);
float4 v319=max(float4(0,0,0,0),v318);
float4 v320=min(float4(1,1,1,1),v319);
float4 v321=v320*float4(0,0,0,0);
float4 v322=v313-v321;
float4 v323=v322*v322;
float4 v324=v320*float4(17.117506,17.117506,17.117506,17.117506);
float4 v325=v315-v324;
float4 v326=v325*v325;
float4 v327=v323+v326;
float4 v328=sqrt(max(0,v327));
float4 v329=min(v312,v328);
float4 v330=float4((v329).x<(float4(0.800000012,0.800000012,0.800000012,0.800000012)).x,(v329).y<(float4(0.800000012,0.800000012,0.800000012,0.800000012)).y,(v329).z<(float4(0.800000012,0.800000012,0.800000012,0.800000012)).z,(v329).w<(float4(0.800000012,0.800000012,0.800000012,0.800000012)).w);
float4 v331=v3*float4(0.0311672054,0.0311672054,0.0311672054,0.0311672054);
float4 v332=v7-v331;
float4 v333=v332-float4(21.3681927,21.3681927,21.3681927,21.3681927);
float4 v334=float4((v333).x<(float4(0.219999999,0.219999999,0.219999999,0.219999999)).x,(v333).y<(float4(0.219999999,0.219999999,0.219999999,0.219999999)).y,(v333).z<(float4(0.219999999,0.219999999,0.219999999,0.219999999)).z,(v333).w<(float4(0.219999999,0.219999999,0.219999999,0.219999999)).w);
float4 v335=v330*v334;
float4 v336=float4((v329).x>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).x,(v329).y>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).y,(v329).z>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).z,(v329).w>(float4(0.699999988,0.699999988,0.699999988,0.699999988)).w);
float4 v337=v335*v336;
float4 v338=max(v41,v337);
float4 v339=lerp(v21,float4(0.254999995,0.26699999,0.25,1),saturate(v338));
float4 v340=float4((v333).x>(float4(-0.649999976,-0.649999976,-0.649999976,-0.649999976)).x,(v333).y>(float4(-0.649999976,-0.649999976,-0.649999976,-0.649999976)).y,(v333).z>(float4(-0.649999976,-0.649999976,-0.649999976,-0.649999976)).z,(v333).w>(float4(-0.649999976,-0.649999976,-0.649999976,-0.649999976)).w);
float4 v341=v335*v340;
float4 v342=lerp(v339,float4(0.319999993,0.323000014,0.296000004,1),saturate(v341));
float4 v343=float4((v333).x>(float4(0.100000001,0.100000001,0.100000001,0.100000001)).x,(v333).y>(float4(0.100000001,0.100000001,0.100000001,0.100000001)).y,(v333).z>(float4(0.100000001,0.100000001,0.100000001,0.100000001)).z,(v333).w>(float4(0.100000001,0.100000001,0.100000001,0.100000001)).w);
float4 v344=v335*v343;
float4 v345=lerp(v342,float4(0.234999999,0.25,0.238000005,1),saturate(v344));
RouteSurface v346=MakeRouteSurface((v345).rgb,(float4(0.910000026,0.910000026,0.910000026,0.910000026)).x,(float4(0,0,0,0)).x,(float4(1,1,1,1)).rgb*(float4(0,0,0,0)).x,(float4(0,0,0,0)).x);
float4 v347=max(v28,v335);
float4 v348=float4((v3).x>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).x,(v3).y>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).y,(v3).z>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).z,(v3).w>(float4(-2.9000001,-2.9000001,-2.9000001,-2.9000001)).w);
float4 v349=float4((v3).x<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).x,(v3).y<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).y,(v3).z<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).z,(v3).w<(float4(-0.0500000007,-0.0500000007,-0.0500000007,-0.0500000007)).w);
float4 v350=v348*v349;
float4 v351=float4((v7).x>(float4(0.180000007,0.180000007,0.180000007,0.180000007)).x,(v7).y>(float4(0.180000007,0.180000007,0.180000007,0.180000007)).y,(v7).z>(float4(0.180000007,0.180000007,0.180000007,0.180000007)).z,(v7).w>(float4(0.180000007,0.180000007,0.180000007,0.180000007)).w);
float4 v352=float4((v7).x<(float4(3.4000001,3.4000001,3.4000001,3.4000001)).x,(v7).y<(float4(3.4000001,3.4000001,3.4000001,3.4000001)).y,(v7).z<(float4(3.4000001,3.4000001,3.4000001,3.4000001)).z,(v7).w<(float4(3.4000001,3.4000001,3.4000001,3.4000001)).w);
float4 v353=v351*v352;
float4 v354=v350*v353;
float4 v355=max(v347,v354);
float4 v356=float4(sourceNormal,0);
float4 v357=(v356).zzzz;
float4 v358=float4((v357).x>(float4(0.600000024,0.600000024,0.600000024,0.600000024)).x,(v357).y>(float4(0.600000024,0.600000024,0.600000024,0.600000024)).y,(v357).z>(float4(0.600000024,0.600000024,0.600000024,0.600000024)).z,(v357).w>(float4(0.600000024,0.600000024,0.600000024,0.600000024)).w);
float4 v359=v355*v358;
RouteSurface v360=MixRouteSurface(v0,v346,(v359).x);
return v360;
   }
   half4 frag(Varyings i):SV_Target
   {
    float3 n=normalize(i.normalWS);float3 sourceWorld=float3(-i.positionWS.x,-i.positionWS.z,i.positionWS.y);float3 sourceNormal=float3(-n.x,-n.z,n.y);
    RouteSurface r=EvaluateRoute(i,sourceWorld,sourceNormal);
    // Source material: road-materials-r65-source.blend. Generic aggregate study.
    // No displacement, new curb, painted line, or terrain mask.
    float2 asphaltUV=i.positionWS.xz/4.0;
    float3 asphalt=SAMPLE_TEXTURE2D(_AsphaltMap,sampler_AsphaltMap,asphaltUV).rgb;
    float asphaltWeight=1.0-smoothstep(272.0,280.0,i.positionWS.z);
    r.color=lerp(r.color,asphalt,asphaltWeight);r.roughness=lerp(r.roughness,.89,asphaltWeight);
    r.height=lerp(r.height,0,asphaltWeight);r.metallic=lerp(r.metallic,0,asphaltWeight);
    r.emission=lerp(r.emission,0,asphaltWeight);

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
