using System.Collections.Generic;
using UnityEngine;

namespace GTAI.TaskSystem
{
	public class UtilitySelector : Composite
	{
		public Task CurrentTask { get; private set; }

		#region Overridden Virtual Methods

		protected override void OnEntry()
		{
			CurrentTask = null;
		}
		
		public override void OnDrawGizmos()
		{
			CurrentTask?.OnDrawGizmos();
		}

		public override void OnDrawGizmosSelected()
		{
			CurrentTask?.OnDrawGizmosSelected();
		}
		
		public override float GetUtility()
		{
			if (CurrentTask != null)
			{
				return CurrentTask.GetUtility();
			}

			return DefaultUtility;
		}

		protected override TaskStatus OnUpdate()
		{
			SelectTask();

			if (CurrentTask != null)
			{
				TaskStatus status = Update(CurrentTask);

				if (status == TaskStatus.Success)
				{
					Stop(CurrentTask);

					return TaskStatus.Success;
				}
				
				if (status == TaskStatus.Failure)
				{
					Stop(CurrentTask);
					CurrentTask = null;
				}
			}

			return TaskStatus.Running;
		}

		#endregion

		private void SelectTask()
		{
			Task bestTask = SelectTaskByUtility(tasks);

			if (bestTask == null)
			{
				Debug.LogError("Utility selector gets null when trying to select best task");
				return;
			}

			if (bestTask == CurrentTask)
			{
				return;
			}
			
			// Stopping previous task
			if (CurrentTask is { Active: true })
			{
				Stop(CurrentTask);
			}

			// Setting and starting new task
			CurrentTask = bestTask;

			Start(CurrentTask);
		}
		
		protected static Task SelectTaskByUtility(List<Task> allTasks)
		{
			if (allTasks == null || allTasks.Count == 0)
			{
				return null;
			}

			if (allTasks.Count == 1)
			{
				return allTasks[0];
			}

			Task bestTask = null;
			float highestUtility = float.MinValue;

			foreach (Task task in allTasks)
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
	}
}