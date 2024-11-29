using GTAI.Navigation.NPCs;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.Navigation.Groups
{
    public abstract class Formation : ScriptableObject
    {
        protected static Vector3 AdjustPosition(Vector3 position, Vector3 leaderPosition)
        {
            if (NavMesh.Raycast(leaderPosition, position, out NavMeshHit hit, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return position;
        }
        
        public abstract Vector3 GetPosition(NPC npc, Group group);
    }
}