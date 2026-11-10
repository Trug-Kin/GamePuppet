using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.EditorChecks
{
    // Contract fixtures only. No authored feature/rubric, review or media becomes canon.
    public static class ProductBindingChecks
    {
        static readonly JArray checks=new JArray();static string output;static int failures;
        static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:"";}
        static T Clone<T>(T x)=>JsonUtility.FromJson<T>(JsonUtility.ToJson(x));
        static void C(string name,bool pass){checks.Add(new JObject{["name"]=name,["pass"]=pass});if(!pass)throw new Exception(name);}
        static void Must(FullActionResult result){if(!result.Success)throw new Exception(result.Code+": "+result.Message);}
        static FullCaseCatalog Catalog()=>new FullCaseCatalog{
            id="fixture.product.contract",revision="1",testOnly=false,startLocation="office",
            locations=new[]{new FullLocationDefinition{id="office",label="FIXTURE office"},new FullLocationDefinition{id="market",label="FIXTURE market"},new FullLocationDefinition{id="station",label="FIXTURE station",requires=new[]{new FullRequirement(FullRequirementKind.Finalized,"")}}},
            evidence=new[]{"sample.a","sample.b","letter"}.Select(id=>new FullEvidenceDefinition{id=id,location="office",label="FIXTURE "+id,source="FIXTURE source",text="FIXTURE visible description",observations=new[]{"FIXTURE visible detail"},portable=false}).ToArray(),
            comparisons=new[]{new FullComparisonDefinition{id="pair",label="FIXTURE pair",evidenceIds=new[]{"sample.a","sample.b"},options=new[]{"different","similar"},expectedOption="different",requires=new[]{new FullRequirement(FullRequirementKind.Recorded,"sample.a",0)}}},
            conclusions=new[]{"sender","presence","fall","motive"}.Select(id=>new FullConclusionRule{id=id,label="FIXTURE "+id,claims=new[]{"claim.known","claim.other"},correctClaim="claim.known",reviewed=false,alternatives=new[]{new FullSupportSet{requires=new[]{new FullRequirement(FullRequirementKind.Recorded,"sample.a",0)}}}}).ToArray()
        };
        static FullProductBindings Bindings()=>new FullProductBindings{
            catalogId="fixture.product.contract",catalogRevision="1",
            features=new[]{"different","similar"}.SelectMany(o=>new[]{"sample.a","sample.b"}.Select(e=>new FullFeatureBinding{id="feature."+o+"."+e,comparisonId="pair",optionId=o,evidenceId=e,observationIndex=0,label="FIXTURE feature",description="FIXTURE explicit feature, not reviewed content."})).ToArray(),
            evidenceAssets=new[]{new FullEvidenceAssetBinding{evidenceId="sample.a"}}
        };
        static FullInvestigationRuntime Create(FullCaseCatalog c,FullProductBindings b){Must(FullInvestigationRuntime.TryCreateProduct(c,b,out var s));return s;}
        static void View(FullInvestigationRuntime s,string id){s.Cancel();Must(s.TryCollect(id));Must(s.TryInspect(id));Must(s.TryRecordObservation(id,0));s.Cancel();}
        static void Reviewed(FullCaseCatalog c,FullProductBindings b){foreach(var rule in c.conclusions)rule.reviewed=true;c.fallChainReviewed=true;b.reviewedCatalogRevision=c.revision;b.reviewReference="FIXTURE C3 report (not real content approval)";b.fallChainReviewReference="FIXTURE GDD14 report (not real physics evidence)";}
        public static void RunBatch()
        {
            output=Arg("--binding-output");if(!Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Fresh isolated output required");Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");
            try{ValidateInputs();RuntimeAndSave();}catch(Exception e){failures++;checks.Add(new JObject{["pass"]=false,["exception"]=e.ToString()});}
            File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--binding-source"),["unity"]=Application.unityVersion,["kind"]="EDITOR_API_CONFIG_AND_SAVE_FIXTURES_NOT_OFFICIAL_C3_NOT_ASSET_OR_UI_INTEGRATION",["failures"]=failures,["checks"]=checks}.ToString());EditorApplication.Exit(failures==0?0:1);
        }
        static void ValidateInputs()
        {
            var c=Catalog();var b=Bindings();
            C("null_product_catalog_rejected_no_fallback",!FullInvestigationRuntime.TryCreateProduct(null,b,out var n).Success&&n==null);
            C("TEST_ONLY_catalog_rejected_for_product",!FullInvestigationRuntime.TryCreateProduct(FullCaseTestCatalog.CreateApproved(),b,out n).Success&&n==null);
            bool blocked=false;try{new FullInvestigationRuntime(c);}catch(InvalidDataException){blocked=true;}C("legacy_constructor_cannot_bypass_product_bindings",blocked);
            Action<string,Action<FullCaseCatalog,FullProductBindings>> bad=(name,change)=>{var x=Catalog();var y=Bindings();change(x,y);var result=FullInvestigationRuntime.TryCreateProduct(x,y,out var runtime);C(name,!result.Success&&runtime==null&&result.Code=="product-config");};
            bad("binding_format_unknown",(x,y)=>y.formatVersion=99);bad("binding_catalog_revision_mismatch",(x,y)=>y.catalogRevision="other");bad("binding_catalog_ID_mismatch",(x,y)=>y.catalogId="other");
            bad("catalog_duplicate_ID",(x,y)=>x.evidence[1].id=x.evidence[0].id);bad("catalog_missing_evidence_reference",(x,y)=>x.comparisons[0].evidenceIds[0]="missing");
            bad("catalog_reserved_test_ID_not_product",(x,y)=>x.conclusions[0].claims[1]="test.claim");
            bad("features_missing",(x,y)=>y.features=new FullFeatureBinding[0]);bad("features_null_collection",(x,y)=>y.features=null);
            bad("feature_null_element",(x,y)=>y.features[0]=null);bad("feature_duplicate_ID",(x,y)=>y.features[1].id=y.features[0].id);
            bad("feature_missing_task",(x,y)=>y.features[0].comparisonId="missing");bad("feature_missing_option",(x,y)=>y.features[0].optionId="missing");bad("feature_wrong_source",(x,y)=>y.features[0].evidenceId="letter");
            bad("feature_wrong_observation_index",(x,y)=>y.features[0].observationIndex=999);bad("feature_empty_description",(x,y)=>y.features[0].description="");
            bad("asset_duplicate_ID",(x,y)=>y.evidenceAssets=new[]{y.evidenceAssets[0],Clone(y.evidenceAssets[0])});bad("asset_missing_evidence_ID",(x,y)=>y.evidenceAssets[0].evidenceId="missing");
            bad("ending_invalid_enum",(x,y)=>y.endings=new[]{new FullEndingMediaBinding{ending=(FullEnding)999,presenterId="presenter"}});
            bad("ending_duplicate",(x,y)=>y.endings=new[]{new FullEndingMediaBinding{ending=FullEnding.Open,presenterId="presenter"},new FullEndingMediaBinding{ending=FullEnding.Open,presenterId="presenter"}});
            bad("ending_missing_presenter_ID",(x,y)=>y.endings=new[]{new FullEndingMediaBinding{ending=FullEnding.Open}});
            var asset=ScriptableObject.CreateInstance<FullProductConfiguration>();
            try{asset.catalog=c;asset.bindings=b;C("configuration_asset_uses_existing_factory",asset.TryCreateRuntime(out var configured).Success&&configured.GetRubricReadiness().Code=="review-pending");}finally{UnityEngine.Object.DestroyImmediate(asset);}
            var s=Create(c,b);C("structural_success_does_not_mean_C3_review",s.GetRubricReadiness().Code=="review-pending");
            var x=Catalog();x.conclusions[0].alternatives=new FullSupportSet[0];C("missing_rubric_explicit_status",Create(x,Bindings()).GetRubricReadiness().Code=="rubric-missing");
            foreach(var rule in c.conclusions)rule.reviewed=true;c.fallChainReviewed=true;C("reviewed_flags_alone_not_receipt",Create(c,b).GetRubricReadiness().Code=="review-pending");
            var test=new FullInvestigationRuntime(FullCaseTestCatalog.CreateApproved());C("test_simulation_still_runs_without_product_media",test.GetRubricReadiness().Success&&test.GetEndingMediaReadiness(FullEnding.Open).Success);
            C("invalid_ending_query_explicit_status",s.GetEndingMediaReadiness(FullEnding.None).Code=="ending-id");
            C("unopened_feature_asset_and_ending_queries_do_not_leak",s.GetFeatureBindings("pair").Length==0&&s.GetEvidenceAssetBinding("sample.a")==null&&s.GetEndingMediaBinding()==null);
            View(s,"sample.a");C("unviewed_second_specimen_withheld",s.GetFeatureBindings("pair").Length==2&&s.GetFeatureBindings("pair").All(f=>f.evidenceId=="sample.a"));View(s,"sample.b");
            var features=s.GetFeatureBindings("pair");features[0].description="changed externally";C("binding_snapshots_isolated_and_no_expectedOption",s.GetFeatureBindings("pair").Length==4&&s.GetFeatureBindings("pair")[0].description!="changed externally"&&!typeof(FullFeatureBinding).GetFields().Any(f=>f.Name=="expectedOption"));
            C("missing_visual_explicit_null_no_test_prefab",s.GetEvidenceAssetBinding("sample.a").inspectionPrefab==null&&s.GetEvidenceAssetBinding("sample.b")==null);
            s.SetModal(FullCaseModal.Conclusions);Must(s.TryConfirm());string before=s.ExportStateJson();var failed=s.TryFinalize(Path.Combine(output,"unreviewed-profile"));
            C("product_finalize_blocked_without_review_no_write",failed.Code=="review-pending"&&before==s.ExportStateJson()&&!File.Exists(FullInvestigationRuntime.SavePathFor(Path.Combine(output,"unreviewed-profile"))));
        }
        static void RuntimeAndSave()
        {
            var c=Catalog();var b=Bindings();Reviewed(c,b);var s=Create(c,b);C("explicit_review_references_are_separate_declarations",s.GetRubricReadiness().Success);
            string profile=Path.Combine(output,"product-profile");s.SetModal(FullCaseModal.Conclusions);Must(s.TryConfirm());Must(s.TryFinalize(profile));s.Cancel();Must(s.TryTravel("station"));
            // Fixture only: set prerequisites in a copied test save to isolate presentation binding.
            var j=JObject.Parse(s.ExportStateJson());j["replySent"]=true;j["replyProducerVerified"]=true;
            ((JArray)j["evidence"]).Add(new JObject{["id"]="letter",["viewed"]=true,["observations"]=new JArray(),["placement"]="office",["slot"]="source"});
            ((JArray)j["decoded"]).Add("letter");((JArray)j["heardStatements"]).Add(new JObject{["id"]="ha.channel.paper",["topicId"]="ha.channel.paper",["participantId"]="npc.ha",["question"]="FIXTURE heard",["answer"]="FIXTURE snapshot",["version"]=1});
            string path=FullInvestigationRuntime.SavePathFor(profile);File.WriteAllText(path,j.ToString());Must(s.TryLoad(profile));byte[] source=File.ReadAllBytes(path);
            C("schema2_unchanged_no_binding_assets_serialized",j.Value<int>("schema")==2&&j["features"]==null&&j["endings"]==null);
            Must(s.TryOpenEndingSummary());string before=s.ExportStateJson();C("missing_media_blocks_start_preserves_state",s.TryStartEndingPresentation().Code=="media-missing"&&!s.EndingPlaying&&before==s.ExportStateJson()&&s.GetEndingMediaBinding()==null);s.Cancel();
            var prefab=new GameObject("TEST ONLY presenter binding fixture");
            try{
                b.endings=new[]{new FullEndingMediaBinding{ending=FullEnding.Open,presenterId="presenter.open",presenterPrefab=prefab}};
                var ready=Create(c,b);Must(ready.TryLoad(profile));
                C("media_only_rebinding_keeps_catalog_save_compatible",source.SequenceEqual(File.ReadAllBytes(path))&&ready.GetEndingMediaReadiness(FullEnding.Open).Success&&ready.GetEndingMediaBinding().presenterPrefab==prefab);
                var media=ready.GetEndingMediaBinding();media.presenterId="changed";C("media_snapshot_isolated",ready.GetEndingMediaBinding().presenterId=="presenter.open");
                Must(ready.TryOpenEndingSummary());Must(ready.TryStartEndingPresentation());ready.SetPaused(true);C("product_pause_cannot_complete",!ready.TryCompleteEndingPresentation(profile).Success&&!ready.EndingViewed);ready.SetPaused(false);ready.Cancel();C("cancel_does_not_mark_viewed",!ready.EndingViewed&&!ready.EndingPlaying);
                Must(ready.TryOpenEndingSummary());Must(ready.TryStartEndingPresentation());UnityEngine.Object.DestroyImmediate(prefab);prefab=null;before=ready.ExportStateJson();
                C("destroyed_media_blocks_completion_no_write",ready.TryCompleteEndingPresentation(profile).Code=="media-missing"&&before==ready.ExportStateJson()&&source.SequenceEqual(File.ReadAllBytes(path)));ready.Cancel();
                var restoredPrefab=new GameObject("TEST ONLY restored presenter fixture");
                try{
                    b.endings[0].presenterPrefab=restoredPrefab;var restored=Create(c,b);Must(restored.TryLoad(profile));
                    byte[] checkpoint=File.ReadAllBytes(FullInvestigationRuntime.CheckpointPathFor(profile));
                    Must(restored.TryOpenEndingSummary());Must(restored.TryStartEndingPresentation());Must(restored.TryCompleteEndingPresentation(profile));
                    C("restored_media_allows_explicit_completion_and_persists_viewed",restored.EndingViewed&&!restored.EndingPlaying&&checkpoint.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.CheckpointPathFor(profile))));
                    source=File.ReadAllBytes(path);Must(restored.TryOpenEndingSummary());Must(restored.TryStartEndingPresentation());Must(restored.TryCompleteEndingPresentation(profile));
                    C("product_replay_no_save_or_checkpoint_rewrite",source.SequenceEqual(File.ReadAllBytes(path))&&checkpoint.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.CheckpointPathFor(profile))));
                }finally{UnityEngine.Object.DestroyImmediate(restoredPrefab);}
                // Changed source semantics or revision must never reinterpret the same save.
                var changed=Bindings();Reviewed(c,changed);changed.features[0].description="FIXTURE changed semantic description";var other=Create(c,changed);before=other.ExportStateJson();
                C("feature_change_same_revision_rejected_by_hash",!other.TryLoad(profile).Success&&before==other.ExportStateJson()&&source.SequenceEqual(File.ReadAllBytes(path)));
                var revision=Clone(c);revision.revision="2";var revised=Bindings();revised.catalogRevision="2";Reviewed(revision,revised);other=Create(revision,revised);before=other.ExportStateJson();
                C("catalog_revision_change_preserves_file_live_state",!other.TryLoad(profile).Success&&before==other.ExportStateJson()&&source.SequenceEqual(File.ReadAllBytes(path)));
                var reordered=Bindings();reordered.features=reordered.features.Reverse().ToArray();Reviewed(c,reordered);Must(Create(c,reordered).TryLoad(profile));C("feature_order_only_not_semantic_migration",source.SequenceEqual(File.ReadAllBytes(path)));
            }finally{if(prefab)UnityEngine.Object.DestroyImmediate(prefab);}
            string legacyProfile=Arg("--binding-legacy-profile");byte[] legacy=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(legacyProfile));var old=new FullInvestigationRuntime(FullCaseTestCatalog.Create());Must(old.TryLoad(legacyProfile));
            C("test_catalog_schema1_migration_still_read_only",JObject.Parse(old.ExportStateJson()).Value<int>("schema")==2&&old.GetHeardStatements().Length==0&&!old.EndingViewed&&legacy.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(legacyProfile))));
        }
    }
}
