using GTAI.TaskSystem;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.NPCTasks.Actions.Combat
{
	/// <summary>
	/// If hostiles are detected, picks a safe position. Use alongside <c>GoTo</c> to make NPC move to the safe position.
	/// </summary>
	public class EvadeHostiles : NPCAction
	{
		public SharedVector3 safePosition = new();
		public float safeDistance = 15f; // Distance at which NPC should move away from hostiles
		public float minimumSafeDistance = 5f; // Minimum required distance to consider a position as safe
		public int maxAttempts = 10; // Maximum number of attempts to find a valid safe position

		protected override TaskStatus OnUpdate()
		{
			// If no hostiles are detected, return failure
			if (sensor.HostilesCount <= 0)
			{
				return TaskStatus.Failure;
			}

			// Get the evasion direction
			var evasionDirection = GetEvasionDirection();

			// Try to find a safe position by adjusting the direction if needed
			for (var i = 0; i < maxAttempts; i++)
			{
				// Compute a safe position in the current direction
				var potentialPosition = npc.Position + evasionDirection * safeDistance;

				// Try to find a valid position on the NavMesh
				if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas))
				{
					// Check if the found position is far enough from the NPC's current position
					if (Vector3.Distance(npc.Position, hit.position) >= minimumSafeDistance)
					{
						safePosition.Value = hit.position;
						return TaskStatus.Success;
					}
				}

				// Rotate the direction slightly and try again
				evasionDirection = Quaternion.Euler(0, 360f / maxAttempts, 0) * evasionDirection;
			}

			// If no valid position is found after maxAttempts, return failure
			return TaskStatus.Failure;
		}

		/// <summary>
		/// Calculates the evasion direction based on the positions of hostiles.
		/// </summary>
		/// <returns>A normalized direction vector away from the hostiles.</returns>
		private Vector3 GetEvasionDirection()
		{
			var evasionDirection = Vector3.zero;
			var hostileCount = 0;

			// Accumulate direction away from hostiles
			foreach (var entry in sensor.detectionList.List)
			{
				if (entry.isHostile)
				{
					evasionDirection += (npc.Position - entry.lastKnownPosition).normalized;
					hostileCount++;
				}
			}

			// If no hostiles are found, return Vector3.zero
			if (hostileCount == 0)
			{
				return Vector3.zero;
			}

			// Normalize the evasion direction
			return evasionDirection.normalized;
		}
	}
}
