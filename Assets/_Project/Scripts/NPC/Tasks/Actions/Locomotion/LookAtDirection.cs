using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCTasks.Actions.Locomotion
{
	public class LookAtDirection : NPCAction
	{
		public SharedVector3 direction;

		protected override TaskStatus OnUpdate()
		{
			Vector3 targetPosition = npc.SensorPosition + direction.Value * 100.0f;

			npc.OnLookAt?.Invoke(targetPosition);
			if (npc.IsLookingAt(targetPosition))
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}
	}
}