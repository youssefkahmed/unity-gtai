using GTAI.TaskSystem;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.NPCTasks.Actions.Combat
{
	public class EngageMainTarget : NPCAction
	{
		public SharedNPC mainTarget = new();
		
		#region Overridden Virtual Methods

		protected override void OnEntry()
		{
			npc.OnSetAim?.Invoke(true);
			npc.CanMove = false;
		}

		protected override void OnExit()
		{
			npc.OnSetAim?.Invoke(false);
		}
		
		protected override TaskStatus OnUpdate()
		{
			MaintainDistanceAndVisibility();

			// TODO shoot

			return TaskStatus.Running;
		}
		
		public override float GetUtility()
		{
			// The below line will not work, and will result in the "engage" branch always failing
			// that's because MainTarget is only updated inside the SelectMainTarget action,
			// this creates a catch 22 situation: can't engage because I don't have a main target,
			// and main target will always be null because I need to be in the engage branch to update it.
			// Instead, we check if there are visible targets.
			if (sensor.VisibleHostilesCount > 0 || mainTarget.Value)
			{
				return 10f;
			}

			return 0f;
		}

		#endregion

		private void MaintainDistanceAndVisibility()
		{
			if (!mainTarget.Value)
			{
				return;
			}

			float distance = npc.GetDistance(mainTarget.Value);

			const float maxDistance = 15f;
			const float minDistance = 4f;

			npc.OnLookAt?.Invoke(mainTarget.Value.SensorPosition);

			if (sensor.IsVisible(mainTarget.Value) && distance <= maxDistance)
			{
				npc.CanMove = false;
			}
			else
			{
				npc.CanMove = true;
				npc.OnSetDestination?.Invoke(mainTarget.Value.Position);
			}

			if (distance <= minDistance)
			{
				GetAwayFrom(mainTarget.Value.Position);
			}
		}

		private void GetAwayFrom(Vector3 point)
		{
			Vector3 direction = (npc.Position - point).normalized;
			Vector3 targetPosition = direction * 5f + npc.Position;

			if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas))
			{
				targetPosition = hit.position;
			}

			npc.CanMove = true;
			npc.OnSetDestination(targetPosition);
		}
	}
}