using GTAI.NPCs.Components;
using UnityEngine;

namespace GTAI.NPCs.Locomotion
{
	/// <summary>
	/// Class <c>NPCLook</c> takes care of turning to face a target and orienting head
	/// </summary>
	public class NPCLook : NPCComponent
	{
		[Header("Values:")]
		[SerializeField] private float turnSpeed = 15f;
		[SerializeField] private float expireTime = 0.2f;

		[Header("Animator Settings:")]
		[SerializeField] private float animatorTurnSmoothing = 5f;
		
		private float _lookTimer;
		private Vector3 _targetPosition = Vector3.zero;
		private float _animatorTurnParameter;
		
		private static readonly int TurnHash = Animator.StringToHash("Turn");

		#region Unity Event Methods

		private void LateUpdate()
		{
			_lookTimer -= Time.deltaTime;

			if (_lookTimer < 0f)
			{
				_lookTimer = -1f;

				npc.CanTurn = true;

				_animatorTurnParameter = Mathf.Lerp(_animatorTurnParameter, 0f, animatorTurnSmoothing * Time.deltaTime);
			}
			else
			{
				npc.CanTurn = false;

				NPCLookAtPosition(_targetPosition);
			}

			npc.Animator.SetFloat(TurnHash, _animatorTurnParameter);
		}

		#endregion
		
		#region Overridden Virtual Methods

		protected override void SetNPC(NPC newNpc)
		{
			if (npc != null)
			{
				npc.OnLookAt -= OnLookAt;
				npc.IsLookingAt -= OnIsLookingAt;
			}

			base.SetNPC(newNpc);

			newNpc.OnLookAt += OnLookAt;
			newNpc.IsLookingAt += OnIsLookingAt;
		}

		#endregion

		#region Event Listeners

		private void OnLookAt(Vector3 position)
		{
			_lookTimer = expireTime;
			_targetPosition = position;
		}

		private bool OnIsLookingAt(Vector3 targetPosition)
		{
			Vector3 direction = targetPosition - npc.Position;
			direction.y = 0f;
			direction.Normalize();

			Vector3 fwd = npc.transform.forward;
			fwd.y = 0f;
			fwd.Normalize();

			return Vector3.Dot(fwd, direction) > 0.95f;
		}

		#endregion

		private void NPCLookAtPosition(Vector3 position)
		{
			// Calculate the direction to the target
			Vector3 directionToTarget = position - npc.Position;
			directionToTarget.y = 0; // Keep the object level on the y-axis (optional)

			// Calculate the target rotation
			Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

			// Smoothly rotate towards the target rotation
			npc.transform.rotation = Quaternion.RotateTowards(
					npc.transform.rotation,
					targetRotation,
					turnSpeed * Time.deltaTime
			);

			UpdateAnimatorTurnParameter(directionToTarget);
		}

		private void UpdateAnimatorTurnParameter(Vector3 targetPosition)
		{
			Vector3 normalizedDirection = targetPosition - npc.transform.position;
			normalizedDirection.y = 0f;
			normalizedDirection.Normalize();

			// Calculate the angle between the current forward direction and the target direction
			Vector3 forward = npc.transform.forward;

			float angle = Vector3.SignedAngle(forward, normalizedDirection, Vector3.up);

			const float maxAngle = 30f;
			angle = Mathf.Clamp(angle, -maxAngle, maxAngle) / maxAngle;
			
			_animatorTurnParameter = Mathf.Lerp(_animatorTurnParameter, angle, animatorTurnSmoothing * Time.deltaTime);
		}
	}
}