using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Combat
{
	public class SetAim : NPCAction
	{
		public SharedVariable<bool> aim;

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			npc.OnSetAim(aim.Value);

			return TaskStatus.Success;
		}

		#endregion
	}
}