using GTAI.NPCs;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.Tasks
{
	public class FollowGroup : NPCTask
	{
		public FollowGroup(NPC npc) : base(npc) { }

		#region Overridden NPCTask Methods

		public override float GetUtility()
		{
			if (!npc.HasGroup())
			{
				return 0f;
			}

			if (npc.IsGroupLeader() && !npc.GroupHasStragglers())
			{
				return 0f;
			}

			return 2f;
		}

		protected override TaskStatus OnUpdate()
		{
			UpdateSpeed();

			if (npc.Group.IsLeader(npc))
			{
				UpdateLeader();
			}
			else
			{
				UpdateFollower();
			}

			return TaskStatus.Running;
		}

		#endregion
		
		private void UpdateSpeed()
		{
			if (npc.Group == null)
			{
				return;
			}

			if (npc.Group.IsLeader(npc))
			{
				npc.OnSetMaxSpeed?.Invoke(npc.WalkSpeed);
				return;
			}

			// Run to keep up with leader
			float catchUpDistance = npc.Group.StragglerDistance * 0.5f;
			float distToFormation = npc.Group.GetDistanceToFormation(npc);
			float speedBoost = Mathf.InverseLerp(0.5f, catchUpDistance, distToFormation);

			// If leader is running, we set the speed to run speed immediately
			NPC leader = npc.Group.GetLeader();

			if (Mathf.Abs(leader.Velocity.magnitude - leader.RunSpeed) <= 0.5f)
			{
				speedBoost = 1f;
			}

			float speed = Mathf.Lerp(npc.WalkSpeed, npc.RunSpeed, speedBoost);

			npc.OnSetMaxSpeed?.Invoke(speed);
		}
		
		private void UpdateLeader()
		{
			npc.CanMove = true;

			// Wait for stragglers
			if (npc.Group.HasStragglers())
			{
				npc.CanMove = false;
			}
		}
		
		private void UpdateFollower()
		{
			// Using CanMove instead of setting the destination allows NavMeshAgents to slow down when arriving.
			if (npc.Group.GetDistanceToFormation(npc) <= 0.5f)
			{
				npc.CanMove = false;
			}
			else
			{
				npc.CanMove = true;
				npc.OnSetDestination?.Invoke(npc.Group.GetPositionInGroup(npc));
			}
		}
	}
}