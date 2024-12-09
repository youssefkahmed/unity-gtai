namespace GTAI.TaskSystem
{
	public class Interrupt : Action
	{
		public readonly Task task; // task to interrupt

		public Interrupt() { }

		public Interrupt(Task task)
		{
			this.task = task;
		}

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			if (task.Active)
			{
				Stop(task);
			}
			return TaskStatus.Success;
		}

		#endregion
	}
}