using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCTasks.Actions.Locomotion
{
	public class GoTo : NPCAction
	{
		public SharedVector3 destination;

		protected override void OnEntry()
		{
			npc.CanMove = true;
		}

		protected override void OnExit()
		{
			npc.CanMove = false;
		}

		protected override TaskStatus OnUpdate()
		{
			npc.OnSetDestination?.Invoke(destination.Value);

			if (!npc.HasPathToDestination())
			{
				return TaskStatus.Failure;
			}

			if (HasArrived())
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		private bool HasArrived()
		{
			return Vector3.Distance(npc.Position, destination.Value) <= 1f;
		}
	}
}