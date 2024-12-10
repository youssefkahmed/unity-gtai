using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Sensors
{
	public class GetClosestVisibleHostile : NPCAction
	{
		public SharedNPC target = new();

		protected override TaskStatus OnUpdate()
		{
			if (sensor.VisibleHostilesCount <= 0)
			{
				return TaskStatus.Failure;
			}

			foreach (var entry in sensor.detectionList.List)
			{
				if (entry.isHostile && entry.isVisible)
				{
					target.Value = entry.npc;
					return TaskStatus.Success;
				}
			}

			return TaskStatus.Failure;
		}
	}
}