namespace GTAI.TaskSystem
{
	public class UtilitySelector : CompositeTask
	{
		private Task _currentTask;

		#region Overridden NPCTask Methods

		public override float GetUtility()
		{
			if (_currentTask != null)
			{
				return _currentTask.GetUtility();
			}

			return 0f;
		}
		
		public override void OnDrawGizmos()
		{
			_currentTask?.OnDrawGizmos();
		}

		public override void OnDrawGizmosSelected()
		{
			_currentTask?.OnDrawGizmosSelected();
		}

		public override TaskStatus OnUpdate()
		{
			PickCurrentTask();

			if (_currentTask != null)
			{
				return _currentTask.OnUpdate();
			}

			return TaskStatus.Success;
		}

		#endregion

		private void SetCurrentTask(Task task)
		{
			if (_currentTask == task)
			{
				return;
			}

			_currentTask?.OnExit();
			_currentTask = task;
			_currentTask?.OnEntry();
		}

		private void PickCurrentTask()
		{
			Task bestTask = SelectTaskByUtility(tasks);
			SetCurrentTask(bestTask);
		}
	}
}