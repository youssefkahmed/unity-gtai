using GTAI.NPCTasks.Actions.Locomotion;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/Test/Strafe Test")]
	public class StrafeTest : ExternalBehaviorTree
	{
		public override Task CreateBehaviorTree()
		{
			Parallel parallel = new();
			parallel.AddTask(new Repeater(new LookAtDirection { direction = Vector3.forward }));
			
			SharedVector3 destination = new();

			Sequence sequence = new();
			sequence.AddTask(new PickRandomDestination { destination = destination });
			sequence.AddTask(new GoTo { destination = destination });
			
			parallel.AddTask(new Repeater(sequence));

			return new Repeater(parallel);
		}
	}
}