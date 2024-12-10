using GTAI.NPCTasks;
using GTAI.NPCTasks.Actions.General;
using GTAI.NPCTasks.Actions.Locomotion;
using GTAI.NPCTasks.Actions.Speech;
using GTAI.TaskSystem;
using GTAI.TaskSystem.Actions;
using GTAI.TaskSystem.Composites;
using GTAI.TaskSystem.Decorators;
using UnityEngine;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/Zombie")]
	public class Zombie : ExternalBehaviorTree
	{
		[SerializeField] private float followDistance = 10.0f;
		
		private readonly SharedNPC _player = new();

		public override Task CreateBehaviorTree()
		{
			Parallel parallel = new();

			parallel.AddTask(new GetPlayer { player = _player });
			parallel.AddTask(new FollowNPC { target = _player, minDistance = Mathf.Max(followDistance/4f, 3f), maxDistance = followDistance });
			parallel.AddTask(CreateSpeechBranch());

			return parallel;
		}

		public override void CreateData(BehaviorTreeData data)
		{
			data.npcList.Add(_player);
		}

		private static Task CreateSpeechBranch()
		{
			var sequence = new Sequence();

			sequence.AddTask(new Wait(1f, 5f));
			sequence.AddTask(new RandomChance(new SayRandom("Brraaainns...", "Grrrraaaahhh...", "Mmmmmh...", "Rrraaaaagh..."), chance: 0.6f));
			sequence.AddTask(new Wait(5f, 20f));

			return new Repeater(sequence);
		}
	}
}