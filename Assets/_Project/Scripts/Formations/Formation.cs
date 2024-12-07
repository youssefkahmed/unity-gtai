using GTAI.Groups;
using GTAI.NPCs;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.Formations
{
    public abstract class Formation : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        
        protected static Vector3 AdjustPosition(Vector3 position, Vector3 leaderPosition)
        {
            Vector3 targetPosition = position;
            if (NavMesh.Raycast(leaderPosition, position, out NavMeshHit hit, NavMesh.AllAreas))
            {
                targetPosition = hit.position;
            }
            
            if (NavMesh.SamplePosition(targetPosition, out hit, 3f, NavMesh.AllAreas))
            {
                targetPosition = hit.position;
            }
            
            return targetPosition;
        }
        
        public abstract Vector3 GetPosition(NPC npc, Group group);
    }
}