using GTAI.NPCs;

namespace GTAI.TaskSystem
{
	public abstract class NPCTask : Task
	{
		protected readonly NPC npc;

		protected NPCTask(NPC npc)
		{
			this.npc = npc;
		}
	}
}