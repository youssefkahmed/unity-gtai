using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Combat
{
	public class SetAim : NPCAction
	{
		public SharedBool aim;

		public SetAim() { }

		public SetAim(SharedBool aim)
		{
			this.aim = aim;
		}
		
		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			npc.OnSetAim(aim.Value);

			return TaskStatus.Success;
		}

		#endregion
	}
}