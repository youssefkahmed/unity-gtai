using UnityEngine;

namespace GTAI.NPCs
{
    public class NPCComponent : MonoBehaviour
    {
        protected NPC npc;

        protected virtual void Awake()
        {
            SetNPC(GetComponentInParent<NPC>());
        }

        public virtual void SetNPC(NPC newNpc)
        {
            npc = newNpc;
        }
    }
}