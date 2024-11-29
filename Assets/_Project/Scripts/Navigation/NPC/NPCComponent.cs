using UnityEngine;

namespace GTAI.Navigation.NPCs
{
    public class NPCComponent : MonoBehaviour
    {
        protected NPC npc;

        protected virtual void Awake()
        {
            npc = GetComponent<NPC>();
        }
    }
}