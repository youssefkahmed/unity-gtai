using GTAI.Sensors;
using UnityEngine;
using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Sensors
{
	public class ForgetDetectionEntry : NPCAction
	{
		public SharedVariable<DetectionEntry> entry = new();

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			if (entry.Value == null)
			{
				Debug.LogError($"{GetType().Name} action: entry is null");

				return TaskStatus.Failure;
			}
			
			if (!entry.Value.npc)
			{
				Debug.LogError($"{GetType().Name} action: entry.NPC is null");
			}

			// we make sure to remove the entry by the NPC reference
			// because our own entry may be a clone, thus different from the entry in the NPC's detection list.
			sensor.detectionList.Remove(entry.Value.npc);

			entry.Value = null;

			return TaskStatus.Success;
		}

		#endregion
	}
}