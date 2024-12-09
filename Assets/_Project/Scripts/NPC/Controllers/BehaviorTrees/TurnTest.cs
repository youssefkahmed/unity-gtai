using GTAI.NPCTasks.Actions.Locomotion;
using GTAI.NPCTasks.Actions.Speech;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCControllers.ExternalBehaviorTrees
{
	[CreateAssetMenu(menuName = "GTAI/BehaviorTrees/Test/Turning Test")]
	public class TurnTest : ExternalBehaviorTree
	{
		public override Task CreateBehaviorTree()
		{
			Sequence sequence = new();
			sequence.AddTask(new LookAtRandomAngle());
			sequence.AddTask(new Wait());

			SayRandom say = new ("Turning...", "Uh-uh...", "Yep", "What's over there?", "Let's see...", "Huh?");

			sequence.AddTask(new RandomChance(say, 1f));

			return new Repeater(sequence);
		}
	}
}