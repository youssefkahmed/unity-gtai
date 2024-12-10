namespace GTAI.TaskSystem.Decorators
{
	public class ReturnSuccess : Decorator
	{
		public ReturnSuccess(Task childTask) : base(childTask) { }

		#region Overridden Virtual Method

		protected override TaskStatus OnUpdate()
		{
			if (child == null)
			{
				return TaskStatus.Failure;
			}

			var status = Update(child);
			if (status is TaskStatus.Success or TaskStatus.Failure)
			{
				Stop(child);
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}