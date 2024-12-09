using GTAI.TaskSystem;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.NPCTasks.Actions.Locomotion
{
	/// <summary>
	/// Chooses a random destination in a radius around the NPC and stores it in a shared variable.
	/// </summary>
	public class PickRandomDestination : NPCAction
	{
		public SharedFloat radius = 20.0f;
		public SharedVector3 destination = new();

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			for (var i = 0; i < 1000; i++)
			{
				Vector3 randomDirection = Random.insideUnitSphere * radius.Value;
				randomDirection += npc.Position;

				if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 5f, NavMesh.AllAreas))
				{
					destination.Value = hit.position;
					return TaskStatus.Success;
				}
			}

			return TaskStatus.Failure;
		}

		#endregion
	}
}