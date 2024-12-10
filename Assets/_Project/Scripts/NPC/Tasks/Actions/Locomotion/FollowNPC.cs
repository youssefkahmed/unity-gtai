using GTAI.TaskSystem;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.NPCTasks.Actions.Locomotion
{
	public class FollowNPC : NPCAction
	{
		public SharedNPC target = new();

		public SharedFloat maxDistance = 15f;
		public SharedFloat minDistance = 4f;

		protected override TaskStatus OnUpdate()
		{
			if (!target.Value)
			{
				return TaskStatus.Failure;
			}

			MaintainDistanceAndVisibility();

			return TaskStatus.Running;
		}

		private void MaintainDistanceAndVisibility()
		{
			float distance = npc.GetDistance(target.Value);

			// setting max movement speed
			float t = Mathf.InverseLerp(maxDistance.Value, maxDistance.Value * 1.5f, distance);

			npc.OnSetMaxSpeed(Mathf.Lerp(npc.WalkSpeed, npc.RunSpeed, t));

			if (distance <= maxDistance.Value)
			{
				npc.CanMove = false;
			}
			else
			{
				npc.CanMove = true;
				npc.OnSetDestination?.Invoke(target.Value.Position);
			}

			if (distance <= minDistance.Value)
			{
				GetAwayFrom(target.Value.Position);
			}
		}

		private void GetAwayFrom(Vector3 point)
		{
			var direction = (npc.Position - point).normalized;
			var targetPosition = direction * 5f + npc.Position;

			if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas))
			{
				targetPosition = hit.position;
			}

			npc.CanMove = true;
			npc.OnSetDestination?.Invoke(targetPosition);
		}
	}
}