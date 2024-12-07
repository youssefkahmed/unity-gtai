using System.Collections.Generic;

namespace GTAI.TaskSystem
{
	public enum TaskStatus
	{
		Success,
		Failure,
		Running
	}

	public class Task
	{
		public static Task SelectTaskByUtility(List<Task> tasks)
		{
			if (tasks == null || tasks.Count == 0)
			{
				return null;
			}

			Task bestTask = null;
			var highestUtility = float.MinValue;

			foreach (Task task in tasks)
			{
				float utility = task.GetUtility();
				if (utility > highestUtility)
				{
					highestUtility = utility;
					bestTask = task;
				}
			}

			return bestTask;
		}

		public virtual float GetUtility()
		{
			return 0f;
		}
		
		public virtual void OnDrawGizmos() { }

		public virtual void OnDrawGizmosSelected() { }
		
		public virtual void OnEntry() { }

		public virtual TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}
		
		public virtual void OnExit() { }
	}
}