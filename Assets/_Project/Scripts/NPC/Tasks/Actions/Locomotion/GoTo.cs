using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCTasks.Actions.Locomotion
{
	public class GoTo : NPCAction
	{
		public SharedVector3 destination;
		public SharedFloat arriveDistance = 1f;
		public SharedBool run = false;
		
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

			npc.OnSetMaxSpeed?.Invoke(run.Value ? npc.RunSpeed : npc.WalkSpeed);
			if (npc.HasPathToDestination() == false)
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
			return Vector3.Distance(npc.Position, destination.Value) <= arriveDistance.Value;
		}
	}
}