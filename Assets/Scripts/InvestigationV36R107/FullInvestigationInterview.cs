using System;
using System.Linq;

namespace ConRoi.Investigation.V36.R107
{
    // The existing interview mechanism needs only evidence/observation queries and a
    // load-context identity. This boundary supports full-case IDs without changing r107 IDs.
    public interface IInvestigationInterviewSource
    {
        bool Available {get;} bool Paused {get;} object DataContext {get;}
        bool IsValidInterviewRequirement(InterviewRequirement requirement);
        bool MeetsInterviewRequirement(InterviewRequirement requirement);
    }
    internal sealed class SessionInterviewSource : IInvestigationInterviewSource
    {
        readonly InvestigationSession session;
        internal SessionInterviewSource(InvestigationSession source){if(!source)throw new ArgumentNullException(nameof(source));session=source;}
        public bool Available=>session&&session.data!=null;
        public bool Paused=>Available&&session.Paused;
        public object DataContext=>Available?session.data:null;
        public bool IsValidInterviewRequirement(InterviewRequirement r)
        {
            var d=InvestigationContent.Get(r.EvidenceId);return d!=null&&(r.Kind==InterviewRequirementKind.AcquiredEvidence?r.ObservationIndex==-1:
                r.Kind==InterviewRequirementKind.RecordedObservation&&d.observations!=null&&r.ObservationIndex>=0&&r.ObservationIndex<d.observations.Length);
        }
        public bool MeetsInterviewRequirement(InterviewRequirement r)=>Available&&(r.Kind==InterviewRequirementKind.AcquiredEvidence?
            session.data.evidence!=null&&session.data.evidence.Any(e=>e!=null&&e.id==r.EvidenceId):session.TryGetRecordedNote(r.EvidenceId,r.ObservationIndex,out _,out _));
    }
    public sealed partial class FullInvestigationRuntime : IInvestigationInterviewSource
    {
        public bool Available=>true;
        public bool IsValidInterviewRequirement(InterviewRequirement r)=>r.Kind==InterviewRequirementKind.AcquiredEvidence?
            Evidence(r.EvidenceId)!=null&&r.ObservationIndex==-1:r.Kind==InterviewRequirementKind.RecordedObservation&&IsKnownObservation(r.EvidenceId,r.ObservationIndex);
        public bool MeetsInterviewRequirement(InterviewRequirement r)=>r.Kind==InterviewRequirementKind.AcquiredEvidence?HasEvidence(r.EvidenceId):HasRecordedObservation(r.EvidenceId,r.ObservationIndex);
    }
}
