using GTAI.Sensors;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCTasks.Actions.Sensors
{
	public class GoToOutOfSightHostile : NPCAction
	{
		public SharedVariable<DetectionEntry> investigatedEntry = new();

		#region Overridden Virtual Methods

		public override float GetUtility()
		{
			if (sensor.VisibleHostilesCount == 0 && sensor.OutOfSightHostileCount > 0)
			{
				return 5f;
			}

			return 0f;
		}
		
		protected override void OnEntry()
		{
			npc.CanMove = true;

			foreach (DetectionEntry entry in sensor.detectionList.List)
			{
				if (entry.isVisible == false && entry.isHostile)
				{
					investigatedEntry.Value = (DetectionEntry)entry.Clone();

					npc.OnSetDestination?.Invoke(investigatedEntry.Value.lastKnownPosition);

					break;
				}
			}
		}

		protected override TaskStatus OnUpdate()
		{
			if (investigatedEntry.Value == null)
			{
				return TaskStatus.Success;
			}

			Vector3 lastKnownPosition = investigatedEntry.Value.lastKnownPosition;

			npc.OnSetDestination?.Invoke(lastKnownPosition);
			if (Vector3.Distance(lastKnownPosition, npc.Position) <= 3f)
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		#endregion
	}

}