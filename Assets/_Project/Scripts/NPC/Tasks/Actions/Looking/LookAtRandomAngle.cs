﻿using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCTasks.Actions.Looking
{
	public class LookAtRandomAngle : NPCAction
	{
		private const float MIN_ANGLE = 45f;
		private const float MAX_ANGLE = 180f;
		
		private Vector3 _target;

		#region Debug & Validation

		public override void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(npc.Position, _target);
		}

		#endregion

		#region Overridden Virtual Methods

		protected override void OnEntry()
		{
			ChooseRandomTarget();
		}

		protected override TaskStatus OnUpdate()
		{
			npc.OnLookAt?.Invoke(_target);
			if (npc.IsLookingAt(_target))
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		#endregion
		
		private void ChooseRandomTarget()
		{
			float angle = npc.transform.rotation.eulerAngles.y;
			float sign = Random.Range(0f, 100f) > 50f ? 1f : -1f;
			float deltaAngle = sign * Random.Range(MIN_ANGLE, MAX_ANGLE);

			_target = npc.Position + Quaternion.Euler(0f, angle + deltaAngle, 0f) * Vector3.forward * 1000.0f + Vector3.up * 1.5f;
		}
	}
}