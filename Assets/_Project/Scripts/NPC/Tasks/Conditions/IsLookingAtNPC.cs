using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Conditions
{
	public class IsLookingAtNPC : NPCCondition
	{
		public readonly SharedNPC target = new();

		public IsLookingAtNPC() { }

		public IsLookingAtNPC(SharedNPC target)
		{
			this.target = target;
		}

		protected override TaskStatus OnUpdate()
		{
			if (target == null || !target.Value)
			{
				return TaskStatus.Failure;
			}

			return npc.IsLookingAt(target.Value.SensorPosition) ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}