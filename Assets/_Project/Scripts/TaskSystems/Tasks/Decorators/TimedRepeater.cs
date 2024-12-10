using UnityEngine;

namespace GTAI.TaskSystem.Decorators
{
	public class TimedRepeater : Decorator
	{
		public float duration = 3f;
		public float durationRandom;

		private float _currentDuration = 3f;
		private float _timer;

		public TimedRepeater() { }

		public TimedRepeater(Task childTask, float duration = 3f, float durationRandom = 0f) : base(childTask)
		{
			this.duration = duration;
			this.durationRandom = durationRandom;
		}

		#region Overridden Virtual Method

		protected override void OnEntry()
		{
			base.OnEntry();

			_timer = 0f;
			_currentDuration = duration + Random.Range(0f, durationRandom);
		}

		protected override TaskStatus OnUpdate()
		{
			if (child == null)
			{
				return TaskStatus.Failure;
			}

			var status = Update(child);
			bool childFinished = status is TaskStatus.Success or TaskStatus.Failure;
			if (childFinished)
			{
				Restart(child);
			}

			_timer += Time.deltaTime;

			if (_timer >= _currentDuration)
			{
				return TaskStatus.Success;
			}
			
			return TaskStatus.Running;
		}

		#endregion
	}
}