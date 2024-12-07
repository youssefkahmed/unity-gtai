using UnityEngine;

namespace GTAI.TaskSystem
{
	public class Wait : Task
	{
		public readonly float Duration;
		public readonly float DurationRandom;

		private float _timer;

		public Wait(float duration = 1f, float durationRandom = 0f)
		{
			Duration = duration;
			DurationRandom = durationRandom;
		}

		public override void OnEntry()
		{
			_timer = Duration + Random.Range(0f, DurationRandom);
		}

		public override TaskStatus OnUpdate()
		{
			_timer -= Time.deltaTime;
			if (_timer <= 0f)
			{
				_timer = 0f;

				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}
	}
}