using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Conditions.Sensors
{
	public class CanSeeNPC : NPCCondition
	{
		public SharedNPC target = new();
		
		public CanSeeNPC() { }

		public CanSeeNPC(SharedNPC target)
		{
			this.target = target;
		}

		protected override TaskStatus OnUpdate()
		{
			if (target == null || !target.Value || !sensor)
			{
				return TaskStatus.Failure;
			}

			return sensor.IsVisible(target.Value) ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}