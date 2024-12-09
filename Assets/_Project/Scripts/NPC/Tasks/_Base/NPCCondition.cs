using GTAI.NPCs;
using GTAI.Sensors;
using GTAI.TaskSystem;

namespace GTAI.NPCTasks
{
	public abstract class NPCCondition : Condition
	{
		protected NPC npc;
		protected NPCSensor sensor;

		#region Overridden Virtual Methods

		protected override void OnAwake()
		{
			npc = Owner.GetComponentInParent<NPC>();
			sensor = Owner.GetComponentInParent<NPCSensor>();
		}

		#endregion
	}

}