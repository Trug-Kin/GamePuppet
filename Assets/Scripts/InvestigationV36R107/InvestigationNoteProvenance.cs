using System;
using System.Collections.Generic;
using System.Linq;

namespace ConRoi.Investigation.V36.R107
{
    public enum NoteProvenanceKind { Unclassified, Observation, Statement, Hypothesis }
    public enum NoteProvenanceStatus { MissingMetadata, Classified, ContentReviewPending, LegacySourceUnresolved }
    public enum InvestigationNoteOrigin { RecordedEvidence, PlayerRelation, PlayerConclusion, TrialResult }

    // Derived, immutable UI snapshot. None of these fields are serialized into InvestigationSave.
    public readonly struct InvestigationNote
    {
        public readonly InvestigationNoteOrigin Origin;
        public readonly string EvidenceId, RelatedEvidenceId, Text, Source, Conditions, Limitations;
        // Observation index for RecordedEvidence; current list index for other origins (not a durable save ID).
        public readonly int EntryIndex;
        public readonly NoteProvenanceKind Kind;
        public readonly NoteProvenanceStatus Status;
        public string KindLabel => Kind == NoteProvenanceKind.Observation ? "Quan sát" : Kind == NoteProvenanceKind.Statement ? "Lời kể" : Kind == NoteProvenanceKind.Hypothesis ? "Giả thuyết" : "Chưa phân loại";
        public string StatusLabel => Status == NoteProvenanceStatus.Classified ? "Đã phân loại nguồn" : Status == NoteProvenanceStatus.LegacySourceUnresolved ? "Dữ liệu legacy • chưa tách nguồn" : Status == NoteProvenanceStatus.ContentReviewPending ? "CHỜ REVIEW NỘI DUNG" : "Thiếu metadata";
        internal InvestigationNote(InvestigationNoteOrigin origin, string id, string relatedId, int index, string text,
            NoteProvenanceKind kind, NoteProvenanceStatus status, string source, string conditions = "", string limitations = "")
        { Origin = origin; EvidenceId = id; RelatedEvidenceId = relatedId; EntryIndex = index; Text = text; Kind = kind; Status = status; Source = source; Conditions = conditions; Limitations = limitations; }
    }

