﻿
namespace GTAI.TaskSystem.Actions
{
	public class Idle : Action
	{
		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			return TaskStatus.Running;			
		}

		#endregion
	}
}