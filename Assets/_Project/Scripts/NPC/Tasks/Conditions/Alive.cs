using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Conditions
{
	/// <summary>
	/// Succeeds when target NPC is not null and alive.
	/// </summary>
	public class Alive : Condition
	{
		public readonly SharedNPC target = new();

		public Alive() { }

		public Alive(SharedNPC target)
		{
			this.target = target;
		}
		
		protected override TaskStatus OnUpdate()
		{
			if (target == null || !target.Value || target.Value.IsAlive == false)
			{
				return TaskStatus.Failure;
			}

			return TaskStatus.Success;
		}
	}
}