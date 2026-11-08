using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // Read-only contract audit. No gameplay progress, Load, UI or runtime patch.
    public static class RelationReferenceAudit
    {
        static string Arg(string key)
        {
            var args=Environment.GetCommandLineArgs();int at=Array.IndexOf(args,key);
            return at>=0&&at+1<args.Length?args[at+1]:"";
        }
        public static void RunBatch()
        {
            string output=Arg("--relation-audit-output"),profile=Arg("--investigation-save-dir");
            if(!Application.isBatchMode||Application.unityVersion!="6000.6.2f1"||EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Explicit batch Unity 6000.6.2f1 outside Play required.");
            if(!Path.IsPathRooted(output)||!Path.IsPathRooted(profile)||Directory.Exists(profile))
                throw new InvalidOperationException("Absolute output and unused private fixture profile required.");
            Directory.CreateDirectory(output);Directory.CreateDirectory(profile);
            var report=new JObject{["unity"]=Application.unityVersion,["kind"]="EDITMODE_SERIALIZATION_VALIDATOR_LOOKUP_API_NOT_LOAD_OR_UI",["profile"]=profile};
            var cases=new JArray();report["cases"]=cases;int exit=2;
            try
            {
                const string scene="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
                EditorSceneManager.OpenScene(scene,OpenSceneMode.Single);report["scene"]=scene;
                Probe(cases,profile,"empty",null,null);
                Probe(cases,profile,"valid_relation","slide","shoeprint");
                Probe(cases,profile,"unknown_first","missing_relation_evidence","shoeprint");
                Probe(cases,profile,"unknown_second","slide","missing_relation_evidence");
                report["gapReproduced"]=(bool)cases[2]["validatorAccepted"]&&(bool)cases[3]["validatorAccepted"]&&(string)cases[2]["consumerLookupException"]=="NullReferenceException"&&(string)cases[3]["consumerLookupException"]=="NullReferenceException";
                report["controlsPassed"]=(bool)cases[0]["validatorAccepted"]&&(bool)cases[1]["validatorAccepted"]&&(string)cases[1]["consumerLookupException"]=="";
                exit=(bool)report["gapReproduced"]&&(bool)report["controlsPassed"]?0:1;
            }
            catch(Exception e){report["auditException"]=e.ToString();}
            finally
            {
                report["exitCode"]=exit;report["runtimeFixImplemented"]=false;
                File.WriteAllText(Path.Combine(output,"results.json"),report.ToString());
                Debug.Log("Relation reference contract audit completed; no runtime fix. "+output);EditorApplication.Exit(exit);
            }
        }
        static void Probe(JArray results,string profile,string name,string first,string second)
        {
            var save=new InvestigationSave();
            save.evidence.Add(new EvidenceRecord{id="slide",collected=true});
            save.evidence.Add(new EvidenceRecord{id="shoeprint",collected=true});
            if(first!=null)save.relations.Add(new EvidenceRelation{first=first,second=second,note="Fixture: chưa đủ chứng minh, cần kiểm thêm."});
            string path=Path.Combine(profile,name+".json");File.WriteAllText(path,JsonUtility.ToJson(save,true));
            var incoming=JsonUtility.FromJson<InvestigationSave>(File.ReadAllText(path));string before=JsonUtility.ToJson(incoming);
            bool accepted=InvestigationSaveValidation.TryValidate(incoming,out string error);
            string exception="",title="";
            // Same definition dereference as DossierUI; no OnGUI or page invoked.
            if(incoming.relations.Count>0){var relation=incoming.relations[0];try{title=InvestigationContent.Get(relation.first).title+" / "+InvestigationContent.Get(relation.second).title;}catch(Exception e){exception=e.GetType().Name;}}
            results.Add(new JObject{["case"]=name,["validatorAccepted"]=accepted,["validatorError"]=error,["inputUnchanged"]=before==JsonUtility.ToJson(incoming),["first"]=first,["second"]=second,["consumerLookupException"]=exception,["resolvedTitles"]=title,["fixturePath"]=path});
        }
    }
}
