using GTAI.NPCs;
using GTAI.Sensors;
using GTAI.TaskSystem;

namespace GTAI.NPCTasks
{
	public class SharedNPC : SharedVariable<NPC>
	{
		public static implicit operator SharedNPC(NPC value)
		{
			return new SharedNPC { Value = value };
		}
	}
	
	public class SharedDetectionEntry : SharedVariable<DetectionEntry>
	{
		public static implicit operator SharedDetectionEntry(DetectionEntry value)
		{
			return new SharedDetectionEntry { Value = value };
		}
	}
}