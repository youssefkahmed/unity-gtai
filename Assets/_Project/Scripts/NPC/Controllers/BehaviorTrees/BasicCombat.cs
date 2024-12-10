using GTAI.NPCTasks;
using GTAI.NPCTasks.Actions.Combat;
using GTAI.NPCTasks.Actions.Looking;
using GTAI.NPCTasks.Conditions;
using GTAI.NPCTasks.Conditions.Sensors;
using GTAI.TaskSystem;
using GTAI.TaskSystem.Actions;
using GTAI.TaskSystem.Composites;
using GTAI.TaskSystem.Decorators;
using UnityEngine;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	/// <summary>
	/// A very basic combat behavior tree: if I have a visible target, I aim and shoot, if not, I do nothing.
	/// </summary>
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/Basic Combat")]
	public class BasicCombat : ExternalBehaviorTree
	{
		public SharedNPC mainTarget = new();

		private Task CreateGunManagementBranch()
		{
			Sequence shootSequence = new(new Wait(0f, 1.5f), new TimedRepeater(new TryShoot(), 2f, 1.5f));

			shootSequence.SetConditions(new CanSeeNPC(mainTarget), new IsLookingAtNPC(mainTarget));

			// TODO add reload sequence
			var selector = new Selector(shootSequence);
			return new Repeater(selector);
		}

		private Task CreateCombatBranch()
		{
			var parallel = new Parallel();

			parallel.AddTask(new SetAim(true));
			parallel.AddTask(new Repeater(new LookAtNPC { Target = mainTarget }));
			parallel.AddTask(CreateGunManagementBranch());

			parallel.SetConditions(new Alive(mainTarget));

			return parallel;
		}

		private Task CreateIdleBranch()
		{
			Sequence sequence = new(new SetAim(false), new Idle());

			sequence.SetConditions(new Alive(mainTarget) { invert = true });

			return sequence;
		}
		
		public override Task CreateBehaviorTree()
		{
			var parallel = new Parallel();

			parallel.AddTask(new Repeater(new SelectMainTarget { mainTarget = mainTarget }));
			parallel.AddTask(new Repeater(new Selector(CreateCombatBranch(), CreateIdleBranch())));

			return parallel;
		}
		
		public override void CreateData(BehaviorTreeData data)
		{
			data.npcList.Add(mainTarget);
		}
	}
}