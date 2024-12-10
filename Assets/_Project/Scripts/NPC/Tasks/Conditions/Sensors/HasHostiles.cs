using GTAI.NPCTasks;
using GTAI.TaskSystem;

namespace Game.TaskSystem
{
	public class HasHostiles : NPCCondition
	{
		protected override TaskStatus OnUpdate()
		{
			if (sensor.HostilesCount > 0)
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Failure;
		}
	}
}