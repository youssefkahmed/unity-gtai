using GTAI.NPCs;
using GTAI.Player;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCTasks.Actions.General
{
	public class GetPlayer : NPCAction
	{
		public SharedNPC player = new();

		protected override TaskStatus OnUpdate()
		{
			var playerController = Object.FindObjectOfType<PlayerController>();
			if (playerController)
			{
				player.Value = playerController.GetComponentInParent<NPC>();
			}

			if (player == null || !player.Value)
			{
				return TaskStatus.Failure;
			}

			return TaskStatus.Success;
		}
	}
}