using GTAI.Groups;
using GTAI.NPCs;
using UnityEngine;

namespace GTAI.Formations
{
    [CreateAssetMenu(menuName = "GTAI/Formations/Circle Formation", fileName = "New Circle Formation")]
    public class CircleFormation : Formation
    {
        [SerializeField] private float radius = 3f;
        [SerializeField] private bool doFollowLeaderDirection;
        
        public override Vector3 GetPosition(NPC npc, Group group)
        {
            if (group.IsLeader(npc))
            {
                return npc.Position;
            }
            
            NPC leader = group.GetLeader();
            float angle = group.GetFollowerIndex(npc) * 360f / group.FollowerCount;
            Vector3 forward = doFollowLeaderDirection ? -leader.Direction : Vector3.forward;
            Vector3 position = leader.Position + Quaternion.Euler(0f, angle, 0f) * forward * radius;
            
            return AdjustPosition(position, leader.Position);
        }
    }
}