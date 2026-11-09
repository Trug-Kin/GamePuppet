using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ConRoi.Investigation.V36.R107
{
    // Read-only validation of the existing schema. Never repairs or grants evidence.
    public static class InvestigationSaveValidation
    {
        // JsonUtility can turn explicit JSON null into empty lists/default records.
        // Check collection tokens first; the typed validator still owns IDs and indices.
        internal static bool TryValidateSerializedCollections(string json, out string error)
        {
            error = "";
            var root = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            if (root["evidence"] == null)
                return Invalid(out error, "Hồ sơ thiếu trường danh sách vật chứng (evidence).");
            foreach (string field in new[] { "evidence", "mapping", "relations", "trials", "conclusions" })
            {
                var token = root[field];
                // Retain existing defaults for absent auxiliary collections in older files.
                if (token == null) continue;
                if (!Collection(token, true))
                    return Invalid(out error, "Danh sách " + field + " trong hồ sơ không hợp lệ; không nhận null hoặc mục null.");
            }
            foreach (var record in (JArray)root["evidence"])
                if (record is JObject item && item.TryGetValue("observations", out var observations) && !Collection(observations, true))
                    return Invalid(out error, "Danh sách quan sát trong hồ sơ không hợp lệ.");
            if (root["conclusions"] is JArray conclusions)
                foreach (var record in conclusions)
                    if (record is JObject item && item.TryGetValue("grounds", out var grounds) && !Collection(grounds, false))
                        return Invalid(out error, "Danh sách căn cứ trong hồ sơ không hợp lệ.");
            return true;
        }
        static bool Collection(JToken token, bool rejectNullItems) => token is JArray array &&
            (!rejectNullItems || array.All(item => item.Type != JTokenType.Null && item.Type != JTokenType.Undefined));

        public static bool TryValidate(InvestigationSave incoming, out string error)
        {
            error = "";
            if (incoming == null || incoming.schema != 1 || incoming.namespaceId != "conroi.investigation.v36.r107")
                return Invalid(out error, "Hồ sơ không đúng phiên bản điều tra; không tự chuyển save cũ.");
            if (!Finite(incoming.position) || !float.IsFinite(incoming.yaw) || !float.IsFinite(incoming.pitch))
                return Invalid(out error, "Vị trí hoặc góc nhìn trong hồ sơ không hợp lệ.");
            if (incoming.gate == null || !float.IsFinite(incoming.gate.angle) || !float.IsFinite(incoming.gate.velocity))
                return Invalid(out error, "Dữ liệu cơ cấu trong hồ sơ không hợp lệ.");
            if (incoming.evidence == null || incoming.mapping == null || incoming.relations == null || incoming.trials == null || incoming.conclusions == null)
                return Invalid(out error, "Hồ sơ thiếu danh sách dữ liệu điều tra.");

            var ids = new HashSet<string>();
            foreach (var record in incoming.evidence)
            {
                var definition = record == null ? null : InvestigationContent.Get(record.id);
                if (definition == null || !ids.Add(record.id))
                    return Invalid(out error, "Vật chứng trong hồ sơ không hợp lệ hoặc bị lặp.");
                if (record.observations == null)
                    return Invalid(out error, "Vật chứng thiếu danh sách quan sát.");
                foreach (int index in record.observations)
                    if (index < 0 || index >= definition.observations.Length)
                        return Invalid(out error, "Chỉ số quan sát trong hồ sơ không hợp lệ.");
            }
            // These collections are dereferenced by existing runtime/UI consumers.
            foreach (var assignment in incoming.mapping)
                if (assignment == null) return Invalid(out error, "Hồ sơ có mục bảng gán không hợp lệ.");
            foreach (var relation in incoming.relations)
            {
                if (relation == null) return Invalid(out error, "Hồ sơ có mục đối chiếu không hợp lệ.");
                if (InvestigationContent.Get(relation.first) == null || InvestigationContent.Get(relation.second) == null)
                    return Invalid(out error, "Liên hệ trong hồ sơ tham chiếu vật chứng không tồn tại.");
            }
            foreach (var trial in incoming.trials)
                if (trial == null) return Invalid(out error, "Hồ sơ có mục thử diễn biến không hợp lệ.");
            foreach (var conclusion in incoming.conclusions)
                if (conclusion == null || conclusion.grounds == null)
                    return Invalid(out error, "Hồ sơ có mục nhận định không hợp lệ.");
            return true;
        }
        static bool Finite(Vector3 position) => float.IsFinite(position.x) && float.IsFinite(position.y) && float.IsFinite(position.z);
        static bool Invalid(out string error, string message) { error = message; return false; }
    }
}
