using UnityEngine;

namespace GTAI.TaskSystem
{
	public class Log : Task
	{
		public string Text;

		public Log(string text)
		{
			Text = text;
		}

		public override TaskStatus OnUpdate()
		{
			Debug.Log(Text);

			return TaskStatus.Success;
		}
	}
}