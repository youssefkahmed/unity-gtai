using GTAI.Groups;
using GTAI.NPCs;
using UnityEngine;

namespace GTAI.Formations
{
    [CreateAssetMenu(menuName = "GTAI/Formations/Convoy Formation", fileName = "New Convoy Formation")]
    public class ConvoyFormation : Formation
    {
        [SerializeField] private float spacing = 3f;

        public override Vector3 GetPosition(NPC npc, Group group)
        {
            if (group.IsLeader(npc))
            {
                return npc.Position;
            }

            int memberIndex = group.Members.IndexOf(npc);
                
            // The "leader" in this case is the group member in front of this NPC
            NPC leader = group.Members[memberIndex - 1];
            float distanceToLeader = Vector3.Distance(npc.Position, leader.Position);
            if (distanceToLeader < spacing)
            {
                return npc.Position;
            }
            
            Vector3 directionFromLeader = (npc.Position - leader.Position).normalized;
            Vector3 targetPosition = leader.Position + directionFromLeader * spacing;
            return AdjustPosition(targetPosition, leader.Position);
        }
    }
}