using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConRoi.Investigation.V36.R107
{
    // Transient conversation context. No rewards, persistence or product UI dependency.
    public sealed class InvestigationInterview
    {
        readonly InvestigationSession source;
        readonly InterviewTopicDefinition[] order;
        readonly Dictionary<string, InterviewTopicDefinition> topics = new Dictionary<string, InterviewTopicDefinition>(StringComparer.Ordinal);
        string participant = "";
        InvestigationSave contextData;
        bool notifying;
        public event Action<InterviewReply> Answered;
        public string ActiveParticipantId => source && source.data != null && ReferenceEquals(contextData, source.data) ? participant : "";

        public InvestigationInterview(InvestigationSession source, IEnumerable<InterviewTopicDefinition> definitions)
        {
            if (!source) throw new ArgumentNullException(nameof(source));
            if (definitions == null) throw new ArgumentNullException(nameof(definitions));
            this.source = source; order = definitions.ToArray();
            foreach (var definition in order)
            {
                if (definition == null || topics.ContainsKey(definition.Id)) throw new ArgumentException("Null or duplicate topic ID in catalog", nameof(definitions));
                topics.Add(definition.Id, definition);
            }
        }
        public bool TryBeginInterview(string participantId, out string error)
        {
            if (!source || source.data == null) { error = "Phiên điều tra chưa sẵn sàng."; return false; }
            if (source.Paused) { error = "Hãy tiếp tục trò chơi trước khi hỏi."; return false; }
            if (string.IsNullOrEmpty(participantId) || !order.Any(t => t.ParticipantId == participantId))
            { error = "Không có chủ đề cho người đối thoại này."; return false; }
            participant = participantId; contextData = source.data; error = ""; return true;
        }
        public void EndInterview() { participant = ""; contextData = null; }
        InterviewTopicAvailability ContextAvailability()
        {
            if (!source || source.data == null) return InterviewTopicAvailability.SessionUnavailable;
            if (participant == "") return InterviewTopicAvailability.NoConversation;
            if (!ReferenceEquals(contextData, source.data)) return InterviewTopicAvailability.ContextExpired;
            return source.Paused ? InterviewTopicAvailability.Paused : InterviewTopicAvailability.Available;
        }
        static string Reason(InterviewTopicAvailability availability)
        {
            switch (availability)
            {
                case InterviewTopicAvailability.Locked: return "Chưa đủ điều kiện để hỏi chủ đề này.";
                case InterviewTopicAvailability.UnknownTopic: return "Không có chủ đề này trong cuộc trao đổi hiện tại.";
                case InterviewTopicAvailability.NoConversation: return "Chưa bắt đầu cuộc trao đổi.";
                case InterviewTopicAvailability.ContextExpired: return "Phiên dữ liệu đã đổi; hãy bắt đầu lại cuộc trao đổi.";
                case InterviewTopicAvailability.Paused: return "Hãy tiếp tục trò chơi trước khi hỏi.";
                case InterviewTopicAvailability.InvalidCondition: return "Dữ liệu điều kiện của chủ đề chưa hợp lệ.";
                case InterviewTopicAvailability.SessionUnavailable: return "Phiên điều tra chưa sẵn sàng.";
                default: return "";
            }
        }
        InterviewTopicAvailability Requirements(InterviewTopicDefinition definition)
        {
            bool unlocked = true;
            foreach (var requirement in definition.Requirements)
            {
                var evidence = InvestigationContent.Get(requirement.EvidenceId);
                if (evidence == null) return InterviewTopicAvailability.InvalidCondition;
                if (requirement.Kind == InterviewRequirementKind.AcquiredEvidence)
                {
                    if (requirement.ObservationIndex != -1) return InterviewTopicAvailability.InvalidCondition;
                    unlocked &= source.data.evidence != null && source.data.evidence.Any(r => r != null && r.id == requirement.EvidenceId);
                }
                else if (requirement.Kind == InterviewRequirementKind.RecordedObservation)
                {
                    if (evidence.observations == null || requirement.ObservationIndex < 0 || requirement.ObservationIndex >= evidence.observations.Length)
                        return InterviewTopicAvailability.InvalidCondition;
                    unlocked &= source.TryGetRecordedNote(requirement.EvidenceId, requirement.ObservationIndex, out _, out _);
                }
                else return InterviewTopicAvailability.InvalidCondition;
            }
            return unlocked ? InterviewTopicAvailability.Available : InterviewTopicAvailability.Locked;
        }
        public InterviewTopicState GetTopicState(string topicId)
        {
            var availability = ContextAvailability();
            if (availability != InterviewTopicAvailability.Available) return new InterviewTopicState(topicId ?? "", "", availability, Reason(availability));
            if (string.IsNullOrEmpty(topicId) || !topics.TryGetValue(topicId, out var definition) || definition.ParticipantId != participant)
                return new InterviewTopicState(topicId ?? "", "", InterviewTopicAvailability.UnknownTopic, Reason(InterviewTopicAvailability.UnknownTopic));
            availability = Requirements(definition);
            return new InterviewTopicState(topicId, availability == InterviewTopicAvailability.Available ? definition.Question : "", availability, Reason(availability));
        }
        public InterviewTopicState[] GetTopics()
        {
            if (ActiveParticipantId == "") return Array.Empty<InterviewTopicState>();
            return order.Where(t => t.ParticipantId == participant).Select(t => GetTopicState(t.Id)).ToArray();
        }
        public InterviewTopicState[] GetAvailableTopics() => GetTopics().Where(t => t.CanAsk).ToArray();
        public bool TryAsk(string topicId, out InterviewReply reply, out string error)
        {
            reply = default;
            if (notifying) { error = "Đang xử lý phản hồi của câu hỏi trước."; return false; }
            var state = GetTopicState(topicId);
            if (!state.CanAsk) { error = state.Reason; return false; }
            reply = new InterviewReply(topics[topicId]); error = "";
            if (Answered != null)
            {
                notifying = true;
                try
                {
                    foreach (Action<InterviewReply> listener in Answered.GetInvocationList())
                        try { listener(reply); } catch (Exception e) { Debug.LogWarning("Interview listener failed: " + e.GetType().Name); }
                }
                finally { notifying = false; }
            }
            return true;
        }
    }
}
