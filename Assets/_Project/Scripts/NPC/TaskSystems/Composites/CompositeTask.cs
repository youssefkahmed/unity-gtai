using System.Collections.Generic;

namespace GTAI.TaskSystem
{
	public abstract class CompositeTask : Task
	{
		protected readonly List<Task> tasks = new();

		public void CreateTasks(params Task[] newTasks)
		{
			tasks.Clear();

			foreach (Task task in newTasks)
			{
				tasks.Add(task);
			}
		}
	}
}