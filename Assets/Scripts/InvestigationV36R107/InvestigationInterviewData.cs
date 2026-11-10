using System;
using System.Linq;

namespace ConRoi.Investigation.V36.R107
{
    public enum InterviewRequirementKind { AcquiredEvidence, RecordedObservation }
    public readonly struct InterviewRequirement
    {
        public readonly InterviewRequirementKind Kind;
        public readonly string EvidenceId;
        public readonly int ObservationIndex;
        public InterviewRequirement(InterviewRequirementKind kind, string evidenceId, int observationIndex = -1)
        { Kind = kind; EvidenceId = evidenceId; ObservationIndex = observationIndex; }
    }
    // Authored data only. No default story catalog is provided by the mechanism.
    public sealed class InterviewTopicDefinition
    {
        public string Id { get; }
        public string ParticipantId { get; }
        public string Question { get; }
        public string Answer { get; }
        readonly InterviewRequirement[] requirements;
        public InterviewRequirement[] Requirements => (InterviewRequirement[])requirements.Clone();
        public InterviewTopicDefinition(string id, string participantId, string question, string answer, params InterviewRequirement[] requirements)
        {
            if (string.IsNullOrWhiteSpace(id) || id != id.Trim() || string.IsNullOrWhiteSpace(participantId) || participantId != participantId.Trim() || string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(answer))
                throw new ArgumentException("Topic data needs stable nonblank IDs, question and answer");
            Id = id; ParticipantId = participantId; Question = question; Answer = answer;
            this.requirements = requirements == null ? Array.Empty<InterviewRequirement>() : requirements.ToArray();
        }
    }
    public enum InterviewTopicAvailability { Available, Locked, UnknownTopic, NoConversation, ContextExpired, Paused, InvalidCondition, SessionUnavailable }
    public readonly struct InterviewTopicState
    {
        public readonly string Id, Question, Reason;
        public readonly InterviewTopicAvailability Availability;
        public bool CanAsk => Availability == InterviewTopicAvailability.Available;
        // Never contains an answer. Locked/inactive questions are also withheld.
        internal InterviewTopicState(string id, string question, InterviewTopicAvailability availability, string reason)
        { Id = id; Question = question; Availability = availability; Reason = reason; }
    }
    public readonly struct InterviewReply
    {
        public readonly string TopicId, ParticipantId, Question, Answer;
        internal InterviewReply(InterviewTopicDefinition topic)
        { TopicId = topic.Id; ParticipantId = topic.ParticipantId; Question = topic.Question; Answer = topic.Answer; }
    }
}
