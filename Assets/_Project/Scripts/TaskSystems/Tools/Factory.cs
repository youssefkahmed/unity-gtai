using GTAI.TaskSystem.Composites;
using GTAI.TaskSystem.Decorators;

namespace GTAI.TaskSystem
{
	public class Factory
	{
		public static Repeater Repeater(
			Task childTask,
			string displayName = "", 
			bool forever=true, int iterations = 10,
			int iterationsRandom = 0)
		{
			var repeater = new Repeater(childTask, forever, iterations, iterationsRandom)
			{
				Name = displayName
			};
			return repeater;
		}

		public static Sequence Sequence(params Task[] tasks)
		{
			var sequence = new Sequence();
			foreach (var task in tasks)
			{
				sequence.AddTask(task);
			}

			return sequence;
		}

		public static Sequence Sequence(string displayName, params Task[] tasks)
		{
			var sequence = Sequence(tasks);

			sequence.Name = displayName;

			return sequence;
		}
	}
}