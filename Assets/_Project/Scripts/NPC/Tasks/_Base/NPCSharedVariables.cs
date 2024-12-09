using GTAI.NPCs;
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
}