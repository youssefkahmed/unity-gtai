using GTAI.NPCs;
using GTAI.NPCTasks;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.Tasks
{
	public class FollowGroup : NPCAction
	{
		#region Overridden Virtual Methods

		public override float GetUtility()
		{
			if (npc.HasGroup() == false)
			{
				return 0f;
			}

			if (npc.IsGroupLeader() && npc.GroupHasStragglers() == false)
			{
				return 0f;
			}

			float distanceToGroup = npc.Group.GetDistanceToFormation(npc);

			// We increase utility the more the distance to group is high
			float t = Mathf.InverseLerp(5f, 30f, distanceToGroup);

			float distanceToGroupUtility = Mathf.Lerp(0f, 3f, t);

			return 2f + distanceToGroupUtility;
		}

		protected override TaskStatus OnUpdate()
		{
			if (npc.HasGroup() == false)
			{
				return TaskStatus.Failure;
			}

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
			if (!npc.Group)
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