    internal static class InvestigationNoteMetadata
    {
        internal readonly struct Entry
        {
            internal readonly NoteProvenanceKind Kind; internal readonly string Source;
            internal NoteProvenanceStatus Status => Kind == NoteProvenanceKind.Unclassified ? NoteProvenanceStatus.LegacySourceUnresolved : NoteProvenanceStatus.Classified;
            internal Entry(NoteProvenanceKind kind, string source) { Kind = kind; Source = source; }
        }
        // Explicit ID/index mapping. Never infer provenance from translated text, selection or truth assessment.
        static readonly Dictionary<string, Entry[]> entries = new Dictionary<string, Entry[]>(StringComparer.Ordinal)
        {
            { "slide", new[] { new Entry(NoteProvenanceKind.Observation, "Xưởng • mặt sàn cạnh mép gác"), new Entry(NoteProvenanceKind.Observation, "Xưởng • mặt sàn cạnh mép gác") } },
            { "shoeprint", new[] { new Entry(NoteProvenanceKind.Observation, "Xưởng • dấu đế trên gác"), new Entry(NoteProvenanceKind.Observation, "Xưởng • dấu đế trên gác") } },
            { "r03", new[] { new Entry(NoteProvenanceKind.Observation, "Xưởng • nhãn R-03 và dây quấn"), new Entry(NoteProvenanceKind.Observation, "Xưởng • nhãn R-03 và dây quấn") } },
            { "r12", new[] { new Entry(NoteProvenanceKind.Observation, "Xưởng • thẻ sửa và rối R-12"), new Entry(NoteProvenanceKind.Observation, "Xưởng • thẻ sửa và rối R-12") } },
            { "mechanism", new[] { new Entry(NoteProvenanceKind.Observation, "Xưởng • cánh, chốt và dây nhả"), new Entry(NoteProvenanceKind.Unclassified, "Xưởng • cánh, chốt và dây nhả"), new Entry(NoteProvenanceKind.Observation, "Xưởng • cánh, chốt và dây nhả") } },
            { "hook", new[] { new Entry(NoteProvenanceKind.Unclassified, "Xưởng / bàn mô hình • móc M") } },
            { "fabric", new[] { new Entry(NoteProvenanceKind.Observation, "Xưởng • mảnh vải tại tay nắm D"), new Entry(NoteProvenanceKind.Observation, "Xưởng • mảnh vải tại tay nắm D") } },
            { "phone", new[] { new Entry(NoteProvenanceKind.Observation, "Xưởng • điện thoại và giá quay"), new Entry(NoteProvenanceKind.Observation, "Xưởng • điện thoại và giá quay") } },
            { "clip", new[] { new Entry(NoteProvenanceKind.Observation, "Điện thoại • đoạn quay sửa R-12"), new Entry(NoteProvenanceKind.Observation, "Điện thoại • đoạn quay sửa R-12"), new Entry(NoteProvenanceKind.Observation, "Điện thoại • đoạn quay sửa R-12") } },
            { "khai", new[] { new Entry(NoteProvenanceKind.Observation, "Cửa xưởng • ngoại hình Khải nhìn thấy") } },
            { "samples", new[] { new Entry(NoteProvenanceKind.Unclassified, "Phương • bàn giao mẫu prototype") } },
            { "coat", new[] { new Entry(NoteProvenanceKind.Observation, "Bàn mẫu xưởng • cổ tay áo được bàn giao"), new Entry(NoteProvenanceKind.Observation, "Bàn mẫu xưởng • cổ tay áo được bàn giao") } },
            { "shoe", new[] { new Entry(NoteProvenanceKind.Observation, "Bàn mẫu xưởng • đế giày được bàn giao"), new Entry(NoteProvenanceKind.Observation, "Bàn mẫu xưởng • đế giày được bàn giao") } },
            { "shoecontrol", new[] { new Entry(NoteProvenanceKind.Observation, "Bàn mẫu xưởng • giày mẫu B") } },
            { "ha", new[] { new Entry(NoteProvenanceKind.Statement, "Quầy Hà • lời Hà kể"), new Entry(NoteProvenanceKind.Statement, "Quầy Hà • lời Hà kể") } },
            { "letter", new[] { new Entry(NoteProvenanceKind.Observation, "Quầy Hà • dải thư tư thế"), new Entry(NoteProvenanceKind.Unclassified, "Quầy Hà • dải thư tư thế") } },
            { "receipt", new[] { new Entry(NoteProvenanceKind.Observation, "Hà giao • nội dung đọc trên phiếu vận chuyển") } },
            { "henri", new[] { new Entry(NoteProvenanceKind.Observation, "Sổ nghề Henri • nội dung bảng tập") } },
            { "draft", new[] { new Entry(NoteProvenanceKind.Unclassified, "Hà giao • giấy mẫu và bản nháp") } },
            { "decoded", new[] { new Entry(NoteProvenanceKind.Unclassified, "Người chơi • bản đọc mật mã đã ghi") } },
        };
        internal static bool TryGet(string id, int index, out Entry entry)
        {
            entry = default;
            if (id == null || !entries.TryGetValue(id, out var list) || index < 0 || index >= list.Length) return false;
            entry = list[index]; return true;
        }
    }

