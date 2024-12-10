using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Combat
{
	public class TryShoot : NPCAction
	{
		protected override TaskStatus OnUpdate()
		{
			if (!npc.IsCarryingGun)
			{
				return TaskStatus.Failure;
			}

			npc.OnTryShoot?.Invoke();

			return TaskStatus.Success;
		}
	}
}