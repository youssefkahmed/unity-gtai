using System;
using System.Collections.Generic;
using GTAI.Navigation.NPCs;
using UnityEngine;

namespace GTAI.Navigation.Groups
{
    public class Group : MonoBehaviour
    {
        [field: SerializeField] public List<NPC> Members { get; private set; } = new();
        [SerializeField] private Formation formation;
        [SerializeField] private bool drawGizmos;

        #region Debug / Validation

        private void OnDrawGizmos()
        {
            if (formation == null || MemberCount <= 0 || !drawGizmos)
            {
                return;
            }

            foreach (NPC member in Members)
            {
                Vector3 position = GetPositionInGroup(member);

                Gizmos.color = IsLeader(member) ? Color.red : Color.green;
                Gizmos.DrawSphere(position, 0.5f);
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

        public int GetFollowerIndex(NPC npc)
        {
            // Not including the Leader
            return Members.IndexOf(npc) - 1;
        }
        
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

        public Vector3 GetPositionInGroup(NPC npc)
        {
            return formation.GetPosition(npc, this);
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

        public void SetFormation(Formation newFormation)
        {
            formation = newFormation;
        }
        
        #endregion
    }
}