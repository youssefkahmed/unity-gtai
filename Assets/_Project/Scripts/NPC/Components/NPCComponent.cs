using UnityEngine;

namespace GTAI.NPCs.Components
{
    public class NPCComponent : MonoBehaviour
    {
        protected NPC npc;

        protected virtual void Awake()
        {
            SetNPC(GetComponentInParent<NPC>());
        }

        protected virtual void SetNPC(NPC newNpc)
        {
            npc = newNpc;
        }
    }
}