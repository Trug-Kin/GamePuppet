using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ConRoi.Investigation.V36.R107
{
    public sealed partial class InvestigationSession
    {
        string conclusionConfirmation;
        public string BeforeConclusionSavePath => Path.Combine(Path.GetDirectoryName(SavePath),"prototype.before-conclusion.json");
        string ConclusionSignature() => string.Join("\n",data.conclusions.Select(c=>JsonUtility.ToJson(c)));
        public bool ConclusionsConfirmed => Page=="conclusions"&&!Paused&&conclusionConfirmation!=null&&conclusionConfirmation==ConclusionSignature();
        public void SetConclusionConfirmation(bool value)
        {
            conclusionConfirmation=value&&Page=="conclusions"&&!Paused?ConclusionSignature():null;
        }
        public bool LoadConclusionCheckpoint() => LoadFromPath(BeforeConclusionSavePath);
        public bool TryFinalizeConclusions()
        {
            if(!ConclusionsConfirmed){Message="Hãy xác nhận bản ghi hiện tại trước khi chốt.";return false;}
            try
            {
                // Stage without changing the live record. Both files use the existing writer.
                var pending=JsonUtility.FromJson<InvestigationSave>(JsonUtility.ToJson(data));
                pending.position=movement.transform.position;pending.yaw=movement.transform.eulerAngles.y;pending.pitch=eye.transform.localEulerAngles.x;
                pending.gate=JsonUtility.FromJson<GateState>(JsonUtility.ToJson(gate.state));pending.page=Page;pending.inspecting=inspectId;pending.finalized=false;
                InvestigationSaveFile.Write(BeforeConclusionSavePath,JsonUtility.ToJson(pending,true));
                pending.finalized=true;
                InvestigationSaveFile.Write(SavePath,JsonUtility.ToJson(pending,true));
                data.position=pending.position;data.yaw=pending.yaw;data.pitch=pending.pitch;data.gate=pending.gate;data.page=pending.page;data.inspecting=pending.inspecting;data.finalized=true;
                conclusionConfirmation=null;
                Message="Đã lưu bản ghi kết luận; bản trước chốt được giữ riêng.";
                return true;
            }
            catch(Exception)
            {
                Message="Không chốt hồ sơ: Không ghi được tệp. Bản lưu hợp lệ trước đó được giữ nguyên; hãy khắc phục rồi thử lại.";
                return false;
            }
        }
    }
}
