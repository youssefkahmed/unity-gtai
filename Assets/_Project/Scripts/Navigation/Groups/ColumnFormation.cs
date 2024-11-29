using GTAI.Navigation.NPCs;
using UnityEngine;

namespace GTAI.Navigation.Groups
{
    [CreateAssetMenu(menuName = "GTAI/Formations/Column Formation", fileName = "New Column Formation")]
    public class ColumnFormation : Formation
    {
        [SerializeField] private float spacing = 3f;
        
        public override Vector3 GetPosition(NPC npc, Group group)
        {
            if (group.IsLeader(npc))
            {
                return npc.Position;
            }

            NPC leader = group.GetLeader();
            Vector3 position = leader.Position - spacing * group.Members.IndexOf(npc) * leader.Direction;

            return AdjustPosition(position, leader.Position);
        }
    }
}