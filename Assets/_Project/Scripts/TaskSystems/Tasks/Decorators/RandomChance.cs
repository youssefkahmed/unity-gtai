using UnityEngine;

namespace GTAI.TaskSystem.Decorators
{
	/// <summary>
	/// Will evaluate a probability and execute the child task or not. If not will return success.
	/// Inspired by the Unreal Engine decorator of the same name.
	/// </summary>
	public class RandomChance : Decorator
	{
		private readonly float _chance;
		private bool chanceIsSuccess;

		public RandomChance(Task childTask, float chance = 0.5f): base(childTask)
		{
			_chance = chance;
		}

		private static bool EvaluateChance(float chance)
		{
			if (chance <= 0f)
			{
				return false;
			}

			if (chance >= 1f)
			{
				return true;
			}

			return chance > Random.Range(0f, 1f);
		}

		#region Overridden Virtual Methods

		protected override void OnEntry()
		{
			if (EvaluateChance(_chance))
			{
				chanceIsSuccess = true;
				Start(child);
			}
			else
			{
				chanceIsSuccess = false;
			}
		}

		protected override TaskStatus OnUpdate()
		{
			if (child == null)
			{
				return TaskStatus.Failure;
			}

			if (!chanceIsSuccess)
			{
				return TaskStatus.Success;
			}

			TaskStatus status = Update(child);
			if (status is TaskStatus.Success or TaskStatus.Failure)
			{
				Stop(child);
				return status;
			}

			return TaskStatus.Running;
		}

		#endregion
	}
}