using GTAI.NPCs;
using GTAI.Sensors;
using UnityEngine;
using GTAI.TaskSystem;

namespace GTAI.NPCTasks.Actions.Combat
{
	public class SelectMainTarget : NPCAction
	{
		public SharedNPC mainTarget = new();
		
		/// <summary>
		/// Maintain the main target even when not visible for this duration.
		/// </summary>
		public readonly float stickyTargetDuration = 5f;

		private float _stickyTargetTimer;

		#region Overridden Virtual Methods
		
		protected override TaskStatus OnUpdate()
		{
			PickBestTarget();

			UpdateMainTarget();

			if (!mainTarget.Value)
			{
				return TaskStatus.Failure;
			}

			return TaskStatus.Success;
		}
		
		#endregion
		
		private float GetScore(NPC target)
		{
			if (!target)
			{
				return 0f;
			}

			float distance = npc.GetDistance(target);

			float distScoreNormalized = Mathf.InverseLerp(5f, 40f, distance);
			float distanceScore = Mathf.Lerp(1f, 5f, distScoreNormalized);

			float visibilityScore = sensor.IsVisible(target) ? 5f : 2f;

			return distanceScore + visibilityScore;
		}

		private void PickBestTarget()
		{
			NPC newTarget = null;
			foreach (DetectionEntry entry in sensor.detectionList.List)
			{
				if (entry.isVisible && entry.isHostile && entry.isAlive && entry.npc != newTarget)
				{
					if (GetScore(newTarget) < GetScore(entry.npc))
					{
						newTarget = entry.npc;
					}
				}
			}

			if (newTarget && GetScore(newTarget) > GetScore(mainTarget.Value))
			{
				mainTarget.Value = newTarget;
			}
		}

		private void UpdateMainTarget()
		{
			if (!mainTarget.Value)
			{
				return;
			}

			if (sensor.IsVisible(mainTarget.Value))
			{
				_stickyTargetTimer = 0f;
			}
			else
			{
				_stickyTargetTimer += Time.deltaTime;

				if (_stickyTargetTimer >= stickyTargetDuration)
				{
					mainTarget.Value = null;
				}
			}
		}
	}
}