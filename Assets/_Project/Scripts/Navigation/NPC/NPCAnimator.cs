using UnityEngine;

namespace GTAI.Navigation.NPCs
{
    public class NPCAnimator : NPCComponent
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private void Update()
        {
            npc.Animator.SetFloat(SpeedHash, npc.CurrentSpeed);
        }
    }
}