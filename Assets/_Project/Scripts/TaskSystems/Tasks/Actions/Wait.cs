using UnityEngine;

namespace GTAI.TaskSystem
{
	public class Wait : Task
	{
		public readonly float duration;
		public readonly float durationRandom;

		private float _timer;

		public Wait(float duration = 1f, float durationRandom = 0f)
		{
			this.duration = duration;
			this.durationRandom = durationRandom;
		}

		#region Overridden Virtual Methods

		protected override void OnEntry()
		{
			_timer = duration + Random.Range(0f, durationRandom);
		}

		protected override TaskStatus OnUpdate()
		{
			_timer -= Time.deltaTime;
			if (_timer <= 0f)
			{
				_timer = 0f;

				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}