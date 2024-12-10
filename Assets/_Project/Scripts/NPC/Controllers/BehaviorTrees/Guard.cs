using GTAI.NPCTasks;
using GTAI.NPCTasks.Actions.Combat;
using GTAI.NPCTasks.Actions.Locomotion;
using GTAI.NPCTasks.Actions.Looking;
using GTAI.NPCTasks.Actions.Sensors;
using GTAI.NPCTasks.Actions.Speech;
using GTAI.Tasks;
using GTAI.TaskSystem;
using GTAI.TaskSystem.Actions;
using GTAI.TaskSystem.Composites;
using GTAI.TaskSystem.Decorators;
using UnityEngine;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/Guard")]
	public class Guard : ExternalBehaviorTree
	{
		[SerializeField] private WanderParameters wanderParameters;
		[SerializeField] private bool wanderEnabled = true;

		private readonly SharedNPC _mainTarget = new() { Name = "MainTarget" };

		#region Overridden Virtual Methods

		public override Task CreateBehaviorTree()
		{
			var utilitySelector = new UtilitySelector();
			utilitySelector.AddTask(Wander());
			utilitySelector.AddTask(new FollowGroup());
			utilitySelector.AddTask(Engage());
			utilitySelector.AddTask(Investigate());

			return utilitySelector;
		}
		
		public override void CreateData(BehaviorTreeData data)
		{
			data.npcList.Add(_mainTarget);
		}

		#endregion

		private Task Wander()
		{
			if (!wanderEnabled)
			{
				return new Idle { DefaultUtility = 0.5f };
			}

			Task wander = CreateWanderBranch();
			wander.DefaultUtility = 0.5f;

			return wander;
		}
		
		private Task CreateWanderBranch()
		{
			Sequence sequence = new();
			SharedVector3 wanderDestination = new();
			ParallelSelector parallelSelector = new();

			var moveSequence = new Sequence { Name = "Move" };
			moveSequence.AddTask(new PickRandomDestination { destination = wanderDestination });
			moveSequence.AddTask(new GoTo { destination = wanderDestination });

			parallelSelector.AddTask(new Repeater(moveSequence));
			parallelSelector.AddTask(new Wait(wanderParameters.maxWanderTime, wanderParameters.maxWanderTimeRandom));

			sequence.AddTask(new Wait(wanderParameters.maxWanderTime, wanderParameters.maxWanderTimeRandom));
			sequence.AddTask(parallelSelector);

			return new Repeater(sequence);
		}

		private Task Engage()
		{
			var root = new Parallel();

			var r = new Repeater(new SelectMainTarget { mainTarget = _mainTarget })
			{
				repeatForever = true,
				endOnFailure = true
			};

			root.AddTask(r);
			root.AddTask(new SayRandom("STOP!", "YOU THERE, STOP!", "HALT!", "HOSTILE SPOTTED!", "I SEE HIM!", "ENEMY IN SIGHT!", "DROP THE GUN!"));
			root.AddTask(new EngageMainTarget { mainTarget = _mainTarget });
			
			return root;
		}
		
		private static Task Investigate()
		{
			SharedDetectionEntry investigatedEntry = new();

			return Factory.Sequence("Investigate", 
				new SayRandom("I lost him!", "Where did he go?", "Where is he?"),
				
				new SetAim { aim = true },
				new GoToOutOfSightHostile { investigatedEntry = investigatedEntry },
				new SetAim { aim = false },

				Factory.Repeater(Factory.Sequence(new LookAtRandomAngle(), new Wait(1f, 3f)), "LookAround", false, 3, 3),

				new SayRandom("Must have been the wind...", "He's gone...", "I give up...", "I'm not paid enough for this!"),

				new ForgetDetectedEntry { entry = investigatedEntry }
				);
		}
	}
}