    public sealed partial class InvestigationSession
    {
        public bool TryGetRecordedNote(string evidenceId, int observationIndex, out InvestigationNote note, out string error)
        {
            note = default;
            var definition = InvestigationContent.Get(evidenceId);
            if (definition == null) { error = "Không có mục vật chứng này."; return false; }
            if (definition.observations == null || observationIndex < 0 || observationIndex >= definition.observations.Length)
            { error = "Không có mục ghi nhận ở vị trí này."; return false; }
            if (data?.evidence == null || !data.evidence.Any(r => r != null && r.id == evidenceId && r.observations != null && r.observations.Contains(observationIndex)))
            { error = "Mục này chưa được ghi vào hồ sơ."; return false; }
            // Same visibility rule as EvidenceTranscript/Observe: old or injected records cannot bypass clip exposure.
            if (evidenceId == "clip" && (!data.clipBeforeSeen || !data.clipEventSeen))
            { error = "Hãy xem cả trước và trong sự cố trước khi đọc ghi nhận clip."; return false; }
            bool mapped = InvestigationNoteMetadata.TryGet(evidenceId, observationIndex, out var metadata);
            note = new InvestigationNote(InvestigationNoteOrigin.RecordedEvidence, evidenceId, "", observationIndex,
                definition.observations[observationIndex] ?? "", mapped ? metadata.Kind : NoteProvenanceKind.Unclassified,
                mapped ? metadata.Status : NoteProvenanceStatus.MissingMetadata, mapped ? metadata.Source : "Chưa xác định nguồn");
            error = ""; return true;
        }

        public InvestigationNote[] GetProvenanceNotes()
        {
            var notes = new List<InvestigationNote>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            if (data?.evidence != null)
                foreach (var record in data.evidence)
                    if (record?.observations != null)
                        foreach (int index in record.observations)
                            if (TryGetRecordedNote(record.id, index, out var note, out _) && seen.Add(record.id + ":" + index)) notes.Add(note);
            if (data?.relations != null)
                for (int i = 0; i < data.relations.Count; i++)
                {
                    var r = data.relations[i];
                    if (r == null || string.IsNullOrWhiteSpace(r.note) || !HasNoteEvidence(r.first) || !HasNoteEvidence(r.second)) continue;
                    notes.Add(new InvestigationNote(InvestigationNoteOrigin.PlayerRelation, r.first, r.second, i, r.note,
                        NoteProvenanceKind.Hypothesis, NoteProvenanceStatus.Classified, "Người chơi • liên hệ tự ghi"));
                }
            if (data?.conclusions != null)
                for (int i = 0; i < data.conclusions.Count; i++)
                {
                    var c = data.conclusions[i];
                    if (c == null || string.IsNullOrWhiteSpace(c.claim)) continue;
                    notes.Add(new InvestigationNote(InvestigationNoteOrigin.PlayerConclusion, "", "", i, c.claim,
                        NoteProvenanceKind.Hypothesis, NoteProvenanceStatus.Classified, "Người chơi • nhận định tự ghi"));
                }
            if (data?.trials != null)
                for (int i = 0; i < data.trials.Count; i++)
                {
                    var t = data.trials[i];
                    if (t == null || string.IsNullOrWhiteSpace(t.report)) continue;
                    bool known = t.id != null && new[] { "release", "help", "hold", "hook", "wedge", "after" }.Any(id => t.id == id || t.id == id + "/clear-holds");
                    notes.Add(new InvestigationNote(InvestigationNoteOrigin.TrialResult, t.id ?? "", "", i, t.report,
                        known ? NoteProvenanceKind.Observation : NoteProvenanceKind.Unclassified, known ? NoteProvenanceStatus.Classified : NoteProvenanceStatus.MissingMetadata,
                        "Mô hình dựng lại", "Ca " + (t.id ?? "chưa biết") + (t.returnSeconds < 0 ? " • chưa đo thời gian hồi trong record" : " • hồi " + t.returnSeconds),
                        "Quan sát mô hình với điều kiện thử đã ghi; không phải sự kiện hiện trường đã chứng minh hoặc xác nhận vật lý thật."));
                }
            return notes.ToArray();
        }
        bool HasNoteEvidence(string id) => InvestigationContent.Get(id) != null && data?.evidence != null && data.evidence.Any(r => r != null && r.id == id);
    }
}
