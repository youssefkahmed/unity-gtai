using UnityEngine;

namespace GTAI.TaskSystem
{
	public class Sequence : CompositeTask
	{
		private int _currentTaskIndex;
		private Task CurrentTask
		{
			get
			{
				try
				{
					return tasks[_currentTaskIndex];
				}
				catch
				{
					Debug.LogError($"[{nameof(Sequence)}] Current task index {_currentTaskIndex} out of bounds, task count = {tasks.Count}");

					return null;
				}
			}
		}

		#region Overridden NPCTask Methods

		public override float GetUtility()
		{
			var maxUtility = 0f;
			foreach (Task task in tasks)
			{
				float utility = task.GetUtility();
				if (utility > maxUtility)
				{
					maxUtility = utility;
				}
			}

			return maxUtility;
		}
		
		public override void OnDrawGizmos()
		{
			if (IsCurrentTaskIndexValid())
			{
				CurrentTask.OnDrawGizmos();
			}
		}

		public override void OnDrawGizmosSelected()
		{
			if (IsCurrentTaskIndexValid())
			{
				CurrentTask.OnDrawGizmosSelected();
			}
		}
		
		public override void OnEntry()
		{
			_currentTaskIndex = 0;

			CurrentTask?.OnEntry();
		}

		public override TaskStatus OnUpdate()
		{
			if (tasks.Count <= 0)
			{
				return TaskStatus.Failure;
			}

			TaskStatus status = CurrentTask.OnUpdate();
			if (status == TaskStatus.Success)
			{
				Task previousTask = CurrentTask;

				_currentTaskIndex++;

				if (_currentTaskIndex >= tasks.Count)
				{
					previousTask.OnExit();
					return TaskStatus.Success;
				}
				
				previousTask.OnExit();
				CurrentTask.OnEntry();
			}
			else if (status == TaskStatus.Failure)
			{
				CurrentTask.OnExit();

				return TaskStatus.Failure;
			}

			return TaskStatus.Running;
		}

		#endregion

		private bool IsCurrentTaskIndexValid()
		{
			return _currentTaskIndex >= 0 && _currentTaskIndex < tasks.Count;
		}
	}
}