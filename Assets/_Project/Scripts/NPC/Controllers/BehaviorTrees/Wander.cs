using GTAI.NPCTasks.Actions.Locomotion;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/Wander")]
	public class Wander : ExternalBehaviorTree
	{
		public enum TreeVersion
		{
			V1,
			V2
		}

		public TreeVersion version = TreeVersion.V2;

		#region Overridden Virtual Methods

		public override Task CreateBehaviorTree()
		{
			return version switch
			{
				TreeVersion.V1 => CreateV1(),
				TreeVersion.V2 => CreateV2(),
				_ => null
			};
		}

		public override void CreateData(BehaviorTreeData data)
		{
		}

		#endregion

		private Task CreateV1()
		{
			SharedVector3 wanderDestination = new();

			Sequence sequence = new();
			sequence.AddTask(new PickRandomDestination { destination = wanderDestination });
			sequence.AddTask(new GoTo { destination = wanderDestination });

			sequence.AddTask(new Wait(1f, 3f));


			return new Repeater(sequence);
		}

		private static Task CreateV2()
		{
			SharedVector3 wanderDestination = new();
			var moveSequence = new Sequence { Name = "Move" };
			moveSequence.AddTask(new PickRandomDestination { destination = wanderDestination });
			moveSequence.AddTask(new GoTo { destination = wanderDestination });

			ParallelSelector parallelSelector = new();
			parallelSelector.AddTask(new Repeater(moveSequence));
			parallelSelector.AddTask(new Wait(2f, 3f));
			
			Sequence sequence = new();
			sequence.AddTask(new Wait(1f, 3f));
			sequence.AddTask(parallelSelector);
			
			return new Repeater(sequence);
		}
	}
}