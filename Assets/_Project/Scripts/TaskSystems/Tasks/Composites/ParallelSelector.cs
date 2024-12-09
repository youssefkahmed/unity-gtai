namespace GTAI.TaskSystem
{
	public class ParallelSelector : Parallel
	{
		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			if (tasks.Count <= 0)
			{
				return TaskStatus.Failure;
			}

			foreach (Task task in tasks)
			{
				if (task.Active == false)
				{
					continue;
				}

				TaskStatus status = Update(task);
				if (status == TaskStatus.Failure)
				{
					Stop(task);
				}
				else if (status == TaskStatus.Success)
				{
					Stop(task);
					return TaskStatus.Success;
				}
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}