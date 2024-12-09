namespace GTAI.TaskSystem
{
	public class Selector : Sequence
	{
		protected override TaskStatus OnUpdate()
		{
			if (tasks.Count <= 0)
			{
				return TaskStatus.Failure;
			}

			TaskStatus status = Update(CurrentTask);
			if (status == TaskStatus.Success)
			{
				Stop(CurrentTask);
				return TaskStatus.Success;
			}
			
			if (status == TaskStatus.Failure)
			{
				currentTaskIndex++;

				if (currentTaskIndex >= tasks.Count)
				{
					return TaskStatus.Success;
				}
				
				Start(CurrentTask);
			}

			return TaskStatus.Running;
		}
	}
}