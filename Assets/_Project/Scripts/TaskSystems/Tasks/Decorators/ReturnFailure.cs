namespace GTAI.TaskSystem.Decorators
{
	public class ReturnFailure : Decorator
	{
		public ReturnFailure(Task childTask) : base(childTask) { }

		#region Overridden Virtual Methods

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
				return TaskStatus.Failure;
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}