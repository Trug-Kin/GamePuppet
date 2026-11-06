// Áp dụng ánh sáng bề mặt theo vùng; dùng chung với camera/gameplay hiện có.
// Component trong scene r92: Regional_Surface_Lighting.
using UnityEngine;
using UnityEngine.Rendering;

// Custom graph shaders use the scene's actual trilight settings even before an
// Editor environment-probe bake. No emission or artificial panel-specific light.
[ExecuteAlways]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "RegionalSurfaceLighting")]
public sealed class AnhSangBeMatTheoKhu : MonoBehaviour
{
 void OnEnable()=>Apply();
 void LateUpdate()=>Apply();
 void Apply()
 {
  Shader.SetGlobalFloat("_RouteUseTrilight",RenderSettings.ambientMode==AmbientMode.Trilight?1:0);
  Shader.SetGlobalColor("_RouteAmbientSky",RenderSettings.ambientSkyColor.linear*RenderSettings.ambientIntensity);
  Shader.SetGlobalColor("_RouteAmbientEquator",RenderSettings.ambientEquatorColor.linear*RenderSettings.ambientIntensity);
  Shader.SetGlobalColor("_RouteAmbientGround",RenderSettings.ambientGroundColor.linear*RenderSettings.ambientIntensity);
 }
 void OnDisable()=>Shader.SetGlobalFloat("_RouteUseTrilight",0);
}
