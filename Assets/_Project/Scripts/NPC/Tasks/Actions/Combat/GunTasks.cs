using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Combat
{
	public class HasGun : NPCCondition
	{
		protected override TaskStatus OnUpdate()
		{
			return npc.IsCarryingGun ? TaskStatus.Success : TaskStatus.Failure;
		}
	}

	public class IsGunLoaded : NPCCondition
	{
		protected override TaskStatus OnUpdate()
		{
			const bool loaded = true; // TODO implement gun magazine

			return npc.IsCarryingGun && loaded ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}