namespace GTAI.TaskSystem
{
	public class Repeater : Task
	{
		private readonly Task _childTask;

		public Repeater(Task childTask)
		{
			_childTask = childTask;
		}

		#region Overridden NPCTask Methods

		public override void OnEntry()
		{
			_childTask.OnEntry();
		}

		public override TaskStatus OnUpdate()
		{
			if (_childTask == null)
			{
				return TaskStatus.Failure;
			}

			TaskStatus status = _childTask.OnUpdate();
			if (status is TaskStatus.Success or TaskStatus.Failure)
			{
				_childTask.OnExit();
				_childTask.OnEntry();
			}

			return TaskStatus.Running;

		}

		#endregion
	}
}