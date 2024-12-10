using UnityEngine;
using Game.TaskSystem;
using GTAI.NPCTasks;
using GTAI.NPCTasks.Actions.Combat;
using GTAI.NPCTasks.Actions.Looking;
using GTAI.NPCTasks.Actions.Sensors;
using GTAI.NPCTasks.Actions.Speech;
using GTAI.NPCTasks.Conditions.Sensors;
using GTAI.TaskSystem;
using GTAI.TaskSystem.Actions;
using GTAI.TaskSystem.Composites;
using GTAI.TaskSystem.Decorators;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/TestInterruption")]
	public class TestInterruption : ExternalBehaviorTree
	{
		private readonly SharedNPC _target = new();

		public override Task CreateBehaviorTree()
		{
			var selector = new Selector();

			selector.AddTask(CreateCombatBranch());
			selector.AddTask(CreateIdleBranch());

			return selector;
		}

		public override void CreateData(BehaviorTreeData data)
		{
			data.npcList.Add(_target);
		}

		private Task CreateCombatBranch()
		{
			var sequence = new Sequence();
			sequence.interruptLowerPriority = true;

			sequence.AddCondition(new HasHostiles());

			sequence.AddTask(new SayRandom("I see them!", "Enemy spotted", "Hostiles!"));
			sequence.AddTask(new GetClosestVisibleHostile { target = _target });
			
			var lookAtTarget = new LookAtNPC { Target = this._target };

			var interrupt = new Interrupt(lookAtTarget, new CanSeeNPC { target = _target })
			{
				returnSuccess = false
			};

			sequence.AddTask(new ReturnSuccess(new Repeater(interrupt) { endOnFailure = true }));


			sequence.AddTask(new SayRandom("I don't see them anymore!", "I lost them!", "Where are they?"));

			sequence.AddTask(CreateLookingAroundBranch());

			sequence.AddTask(new SayRandom("Must be the wind!", "I give up!", "I don't have time for this"));

			sequence.AddTask(new ForgetDetectedNPC { target = _target });

			return sequence;
		}

		private static Task CreateLookingAroundBranch()
		{
			var root = new TimedRepeater { duration = 4f, durationRandom = 3f };

			var sequence = new Sequence();

			sequence.AddTask(new LookAtRandomAngle());
			sequence.AddTask(new Wait(0.8f));

			root.Child = sequence;

			return root;
		}

		private static Task CreateIdleBranch()
		{
			var sequence = new Sequence();

			sequence.CreateTasks(new SetAim { aim = false }, new Idle());

			return sequence;
		}
	}
}
