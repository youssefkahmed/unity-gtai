using Game.TaskSystem;
using GTAI.NPCTasks.Actions.Combat;
using GTAI.NPCTasks.Actions.Locomotion;
using GTAI.NPCTasks.Actions.Speech;
using GTAI.Tasks;
using GTAI.TaskSystem;
using GTAI.TaskSystem.Actions;
using GTAI.TaskSystem.Composites;
using GTAI.TaskSystem.Decorators;
using UnityEngine;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/Civilian")]
	public class Civilian : ExternalBehaviorTree
	{
		[SerializeField] private WanderParameters wanderParameters;
		
		public override Task CreateBehaviorTree()
		{
			var selector = new Selector();

			selector.AddTask(CreateEvadeBranch());
			selector.AddTask(CreateIdleBranch()); 

			return selector;
		}

		private Task CreateIdleBranch()
		{
			Task wander = Wander.CreateWanderBranch(wanderParameters);

			return new Interrupt(wander, new HasHostiles { invert = true });
		}

		private static Task CreateEvadeBranch()
		{
			var sequence = new Sequence();

			var destination = new SharedVector3();

			sequence.AddTask(new EvadeHostiles { safePosition = destination });
			sequence.AddTask(new RandomChance(new SayRandom("AAAAAHHHH!", "Hostiles!", "Run away!", "I don't wanna die!!"), 0.2f));

			var moveComposite = new ParallelSelector();
			moveComposite.AddTask(new GoTo { destination = destination, run = true });
			moveComposite.AddTask(new Wait(10f));
			sequence.AddTask(moveComposite);

			return sequence;
		}
	}
}