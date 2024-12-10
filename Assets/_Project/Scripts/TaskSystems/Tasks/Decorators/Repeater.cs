using UnityEngine;

namespace GTAI.TaskSystem.Decorators
{
	public class Repeater : Decorator
	{
		public bool repeatForever = true;
		public int iterations = 10;
		public int iterationsRandom;

		/// <summary>
		/// Should the repeater end and return failure when the child has failed?
		/// </summary>
		public bool endOnFailure = false;

		private int timesToRepeat;

		public Repeater()
		{

		}

		public Repeater(Task childTask, bool forever=true, int iterations=10, int iterationsRandom = 0): base(childTask)
		{
			repeatForever = forever;
			this.iterations = iterations;
			this.iterationsRandom = iterationsRandom;
		}

		#region Overridden Virtual Methods

		protected override void OnEntry()
		{
			base.OnEntry();

			timesToRepeat = iterations + Random.Range(0, iterationsRandom + 1);
		}

		protected override TaskStatus OnUpdate()
		{
			if (child == null)
			{
				return TaskStatus.Failure;
			}

			TaskStatus status = Update(child);
			// When endOnFailure is on, we end the child task and return.
			if (status == TaskStatus.Failure && endOnFailure)
			{
				Stop(child);
				return TaskStatus.Failure;
			}
			
			bool childFinished = status is TaskStatus.Success or TaskStatus.Failure;
			if (repeatForever && childFinished)
			{
				Restart(child);
			}
			
			if (!repeatForever && childFinished)
			{
				timesToRepeat--;

				Stop(child);

				if (timesToRepeat <= 0)
				{
					return TaskStatus.Success;
				}
				Start(child);
			}
			
			return TaskStatus.Running;
		}

		#endregion
	}
}