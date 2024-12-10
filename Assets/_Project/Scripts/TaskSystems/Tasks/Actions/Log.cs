using UnityEngine;

namespace GTAI.TaskSystem.Actions
{
	public class Log : Task
	{
		public string text;

		public Log(string text)
		{
			this.text = text;
		}

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			Debug.Log(text);

			return TaskStatus.Success;
		}

		#endregion
	}
}