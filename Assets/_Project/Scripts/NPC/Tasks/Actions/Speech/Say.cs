using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Speech
{
	public class Say : NPCAction
	{
		public SharedString text = "";

		public Say() { }

		public Say(SharedString text)
		{
			this.text = text;
		}

		protected override TaskStatus OnUpdate()
		{
			if (text != null)
			{
				npc.OnSay?.Invoke(text.Value);
			}
			
			return TaskStatus.Success;
		}
	}
}