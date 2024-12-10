using UnityEngine;
using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Sensors
{
	public class ForgetDetectedNPC : NPCAction
	{
		public SharedNPC target = new();

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			if (!target.Value)
			{
				Debug.LogError($"{GetType().Name} action: entry is null");

				return TaskStatus.Failure;
			}

			// We make sure to remove the entry by the NPC reference
			// because our own entry may be a clone, thus different from the entry in the NPC's detection list.
			sensor.detectionList.Remove(target.Value);

			target.Value = null;

			return TaskStatus.Success;
		}

		#endregion
	}
}