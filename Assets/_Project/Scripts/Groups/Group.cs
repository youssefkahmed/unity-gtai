using System;
using System.Collections.Generic;
using System.Linq;
using GTAI.Formations;
using GTAI.NPCs;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GTAI.Groups
{
    public class Group : MonoBehaviour
    {
        public float StragglerDistance => stragglerDistance;
        
        [field: Header("Components:")]
        [field: SerializeField] public List<NPC> Members { get; private set; } = new();
        [SerializeField] private Formation formation;
        
        [Header("Debug & Validation:")]
        [SerializeField] private bool drawGizmos;
        
        [Header("Straggler Settings:")]
        [Tooltip("Leader will wait if any member has a distance to formation superior to this value.")]
        [SerializeField] private float stragglerDistance = 10f;

        #region Debug / Validation

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stragglerDistance);
        }

        private void OnDrawGizmos()
        {
            if (formation == null || MemberCount <= 0 || !drawGizmos)
            {
                return;
            }

            Color followerColor = new(0.4f, 1f, 0.375f);
            Color stragglerColor = new(1f, 0.5f, 0.2f);
            Color leaderColor = new(0.118f, 0.529f, 1f);

#if UNITY_EDITOR
            GUIStyle followerStyle = new()
            {
                normal = { textColor = followerColor },
                fontSize = 14
            };

            GUIStyle stragglerStyle = new()
            {
                normal = { textColor = stragglerColor },
                fontSize = 14
            };

            GUIStyle leaderStyle = new()
            {
                normal = { textColor = leaderColor },
                fontSize = 14
            };
            
            foreach (NPC member in Members)
            {
                Vector3 labelPosition = member.Position + Vector3.up * 2f;

                if (IsLeader(member))
                {
                    Handles.Label(labelPosition, "Leader", leaderStyle);
                }
                else
                {
                    float dist = Mathf.Round(GetDistanceToFormation(member) * 100) / 100f;
                    Handles.Label(labelPosition, $"Dist: {dist}", IsStraggler(member) ? stragglerStyle : followerStyle);
                }
            }
#endif // UNITY_EDITOR
            
            foreach (NPC member in Members)
            {
                Vector3 positionInGroup = GetPositionInGroup(member);

                Gizmos.color = IsLeader(member) ? leaderColor : IsStraggler(member) ? stragglerColor : followerColor;
                Gizmos.DrawSphere(positionInGroup, 0.15f);
                
                // Drawing movement direction
                Gizmos.color = Color.cyan;
                Vector3 directionEnd = member.Position + Vector3.up * 0.2f + member.Direction;
                
                Gizmos.DrawLine(member.Position + Vector3.up * 0.2f, directionEnd);
                Gizmos.DrawSphere(directionEnd, 0.05f);
                
                // Drawing line to position in group
                Gizmos.color = IsStraggler(member) ? stragglerColor : followerColor;
                Gizmos.DrawLine(member.Position, positionInGroup);
            }
        }

        #endregion
        
        #region Unity Event Methods

        private void Start()
        {
            foreach (NPC member in Members)
            {
                member.Group = this;
            }
        }

        private void Update()
        {
            for (int i = Members.Count - 1; i >= 0; i--)
            {
                if (Members[i].IsAlive) continue;
                
                Members.RemoveAt(i);
            }
        }

        #endregion
        
        #region Public Methods

        public int MemberCount => Members.Count;
        public int FollowerCount => Math.Max(0, Members.Count - 1);

        public bool IsLeader(NPC npc)
        {
            return Members.IndexOf(npc) == 0;
        }
        
        public NPC GetLeader()
        {
            if (Members.Count > 0)
            {
                return Members[0];
            }
            return null;
        }

        public int GetFollowerIndex(NPC npc)
        {
            // Not including the Leader
            return Members.IndexOf(npc) - 1;
        }
        
        public Vector3 GetPositionInGroup(NPC npc)
        {
            return formation.GetPosition(npc, this);
        }

        public float GetDistanceToFormation(NPC npc)
        {
            Vector3 formationPosition = GetPositionInGroup(npc);
            return Vector3.Distance(formationPosition, npc.Position);
        }

        public bool IsStraggler(NPC npc)
        {
            return GetDistanceToFormation(npc) >= stragglerDistance;
        }

        public bool HasStragglers()
        {
            return Members.Any(IsStraggler);
        }
        
        public void AddMember(NPC member)
        {
            Members.Add(member);
            member.Group = this;
        }
        
        public void RemoveMember(NPC member)
        {
            Members.Remove(member);
            member.Group = null;
        }

        public void IncreaseRank(NPC npc)
        {
            int index = Members.IndexOf(npc);
            if (index > 0)
            {
                // Swap NPC with the one before it
                NPC temp = Members[index - 1];
                Members[index - 1] = npc;
                Members[index] = temp;
            }
        }

        public void DecreaseRank(NPC npc)
        {
            int index = Members.IndexOf(npc);
            if (index >= 0 && index < Members.Count - 1)
            {
                // Swap NPC with the one after it
                NPC temp = Members[index + 1];
                Members[index + 1] = npc;
                Members[index] = temp;
            }
        }
        
        public void SetFormation(Formation newFormation)
        {
            formation = newFormation;
        }
        
        #endregion
    }
}