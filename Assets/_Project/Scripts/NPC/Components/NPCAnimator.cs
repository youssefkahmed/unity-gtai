using UnityEngine;

namespace GTAI.NPCs.Component
{
    public class NPCAnimator : NPCComponent
    {
        [SerializeField] private AudioClip[] footstepClips;
        
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int StrafeSpeedHash = Animator.StringToHash("StrafeSpeed");

        private void Update()
        {
            float forwardSpeed = Vector3.Dot(npc.Velocity, npc.transform.forward);
            float strafeSpeed = Vector3.Dot(npc.Velocity, npc.transform.right);
            
            npc.Animator.SetFloat(SpeedHash, forwardSpeed);
            npc.Animator.SetFloat(StrafeSpeedHash, strafeSpeed);
        }

        #region Animation Events

        public void OnFootstep()
        {
            if (!npc.AudioSource && !npc.AudioSource.isPlaying)
            {
                npc.AudioSource.clip = footstepClips[Random.Range(0, footstepClips.Length)];
                npc.AudioSource.Play();
            }
        }

        #endregion
    }
}