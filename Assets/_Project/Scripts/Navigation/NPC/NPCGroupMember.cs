using UnityEngine;

namespace GTAI.Navigation.NPCs
{
    public class NPCGroupMember : NPCComponent
    {
        private void Update()
        {
            if (!npc.Group)
            {
                npc.Wander = true;
                return;
            }

            if (npc.Group.IsLeader(npc))
            {
                npc.Wander = true;
            }
            else
            {
                Vector3 position = npc.Group.GetPositionInGroup(npc);
                npc.NavMeshAgent.SetDestination(position);
                npc.NavMeshAgent.isStopped = false;
                npc.Wander = false;
            }
        }
    }
}