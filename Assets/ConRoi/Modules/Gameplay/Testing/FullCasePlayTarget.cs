#if UNITY_EDITOR
using UnityEngine;
namespace ConRoi.Gameplay.TestOnly
{
    // TEST ONLY: dynamically attached in the explicit Gameplay Play session, never a product prefab.
    public enum FullPlayAction { Evidence, Actor, Travel, Page, Appointment }
    public sealed class FullCasePlayTarget : MonoBehaviour
    {
        public string Key,StableId,LocationId,Label;
        public FullPlayAction Action;
        public ConRoi.Investigation.V36.R107.FullCaseModal Page;
    }
}
#endif
