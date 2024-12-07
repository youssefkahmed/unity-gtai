using GTAI.NPCs;
using GTAI.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace GTAI.NPCControllers
{
	/// <summary>
	/// Class <c>PlayerController</c> allows receiving player input and mapping it to control the movement and rotation
	/// of an NPC GameObject 
	/// </summary>
	public class PlayerController : NPCController
	{
		// Access to the NPC, mainly used by the UI scripts.
		public NPC Npc => npc;
		
		[Header("Components:")]
		[SerializeField] private CharacterController characterController;
		
		[Header("Camera Settings:")]
		[SerializeField] private Vector3 cameraOffset = new(0f, 1.2f, 0f);
		[SerializeField] private Transform cameraTarget;
		[SerializeField] private float lookSmoothing = 5f;
		[SerializeField] private Vector2 lookSensitivity = Vector2.one * 0.4f;
		
		[Header("Movement Settings:")]
		[SerializeField] private float moveSmoothing = 5f;

		private NavMeshAgent _agent;

		private bool _inputEnabled = true;
		private Vector2 _lookInput;
		private Vector2 _moveInput;
		private bool _runInput;
		private Vector2 _lookAngle = Vector2.zero;

		#region Unity Event Methods

		private void Start()
		{
			if (cameraTarget == null)
			{
				var cam = FindObjectOfType<ThirdPersonCameraTarget>(); 
				cameraTarget = cam?.transform ?? new GameObject().transform;
			}
		}

		private void Update()
		{
			UpdateLookAngle();
			UpdateMovement();
			ApplyGravity();

			characterController.Move(npc.Velocity * Time.deltaTime);

			if (_agent)
			{
				_agent.isStopped = true;
				_agent.Warp(transform.position);
			}
		}

		private void LateUpdate()
		{
			cameraTarget.position = npc.Position + cameraOffset;
			cameraTarget.rotation = Quaternion.Lerp(cameraTarget.rotation, Quaternion.Euler(-_lookAngle.y, _lookAngle.x, 0f), lookSmoothing * Time.deltaTime);
		}

		#endregion
		
		#region Base Class Overrides

		public override void SetNPC(NPC newNpc)
		{
			base.SetNPC(newNpc);
			if (newNpc == null)
			{
				return;
			}
			
			if (characterController == null)
			{
				newNpc.TryGetComponent(out characterController);
			}

			if (characterController == null)
			{
				Debug.LogError($"[{nameof(PlayerController)}] Character Controller not found on {newNpc.gameObject.name}");
			}
			
			_agent = newNpc.GetComponent<NavMeshAgent>();

			UIManager.Instance.Player = this;
		}

		#endregion
        
		#region Input

		public void SetInputEnabled(bool inputEnabled)
		{
			_inputEnabled = inputEnabled;
		}
        
		private void OnMove(InputValue input)
		{
			_moveInput = input.Get<Vector2>();
		}

		private void OnLook(InputValue input)
		{
			_lookInput = input.Get<Vector2>();
		}

		private void OnRun(InputValue input)
		{
			_runInput = input.isPressed;
		}

		#endregion

		#region Movement and Looking Method

		private void ApplyGravity()
		{
			if (characterController.isGrounded == false)
			{
				Vector3 velocity = npc.Velocity;
				velocity.y += Physics.gravity.y * Time.deltaTime;
				
				npc.Velocity = velocity;
			}
			else
			{
				Vector3 velocity = npc.Velocity;
				velocity.y = -3f;
				
				npc.Velocity = velocity;
			}
		}

		private void UpdateLookAngle()
		{
			if (_inputEnabled == false)
			{
				return;
			}

			_lookAngle.x += _lookInput.x * lookSensitivity.x;
			_lookAngle.y += _lookInput.y * lookSensitivity.y;

			_lookAngle.y = Mathf.Clamp(_lookAngle.y, -70f, 70f);
		}

		private void UpdateMovement()
		{
			if (_inputEnabled == false)
			{
				return;
			}

			Vector3 forward = cameraTarget.forward;
			forward.y = 0f;
			forward.Normalize();

			Vector3 motion = forward * _moveInput.y + cameraTarget.right * _moveInput.x;
			motion.y = 0f;
			motion.Normalize();
			
			float targetSpeed = _runInput ? npc.RunSpeed : npc.WalkSpeed;

			float velocityX = Mathf.Lerp(npc.Velocity.x, motion.x * targetSpeed, Time.deltaTime * moveSmoothing);
			float velocityZ = Mathf.Lerp(npc.Velocity.z, motion.z * targetSpeed, Time.deltaTime * moveSmoothing);

			npc.Velocity = new Vector3(velocityX, npc.Velocity.y, velocityZ);

			if (motion.sqrMagnitude > 0.01f)
			{
				Quaternion targetRotation = Quaternion.LookRotation(motion);

				npc.transform.rotation = Quaternion.Lerp(npc.transform.rotation, targetRotation, moveSmoothing * Time.deltaTime);
			}
		}

		#endregion
	}
}