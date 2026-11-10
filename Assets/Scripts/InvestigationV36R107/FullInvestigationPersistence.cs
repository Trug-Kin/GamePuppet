using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ConRoi.Investigation.V36.R107
{
    public sealed partial class FullInvestigationRuntime
    {
        static string Hash(string text){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-","").ToLowerInvariant();}
        static void Require(bool valid,string reason){if(!valid)throw new InvalidDataException(reason);}
        static bool Ids(IEnumerable<string> ids)=>ids!=null&&ids.All(id=>!string.IsNullOrWhiteSpace(id)&&id==id.Trim())&&ids.Distinct(StringComparer.Ordinal).Count()==ids.Count();
        bool ValidRequirement(FullRequirement r)
        {
            if(r==null||!Enum.IsDefined(typeof(FullRequirementKind),r.kind))return false;
            if(r.kind==FullRequirementKind.Finalized)return r.id==""&&r.index==-1;
            if(string.IsNullOrEmpty(r.id))return false;
            switch(r.kind){
                case FullRequirementKind.Collected:return Evidence(r.id)!=null&&r.index==-1;
                case FullRequirementKind.Recorded:return IsKnownObservation(r.id,r.index);
                case FullRequirementKind.Comparison:return catalog.comparisons.Any(c=>c.id==r.id)&&r.index==-1;
                case FullRequirementKind.Decoded:return FullInvestigationLetters.Ids.Contains(r.id)&&Evidence(r.id)!=null&&r.index==-1;
                case FullRequirementKind.TrialRecorded:return new[]{"release","help","hold","hook","wedge","after"}.Any(id=>r.id==id||r.id==id+"/clear-holds")&&r.index==-1;
                case FullRequirementKind.Permission:return catalog.permissions.Any(p=>p.id==r.id)&&r.index==-1;
                default:return false;
            }
        }
        void ValidateCatalog()
        {
            Require(!string.IsNullOrWhiteSpace(catalog.id)&&!string.IsNullOrWhiteSpace(catalog.revision),"Catalog cần ID và revision.");
            Require(catalog.locations!=null&&catalog.evidence!=null&&catalog.comparisons!=null&&catalog.permissions!=null&&catalog.conclusions!=null&&catalog.replyChannelRequirements!=null,"Catalog thiếu collection.");
            Require(catalog.locations.All(x=>x!=null)&&catalog.evidence.All(x=>x!=null)&&catalog.comparisons.All(x=>x!=null)&&catalog.permissions.All(x=>x!=null)&&catalog.conclusions.All(x=>x!=null),"Catalog có phần tử null.");
            Require(Ids(catalog.locations.Select(l=>l.id))&&Ids(catalog.evidence.Select(e=>e.id))&&Ids(catalog.comparisons.Select(c=>c.id))&&Ids(catalog.permissions.Select(p=>p.id))&&Ids(catalog.conclusions.Select(c=>c.id)),"ID catalog rỗng hoặc trùng.");
            Require(catalog.locations.Any(l=>l.id==catalog.startLocation),"Thiếu nơi bắt đầu.");
            Require(catalog.conclusions.Length==4&&new[]{"sender","presence","fall","motive"}.All(id=>catalog.conclusions.Any(c=>c.id==id)),"Cần đúng bốn mục kết luận GDD7.3.");
            foreach(var l in catalog.locations)Require(l.requires!=null&&l.requires.All(ValidRequirement)&&!string.IsNullOrWhiteSpace(l.label),"Điều kiện địa điểm chưa hợp lệ.");
            foreach(var d in catalog.evidence)Require(catalog.locations.Any(l=>l.id==d.location)&&d.observations!=null&&d.observations.All(x=>x!=null)&&d.placementSlots!=null&&Ids(d.placementSlots)&&d.requires!=null&&d.requires.All(ValidRequirement)&&d.label!=null&&d.source!=null&&d.text!=null,"Nguồn vật chứng chưa hợp lệ.");
            foreach(var c in catalog.comparisons)Require(c.requires!=null&&c.requires.All(ValidRequirement)&&c.evidenceIds!=null&&c.evidenceIds.Length>=2&&Ids(c.evidenceIds)&&c.evidenceIds.All(id=>Evidence(id)!=null)&&c.options!=null&&Ids(c.options)&&c.options.Contains(c.expectedOption),"Bài đối chiếu chưa hợp lệ.");
            foreach(var p in catalog.permissions)Require(catalog.locations.Any(l=>l.id==p.location)&&p.requires!=null&&p.requires.All(ValidRequirement),"Quyền truy cập chưa hợp lệ.");
            foreach(var c in catalog.conclusions)Require(c.claims!=null&&Ids(c.claims)&&c.claims.Contains(c.correctClaim)&&c.alternatives!=null&&c.alternatives.All(a=>a!=null&&a.requires!=null&&a.requires.Length>0&&a.requires.All(ValidRequirement)),"Rubric kết luận chưa hợp lệ.");
            Require(catalog.replyChannelRequirements.All(ValidRequirement),"Điều kiện kênh hồi đáp chưa hợp lệ.");
            // Comparison/permission prerequisites are data, but may not recursively award each other.
            foreach(var c in catalog.comparisons)CheckCycle("C:"+c.id,new HashSet<string>(),new HashSet<string>());
            foreach(var p in catalog.permissions)CheckCycle("P:"+p.id,new HashSet<string>(),new HashSet<string>());
        }
        void CheckCycle(string node,HashSet<string> stack,HashSet<string> done)
        {
            if(done.Contains(node))return;Require(stack.Add(node),"Điều kiện catalog có vòng lặp.");
            var requirements=node.StartsWith("C:")?catalog.comparisons.First(c=>c.id==node.Substring(2)).requires:catalog.permissions.First(p=>p.id==node.Substring(2)).requires;
            foreach(var r in requirements){if(r.kind==FullRequirementKind.Comparison)CheckCycle("C:"+r.id,stack,done);if(r.kind==FullRequirementKind.Permission)CheckCycle("P:"+r.id,stack,done);}
            stack.Remove(node);done.Add(node);
        }
        bool MeetsIn(FullCaseSave s,FullRequirement r,HashSet<string> evaluating=null)
        {
            if(!ValidRequirement(r))return false;
            switch(r.kind){
                case FullRequirementKind.Collected:return s.evidence.Any(e=>e.id==r.id);
                case FullRequirementKind.Recorded:return (r.id!="clip"||(s.clipBeforeSeen&&s.clipEventSeen))&&s.evidence.Any(e=>e.id==r.id&&e.observations.Contains(r.index));
                case FullRequirementKind.Decoded:return s.decoded.Contains(r.id);
                case FullRequirementKind.TrialRecorded:return s.trials.Any(t=>t.id==r.id);
                case FullRequirementKind.Permission:return s.permissions.Contains(r.id);
                case FullRequirementKind.Finalized:return s.finalized;
                case FullRequirementKind.Comparison:
                    evaluating=evaluating??new HashSet<string>();if(!evaluating.Add(r.id))return false;
                    var task=catalog.comparisons.First(c=>c.id==r.id);var a=s.comparisons.FirstOrDefault(c=>c.id==r.id);
                    bool met=a!=null&&a.option==task.expectedOption&&a.evidenceIds.OrderBy(x=>x,StringComparer.Ordinal).SequenceEqual(task.evidenceIds.OrderBy(x=>x,StringComparer.Ordinal))&&task.requires.All(need=>MeetsIn(s,need,evaluating));evaluating.Remove(r.id);return met;
                default:return false;
            }
        }
        bool Citable(FullCaseSave save,FullRequirement requirement)=>ValidRequirement(requirement)&&
            (requirement.kind==FullRequirementKind.Comparison?save.comparisons.Any(a=>a.id==requirement.id):MeetsIn(save,requirement));
        static JObject ParseStrict(string json)
        {
            using(var reader=new JsonTextReader(new StringReader(json))){reader.DateParseHandling=DateParseHandling.None;
                var root=JObject.Load(reader,new JsonLoadSettings{DuplicatePropertyNameHandling=DuplicatePropertyNameHandling.Error});
                if(reader.Read())throw new InvalidDataException("Có dữ liệu sau hồ sơ.");return root;}
        }
        static void ArrayField(JObject obj,string name)
        {
            Require(obj[name] is JArray a&&!a.Any(item=>item.Type==JTokenType.Null),"Collection "+name+" phải là mảng không có null.");
        }
        FullCaseSave ParseSave(string json)
        {
            var parsed=ParseStrict(json);
            Require(parsed["schema"]?.Type==JTokenType.Integer&&parsed.Value<int>("schema")==1&&parsed["namespaceId"]?.Type==JTokenType.String&&parsed["catalogHash"]?.Type==JTokenType.String,"Thiếu schema/namespace/catalog rõ ràng.");
            foreach(var field in new[]{"evidence","mapping","decoded","permissions","comparisons","relations","trials","conclusions","unsupportedDimensions"})ArrayField(parsed,field);
            foreach(var e in (JArray)parsed["evidence"]){Require(e is JObject,"Vật chứng phải là object.");ArrayField((JObject)e,"observations");foreach(var index in (JArray)e["observations"])Require(index.Type==JTokenType.Integer,"Chỉ số quan sát phải nguyên.");}
            foreach(var c in (JArray)parsed["conclusions"]){Require(c is JObject,"Kết luận phải là object.");ArrayField((JObject)c,"grounds");}
            foreach(var c in (JArray)parsed["comparisons"]){Require(c is JObject,"Đối chiếu phải là object.");ArrayField((JObject)c,"evidenceIds");}
            var incoming=JsonUtility.FromJson<FullCaseSave>(json);ValidateState(incoming);return incoming;
        }
        void ValidateState(FullCaseSave s)
        {
            Require(s!=null&&s.schema==1&&s.namespaceId=="conroi.investigation.v36.fullcase"&&s.catalogHash==catalogHash,"Sai schema, namespace hoặc catalog/revision.");
            Require(s.evidence!=null&&s.mapping!=null&&s.decoded!=null&&s.permissions!=null&&s.comparisons!=null&&s.relations!=null&&s.trials!=null&&s.conclusions!=null&&s.unsupportedDimensions!=null,"Thiếu collection.");
            Require(s.evidence.All(e=>e!=null)&&s.mapping.All(m=>m!=null)&&s.comparisons.All(c=>c!=null)&&s.relations.All(r=>r!=null)&&s.trials.All(t=>t!=null)&&s.conclusions.All(c=>c!=null),"Collection có phần tử null.");
            Require(Ids(s.evidence.Select(e=>e.id))&&Ids(s.mapping.Select(m=>m.pose))&&Ids(s.comparisons.Select(c=>c.id))&&Ids(s.conclusions.Select(c=>c.id))&&Ids(s.decoded)&&Ids(s.permissions)&&Ids(s.unsupportedDimensions),"Có ID trùng hoặc rỗng.");
            Require(catalog.locations.Any(l=>l.id==s.location)&&s.held!=null,"Địa điểm hoặc vật cầm sai.");
            foreach(var r in s.evidence){var d=Evidence(r.id);Require(d!=null&&r.observations!=null&&r.observations.Distinct().Count()==r.observations.Count&&r.observations.All(i=>i>=0&&i<d.observations.Length),"Quan sát hoặc ID vật chứng sai.");
                Require(r.placement!=null&&r.slot!=null&&(r.placement=="inventory"||r.placement=="held"||catalog.locations.Any(l=>l.id==r.placement)),"Vị trí vật chứng sai.");
                Require(r.placement!="held"||s.held==r.id,"Vật cầm không nhất quán.");
                Require(r.placement!="held"&&r.placement!="inventory"||d.portable,"Nguồn cố định không thể cầm/cất.");
                Require(r.slot==""||r.slot=="source"||d.placementSlots.Contains(r.slot)&&r.slot.StartsWith(r.placement+".",StringComparison.Ordinal),"Socket không thuộc vật/địa điểm.");
            }
            Require(s.held==""||s.evidence.Any(e=>e.id==s.held&&e.placement=="held"),"Thiếu vật đang cầm.");
            foreach(var m in s.mapping)Require(Enumerable.Range(0,26).Any(i=>m.pose==FullInvestigationLetters.PoseKey((char)('A'+i)))&&m.letter!=null&&m.letter.Length<=1&&m.letter.All(c=>c>='A'&&c<='Z'),"Bảng gán sai.");
            Require(s.decoded.All(id=>FullInvestigationLetters.Ids.Contains(id)&&s.evidence.Any(e=>e.id==id))&&s.permissions.All(id=>catalog.permissions.Any(p=>p.id==id)),"Reference tiến trình sai.");
            foreach(var a in s.comparisons)Require(catalog.comparisons.Any(c=>c.id==a.id&&c.options.Contains(a.option))&&a.evidenceIds!=null&&a.evidenceIds.Length==catalog.comparisons.First(c=>c.id==a.id).evidenceIds.Length&&Ids(a.evidenceIds)&&a.evidenceIds.All(id=>s.evidence.Any(e=>e.id==id)),"Reference đối chiếu sai.");
            foreach(var r in s.relations)Require(s.evidence.Any(e=>e.id==r.first)&&s.evidence.Any(e=>e.id==r.second)&&r.note!=null,"Relation reference sai.");
            foreach(var t in s.trials)Require(new[]{"release","help","hold","hook","wedge","after"}.Any(id=>t.id==id||t.id==id+"/clear-holds")&&t.report!=null&&Finite(t.returnSeconds),"Trial reference sai.");
            Require(s.conclusions.Count==4&&catalog.conclusions.All(c=>s.conclusions.Any(d=>d.id==c.id)),"Cần đủ bốn bản ghi.");
            foreach(var c in s.conclusions)Require(c.claim!=null&&(c.claim==""||catalog.conclusions.First(r=>r.id==c.id).claims.Contains(c.claim))&&c.grounds!=null&&c.grounds.All(g=>ValidRequirement(g)&&Citable(s,g)),"Căn cứ/lựa chọn không hợp lệ.");
            Require(s.gate!=null&&Finite(s.gate.angle)&&Finite(s.gate.velocity)&&s.gate.angle>=0&&s.gate.angle<=82&&Finite(s.clipBeforeExposure)&&Finite(s.clipEventExposure)&&s.clipBeforeExposure>=0&&s.clipEventExposure>=0,"State cơ cấu/media sai.");
            if(s.finalized){Require(Assess(s,out var ending,out var dimensions)&&s.ending==ending&&s.unsupportedDimensions.OrderBy(x=>x).SequenceEqual(dimensions.OrderBy(x=>x)),"Kết quả không khớp rubric đã review.");}
            else Require(s.ending==FullEnding.None&&s.unsupportedDimensions.Count==0,"Bản chưa chốt không được chứa ending.");
        }
        public static string SavePathFor(string profile)=>Path.Combine(Path.GetFullPath(profile),"InvestigationV36","fullcase","case.json");
        public static string CheckpointPathFor(string profile)=>Path.Combine(Path.GetDirectoryName(SavePathFor(profile)),"case.before-conclusion.json");
        public FullActionResult TrySave(string profile)
        {
            if(notifying)return Fail("busy","Đang xử lý thay đổi.");
            try{Require(!string.IsNullOrWhiteSpace(profile),"Cần profile riêng.");ValidateState(state);InvestigationSaveFile.Write(SavePathFor(profile),ExportStateJson());return Done("Đã lưu hồ sơ toàn tuyến riêng.");}
            catch(Exception){return Fail("write","Không lưu được hồ sơ; bản lưu hợp lệ trước đó được giữ.");}
        }
        public FullActionResult TryLoad(string profile,bool checkpoint=false)
        {
            if(notifying)return Fail("busy","Đang xử lý thay đổi.");
            try{Require(!string.IsNullOrWhiteSpace(profile),"Cần profile riêng.");string path=checkpoint?CheckpointPathFor(profile):SavePathFor(profile);
                if(!File.Exists(path))return Fail("missing","Chưa có bản lưu được chọn.");
                var incoming=ParseSave(File.ReadAllText(path));state=incoming;ResetTransient();Publish(false);return Done("Đã tải hồ sơ; thao tác đang chạy trở về trạng thái an toàn.");}
            catch(InvalidDataException e){return Fail("validation","Không tải: "+e.Message);}
            catch(JsonException){return Fail("json","Không tải: nội dung hồ sơ không đúng định dạng.");}
            catch(Exception){return Fail("read","Không tải: không đọc được hồ sơ. Bản hiện tại được giữ.");}
        }
        public FullActionResult TryFinalize(string profile)
        {
            if(!Ready(out var e))return e;
            if(!Confirmed)return Fail("confirmation","Hãy xác nhận bản ghi hiện tại.");
            if(!Assess(state,out var ending,out var dimensions))return Fail("content-pending","Chưa có rubric và review chuỗi gây ngã để chốt đánh giá. Bản ghi vẫn được giữ.");
            try{Require(!string.IsNullOrWhiteSpace(profile),"Cần profile riêng.");var draft=Copy(state);draft.finalized=false;draft.ending=FullEnding.None;draft.unsupportedDimensions.Clear();ValidateState(draft);
                InvestigationSaveFile.Write(CheckpointPathFor(profile),JsonUtility.ToJson(draft,true));
                var pending=Copy(draft);pending.finalized=true;pending.ending=ending;pending.unsupportedDimensions=dimensions;ValidateState(pending);
                InvestigationSaveFile.Write(SavePathFor(profile),JsonUtility.ToJson(pending,true));state=pending;confirmation=null;TrialRunning=false;MediaPlaying=false;Publish(false);return Done("Đã chốt hồ sơ; có bản trước chốt riêng.");}
            catch(Exception){return Fail("write","Không chốt: chưa ghi được tệp. Bản chính hợp lệ trước đó được giữ; khắc phục rồi thử lại.");}
        }
        // Explicit read-only compatibility import. No new location/permission/decoded/topic
        // history is guessed from old flags. It never rewrites the source file.
        public FullActionResult TryImportPrototype(string sourcePath)
        {
            if(!Ready(out var e))return e;
            if(state.evidence.Count!=0||state.mapping.Count!=0||state.comparisons.Count!=0)return Fail("not-empty","Chỉ nhập vào phiên toàn tuyến trống để bảo toàn tiến trình.");
            try{string json=File.ReadAllText(sourcePath);
                if(!InvestigationSaveValidation.TryValidateSerializedCollections(json,out var reason))return Fail("validation","Không nhập: "+reason);
                var old=JsonUtility.FromJson<InvestigationSave>(json);if(!InvestigationSaveValidation.TryValidate(old,out reason))return Fail("validation","Không nhập: "+reason);
                var incoming=FreshState();
                foreach(var r in old.evidence){var d=Evidence(r.id);Require(d!=null,"Catalog chưa biểu diễn một ID cũ.");incoming.evidence.Add(new FullEvidenceRecord{id=r.id,viewed=false,observations=new List<int>(r.observations),placement=d.portable?"inventory":d.location,slot=d.portable?"":"source"});}
                foreach(var m in old.mapping)incoming.mapping.Add(new CharacterAssignment{pose=FullInvestigationLetters.PoseKey(m.pose[0]),letter=m.letter});
                incoming.relations=old.relations.Select(Copy).ToList();incoming.trials=old.trials.Select(Copy).ToList();incoming.clipBeforeSeen=old.clipBeforeSeen;incoming.clipEventSeen=old.clipEventSeen;incoming.clipBeforeExposure=old.clipBeforeExposure;incoming.clipEventExposure=old.clipEventExposure;incoming.gate=Copy(old.gate);
                ValidateState(incoming);state=incoming;ResetTransient();Publish(false);return Done("Đã nhập dữ liệu r107 đã kiểm. Chưa suy tiến trình toàn tuyến hoặc kết luận mới; tệp nguồn không đổi.");}
            catch(Exception){return Fail("import","Không nhập được hồ sơ r107; dữ liệu hiện tại được giữ.");}
        }
    }
}
