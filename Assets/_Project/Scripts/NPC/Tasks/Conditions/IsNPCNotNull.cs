using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Conditions
{
	public class IsNPCNotNull : Condition
	{
		public readonly SharedNPC target = new();

		public IsNPCNotNull() { }

		public IsNPCNotNull(SharedNPC target)
		{
			this.target = target;
		}

		protected override TaskStatus OnUpdate()
		{
			if (target == null || !target.Value)
			{
				return TaskStatus.Failure;
			}

			return TaskStatus.Success;
		}
	}

}