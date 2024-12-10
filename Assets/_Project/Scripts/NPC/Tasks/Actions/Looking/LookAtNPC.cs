using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Looking
{
	public class LookAtNPC : NPCAction
	{
		public SharedNPC Target;

		protected override TaskStatus OnUpdate()
		{
			if (Target == null || !Target.Value)
			{
				return TaskStatus.Failure;
			}

			var targetPosition = Target.Value.SensorPosition;
			
			npc.OnLookAt?.Invoke(targetPosition);
			if (npc.IsLookingAt(targetPosition))
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}
	}
}