using System;
using System.Collections.Generic;
using System.Linq;

namespace ConRoi.Investigation.V36.R107
{
    public enum FullRequirementKind { Collected, Recorded, Comparison, Decoded, Permission, Finalized, TrialRecorded, Inspected, Heard, ReplySent, SourcedObservation }
    public enum FullCaseModal { None, Inspect, Cipher, Comparison, Conversation, Reconstruction, Conclusions, Media, Ending }
    public enum FullEnding { None, Complete, Open, Unsupported }
    [Serializable] public sealed class FullRequirement
    {
        public FullRequirementKind kind; public string id; public int index = -1;
        public FullRequirement(FullRequirementKind kind,string id,int index=-1){this.kind=kind;this.id=id;this.index=index;}
    }
    [Serializable] public sealed class FullLocationDefinition
    {
        public string id,label; public FullRequirement[] requires=new FullRequirement[0];
    }
    [Serializable] public sealed class FullEvidenceDefinition
    {
        public string id,location,label,source,text; public string[] observations=new string[0];
        public FullRequirement[] requires=new FullRequirement[0];
        public bool portable; public string[] placementSlots=new string[0];
        public string kind="Unclassified"; // Authored provenance, never inferred from text.
    }
    [Serializable] public sealed class FullComparisonDefinition
    {
        public string id,label; public string[] evidenceIds=new string[0];
        public FullRequirement[] requires=new FullRequirement[0];
        public string[] options=new string[0]; public string expectedOption;
    }
    [Serializable] public sealed class FullPermissionDefinition
    { public string id,label,location; public FullRequirement[] requires=new FullRequirement[0]; }
    [Serializable] public sealed class FullSupportSet
    { public FullRequirement[] requires=new FullRequirement[0]; }
    [Serializable] public sealed class FullConclusionRule
    {
        public string id,label,correctClaim; public string[] claims=new string[0];
        public FullSupportSet[] alternatives=new FullSupportSet[0];
        public bool reviewed; // Must represent an actual content review, not player progress.
    }
    [Serializable] public sealed class FullCaseCatalog
    {
        public string id,revision,startLocation; public bool testOnly;
        public FullLocationDefinition[] locations=new FullLocationDefinition[0];
        public FullEvidenceDefinition[] evidence=new FullEvidenceDefinition[0];
        public FullComparisonDefinition[] comparisons=new FullComparisonDefinition[0];
        public FullPermissionDefinition[] permissions=new FullPermissionDefinition[0];
        public FullConclusionRule[] conclusions=new FullConclusionRule[0];
        public FullRequirement[] replyChannelRequirements=new FullRequirement[0];
        public bool fallChainReviewed;
    }
    [Serializable] public sealed class FullEvidenceRecord
    {
        public string id; public bool viewed; public List<int> observations=new List<int>();
        public string placement="",slot="";
    }
    [Serializable] public sealed class FullConclusionDraft
    { public string id,claim=""; public List<FullRequirement> grounds=new List<FullRequirement>(); }
    [Serializable] public sealed class FullComparisonAttempt
    { public string id,option; public string[] evidenceIds=new string[0]; }
    [Serializable] public sealed class FullCaseSave
    {
        public int schema=2; public string namespaceId="conroi.investigation.v36.fullcase",catalogHash,location,held="";
        public List<FullEvidenceRecord> evidence=new List<FullEvidenceRecord>();
        public List<CharacterAssignment> mapping=new List<CharacterAssignment>();
        public List<string> decoded=new List<string>(),permissions=new List<string>();
        public List<FullComparisonAttempt> comparisons=new List<FullComparisonAttempt>();
        public List<EvidenceRelation> relations=new List<EvidenceRelation>();
        public List<TrialRecord> trials=new List<TrialRecord>();
        public List<FullConclusionDraft> conclusions=new List<FullConclusionDraft>();
        public bool replySent,finalized,clipBeforeSeen,clipEventSeen; public float clipBeforeExposure,clipEventExposure;
        public FullEnding ending; public List<string> unsupportedDimensions=new List<string>();
        public List<FullHeardStatement> heardStatements=new List<FullHeardStatement>();
        public List<string> recordedStatements=new List<string>(),sourcedNotes=new List<string>();
        public bool endingViewed,replyProducerVerified;
        public GateState gate=new GateState();
    }
    public readonly struct FullActionResult
    {
        public readonly bool Success; public readonly string Code,Message;
        internal FullActionResult(bool success,string code,string message){Success=success;Code=code;Message=message;}
        public static FullActionResult Ok(string message="Đã thực hiện.")=>new FullActionResult(true,"ok",message);
        public static FullActionResult Fail(string code,string message)=>new FullActionResult(false,code,message);
    }
    // Derived from a successfully recorded letter ID; never stored in either save schema.
    // This is a transcription, not a provenance classification or proof of its claims.
    public readonly struct FullRecordedReading
    {
        public readonly string Id,Recipient,Text,Meaning;
        internal FullRecordedReading(string id,string recipient,string text,string meaning){Id=id;Recipient=recipient;Text=text;Meaning=meaning;}
    }
    public readonly struct FullLetterView
    {
        public readonly string Id,Recipient,Reading; public readonly bool Recorded;
        internal FullLetterView(string id,string recipient,string reading,bool recorded){Id=id;Recipient=recipient;Reading=reading;Recorded=recorded;}
    }
    public readonly struct FullEndingView
    {
        public readonly FullEnding Ending; public readonly string[] UnsupportedDimensions;
        // No criminal charge or automatic arrest derived from a motive/sender error.
        public bool FallAccusationUnsupported=>UnsupportedDimensions!=null&&UnsupportedDimensions.Contains("fall");
        internal FullEndingView(FullEnding ending,string[] dimensions){Ending=ending;UnsupportedDimensions=dimensions;}
    }
}
