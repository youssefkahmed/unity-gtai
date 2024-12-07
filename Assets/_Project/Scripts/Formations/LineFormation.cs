using GTAI.Groups;
using GTAI.NPCs;
using UnityEngine;

namespace GTAI.Formations
{
    [CreateAssetMenu(menuName = "GTAI/Formations/Line Formation", fileName = "New Line Formation")]
    public class LineFormation : Formation
    {
        [SerializeField] private float spacing = 3f;
        
        public override Vector3 GetPosition(NPC npc, Group group)
        {
            if (group.IsLeader(npc))
            {
                return npc.Position;
            }
            
            NPC leader = group.GetLeader();
            
            // Calculate the right vector, which is perpendicular to the leader's direction
            Vector3 leaderRight = Vector3.Cross(Vector3.up, leader.Direction).normalized;
            float formationWidth = (group.FollowerCount - 1) * spacing;
            
            Vector3 position = leader.Position - leader.Direction * spacing; // Right behind the Leader
            position -= formationWidth * 0.5f * leaderRight; // Half formation width x Left of the Leader
            position += spacing * group.GetFollowerIndex(npc) * leaderRight;
            
            return AdjustPosition(position, leader.Position);
        }
    }
}