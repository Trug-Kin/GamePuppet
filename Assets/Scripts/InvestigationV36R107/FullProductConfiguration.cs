using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace ConRoi.Investigation.V36.R107
{
    [Serializable] public sealed class FullFeatureBinding
    {
        public string id,comparisonId,optionId,evidenceId,label,description;
        public int observationIndex;
        public Texture2D image; // Optional: description is always required.
    }
    [Serializable] public sealed class FullEvidenceAssetBinding
    {
        public string evidenceId;
        public GameObject inspectionPrefab;
        public Texture2D image;
    }
    [Serializable] public sealed class FullEndingMediaBinding
    {
        public FullEnding ending;
        public string presenterId;
        // At least one is needed at playback. A prefab can host the scene's presenter.
        public VideoClip video;
        public AnimationClip animation;
        public GameObject presenterPrefab;
    }
    [Serializable] public sealed class FullProductBindings
    {
        public int formatVersion=1;
        public string catalogId,catalogRevision;
        public FullFeatureBinding[] features=new FullFeatureBinding[0];
        public FullEvidenceAssetBinding[] evidenceAssets=new FullEvidenceAssetBinding[0];
        public FullEndingMediaBinding[] endings=new FullEndingMediaBinding[0];
        // References to actual review reports, supplied by the content owner; not auto-attestation.
        public string reviewedCatalogRevision,reviewReference,fallChainReviewReference;
    }
    [CreateAssetMenu(menuName="Con Roi/Noi dung/Full case product configuration")]
    public sealed class FullProductConfiguration : ScriptableObject
    {
        public FullCaseCatalog catalog;
        public FullProductBindings bindings;
        public FullActionResult TryCreateRuntime(out FullInvestigationRuntime runtime)=>FullInvestigationRuntime.TryCreateProduct(catalog,bindings,out runtime);
    }
    public sealed partial class FullInvestigationRuntime
    {
        readonly FullProductBindings productBindings;
        public static FullActionResult TryCreateProduct(FullCaseCatalog definitions,FullProductBindings bindings,out FullInvestigationRuntime runtime)
        {
            runtime=null;
            if(definitions==null||definitions.testOnly)return FullActionResult.Fail("product-catalog","Cần catalog sản phẩm tường minh; không dùng catalog TEST ONLY.");
            try{runtime=new FullInvestigationRuntime(definitions,bindings);return FullActionResult.Ok("Cấu trúc hợp lệ; vẫn cần kiểm riêng readiness/review và media trước sử dụng.");}
            catch(InvalidDataException e){return FullActionResult.Fail("product-config",e.Message);}
            catch(Exception){return FullActionResult.Fail("product-config","Cấu hình sản phẩm không đọc được; không thay bằng dữ liệu thử.");}
        }
        static bool ProductId(string id)=>!string.IsNullOrWhiteSpace(id)&&id==id.Trim()&&!id.StartsWith("test.",StringComparison.Ordinal);
        void ValidateProductBindings()
        {
            Require(productBindings!=null,"Cần bindings sản phẩm; dùng TryCreateProduct để nhận lỗi cấu hình.");
            Require(productBindings.formatVersion==1&&productBindings.catalogId==catalog.id&&productBindings.catalogRevision==catalog.revision,"Sai format bindings hoặc phiên bản catalog.");
            Require(ProductId(catalog.id)&&catalog.locations.All(x=>ProductId(x.id))&&catalog.evidence.All(x=>ProductId(x.id))&&catalog.permissions.All(x=>ProductId(x.id))&&catalog.comparisons.All(x=>ProductId(x.id)&&x.options.All(ProductId))&&catalog.conclusions.All(x=>ProductId(x.id)&&x.claims.All(ProductId)),"ID test.* được dành riêng cho dữ liệu thử, không dùng trong sản phẩm.");
            Require(catalog.evidence.All(x=>!string.IsNullOrWhiteSpace(x.label)&&!string.IsNullOrWhiteSpace(x.source)&&!string.IsNullOrWhiteSpace(x.text)&&x.observations.All(o=>!string.IsNullOrWhiteSpace(o))),"Nguồn sản phẩm thiếu nhãn/mô tả/quan sát.");
            Require(productBindings.features!=null&&productBindings.evidenceAssets!=null&&productBindings.endings!=null,"Thiếu collection bindings.");
            Require(productBindings.features.All(x=>x!=null)&&productBindings.evidenceAssets.All(x=>x!=null)&&productBindings.endings.All(x=>x!=null),"Bindings có phần tử null.");
            Require(Ids(productBindings.features.Select(x=>x.id))&&productBindings.features.All(x=>ProductId(x.id)),"Feature ID rỗng/trùng hoặc thuộc test.*.");
            Require(Ids(productBindings.evidenceAssets.Select(x=>x.evidenceId)),"Evidence binding ID rỗng/trùng.");
            foreach(var a in productBindings.evidenceAssets)Require(Evidence(a.evidenceId)!=null,"Evidence asset tham chiếu ID không tồn tại.");
            foreach(var f in productBindings.features){var task=catalog.comparisons.FirstOrDefault(t=>t.id==f.comparisonId);
                Require(task!=null&&task.options.Contains(f.optionId)&&task.evidenceIds.Contains(f.evidenceId)&&IsKnownObservation(f.evidenceId,f.observationIndex)&&!string.IsNullOrWhiteSpace(f.label)&&!string.IsNullOrWhiteSpace(f.description),"Feature thiếu nội dung hoặc tham chiếu task/option/nguồn/index sai.");}
            foreach(var task in catalog.comparisons)foreach(var option in task.options)foreach(var source in task.evidenceIds)
                Require(productBindings.features.Any(f=>f.comparisonId==task.id&&f.optionId==option&&f.evidenceId==source),"Thiếu feature mô tả một option/nguồn của bài đối chiếu.");
            Require(productBindings.endings.Select(x=>x.ending).Distinct().Count()==productBindings.endings.Length&&productBindings.endings.All(x=>x.ending!=FullEnding.None&&Enum.IsDefined(typeof(FullEnding),x.ending)&&ProductId(x.presenterId)),"Ending binding trùng/không tồn tại hoặc thiếu presenter ID.");
        }
        public FullActionResult GetRubricReadiness()
        {
            if(TestOnly)return FullActionResult.Ok("Mô phỏng TEST ONLY, không chứng nhận review sản phẩm.");
            if(catalog.conclusions.Any(r=>r.alternatives.Length==0))return FullActionResult.Fail("rubric-missing","Chưa có bộ căn cứ cho đủ bốn kết luận sản phẩm.");
            if(catalog.conclusions.Any(r=>!r.reviewed)||!catalog.fallChainReviewed||productBindings.reviewedCatalogRevision!=catalog.revision||string.IsNullOrWhiteSpace(productBindings.reviewReference)||string.IsNullOrWhiteSpace(productBindings.fallChainReviewReference))
                return FullActionResult.Fail("review-pending","Chưa có đủ biên nhận review C3/GDD14 cho phiên bản catalog này.");
            return FullActionResult.Ok("Đã khai báo biên nhận review; phía nội dung chịu trách nhiệm bằng chứng thật trong tài liệu được dẫn.");
        }
        public FullActionResult GetEndingMediaReadiness(FullEnding ending)
        {
            if(ending==FullEnding.None||!Enum.IsDefined(typeof(FullEnding),ending))return FullActionResult.Fail("ending-id","Chưa chọn một kết quả ending hợp lệ.");
            if(TestOnly)return FullActionResult.Ok("Presenter mô phỏng TEST ONLY, không phải media sản phẩm.");
            var m=productBindings.endings.FirstOrDefault(x=>x.ending==ending);
            return m!=null&&(m.video||m.animation||m.presenterPrefab)?FullActionResult.Ok():FullActionResult.Fail("media-missing","Chưa có media/presenter sản phẩm cho kết quả ending này.");
        }
        string ProductFeatureSignature()=>"\nproduct-features-v1\n"+string.Join("\n",productBindings.features.OrderBy(f=>f.id,StringComparer.Ordinal).Select(f=>JsonUtility.ToJson(new FullFeatureBinding{
            id=f.id,comparisonId=f.comparisonId,optionId=f.optionId,evidenceId=f.evidenceId,observationIndex=f.observationIndex,label=f.label,description=f.description})));
        public FullFeatureBinding[] GetFeatureBindings(string comparisonId)
        {
            if(productBindings==null||!GetComparisons().Any(t=>t.id==comparisonId))return Array.Empty<FullFeatureBinding>();
            return productBindings.features.Where(f=>f.comparisonId==comparisonId&&Record(f.evidenceId)?.viewed==true&&Readable(f.evidenceId)).Select(Copy).ToArray();
        }
        public FullEvidenceAssetBinding GetEvidenceAssetBinding(string evidenceId)=>productBindings!=null&&Record(evidenceId)?.viewed==true&&Readable(evidenceId)?
            CopyAsset(productBindings.evidenceAssets.FirstOrDefault(x=>x.evidenceId==evidenceId)):null;
        public FullEndingMediaBinding GetEndingMediaBinding()=>productBindings!=null&&state.finalized?
            CopyMedia(productBindings.endings.FirstOrDefault(x=>x.ending==state.ending)):null;
        static FullEvidenceAssetBinding CopyAsset(FullEvidenceAssetBinding value)=>value==null?null:Copy(value);
        static FullEndingMediaBinding CopyMedia(FullEndingMediaBinding value)=>value==null?null:Copy(value);
    }
}
