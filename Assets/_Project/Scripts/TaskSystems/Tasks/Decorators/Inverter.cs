namespace GTAI.TaskSystem.Decorators
{
	public class Inverter : Decorator
	{
		public Inverter(Task childTask) : base(childTask) { }

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
				return status == TaskStatus.Success ? TaskStatus.Failure : TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}