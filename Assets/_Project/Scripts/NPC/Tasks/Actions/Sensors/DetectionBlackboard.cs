using GTAI.NPCs;
using GTAI.Sensors;
using UnityEngine;

namespace GTAI.NPCTasks.Actions.Sensors
{
	public class DetectionBlackboard: MonoBehaviour
	{
		public NPC mainTarget;

		// entry being investigated by the npc right now,
		// it could be out-of-date, which is good, we don't want our NPCs to be omniscient.
		public DetectionEntry investigatedEntry;

		public void Clear()
		{
			investigatedEntry = null;
			mainTarget = null;
		}
	}
}