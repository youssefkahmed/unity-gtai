using GTAI.Sensors;
using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Combat
{
	public class Approach : NPCAction
	{
		public SharedVariable<DetectionEntry> entry;
		public readonly float distance = 5f;

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			if (entry == null)
			{
				return TaskStatus.Failure;
			}

			npc.OnSetDestination?.Invoke(entry.Value.lastKnownPosition);

			if (npc.HasPathToDestination() == false)
			{
				return TaskStatus.Failure;
			}

			if (entry.Value.isVisible)
			{
				npc.OnLookAt?.Invoke(entry.Value.npc.SensorPosition);

				if (npc.IsWithinDistance(entry.Value.npc, distance))
				{
					return TaskStatus.Success;
				}
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}