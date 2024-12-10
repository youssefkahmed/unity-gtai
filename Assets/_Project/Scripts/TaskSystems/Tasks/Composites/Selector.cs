namespace GTAI.TaskSystem.Composites
{
	public class Selector : Sequence
	{
		public Selector() { }

		public Selector(params Task[] tasks) : base(tasks) { }
		
		protected override TaskStatus OnUpdate()
		{
			if (tasks.Count <= 0)
			{
				return TaskStatus.Failure;
			}

			for (var i = 0; i < currentTaskIndex; i++)
			{
				if (ShouldInterruptLowerPriority(tasks[i]))
				{
					Stop(CurrentTask);
					currentTaskIndex = i;
					Start(CurrentTask);

					break;
				}
			}
			
			TaskStatus status = Update(CurrentTask);
			if (status == TaskStatus.Success)
			{
				Stop(CurrentTask);
				return TaskStatus.Success;
			}
			
			if (status == TaskStatus.Failure)
			{
				Stop(CurrentTask);

				currentTaskIndex++;

				if (currentTaskIndex >= tasks.Count)
				{
					return TaskStatus.Success;
				}
				Start(CurrentTask);
			}

			return TaskStatus.Running;
		}
		
		private static bool ShouldInterruptLowerPriority(Task task)
		{
			if (task is Composite comp)
			{
				if (comp.interruptLowerPriority && comp.EvaluateConditions())
				{
					return true;
				}
			}

			return false;
		}
	}
}