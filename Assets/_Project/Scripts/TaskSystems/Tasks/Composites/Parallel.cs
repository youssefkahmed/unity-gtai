namespace GTAI.TaskSystem.Composites
{
	public class Parallel : Composite
	{
		#region Overridden Virtual Methods

		public override void OnDrawGizmos()
		{
			foreach (Task task in tasks)
			{
				if (task.Active)
				{
					task.OnDrawGizmos();
				}
			}
		}
		
		public override void OnDrawGizmosSelected()
		{
			foreach (Task task in tasks)
			{
				if (task.Active)
				{
					task.OnDrawGizmosSelected();
				}
			}
		}

		protected override void OnEntry()
		{
			foreach (Task task in tasks)
			{
				Start(task);
			}
		}

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
					return TaskStatus.Failure;
				}
				
				if (status == TaskStatus.Success)
				{
					Stop(task);
				}
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}