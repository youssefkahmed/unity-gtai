using System.Collections.Generic;
using UnityEngine;

namespace GTAI.NPCs.Components
{
	public class NPCRagdoll : NPCComponent
	{
		[SerializeField]
		[Tooltip("The Root of the ragdoll, used when scanning.")]
		private GameObject ragdollRoot;

		[Header("Components (use context menu to auto set)")]
		[SerializeField] private List<Collider> ragdollColliders;
		[SerializeField] private List<Rigidbody> ragdollBodies;

		#region Overridden Virtual Method

		protected override void SetNPC(NPC newNpc)
		{
			base.SetNPC(newNpc);

			newNpc.OnRevived += OnRevived;
			newNpc.OnDied += OnDied;
		}

		#endregion

		#region Unity Event Methods

		private void Start()
		{
			SetRagdollActive(false);
		}

		#endregion
		
		private void OnDied()
		{
			SetRagdollActive(true);
			
			var velocity = npc.Velocity;
			foreach (var rb in ragdollBodies)
			{
				rb.velocity = velocity;
			}
		}

		private void OnRevived()
		{
			SetRagdollActive(false);
		}
		
		[ContextMenu("Scan Ragdoll Components")]
		private void ScanRagdollComponents()
		{
			if (ragdollRoot == null)
			{
				Debug.LogError("RagdollRoot is null, please set it before scanning.");
			}

			ragdollColliders = new List<Collider>(ragdollRoot.GetComponentsInChildren<Collider>());
			ragdollBodies = new List<Rigidbody>(ragdollRoot.GetComponentsInChildren<Rigidbody>());
		}

		private void SetCharacterControllerEnabled(bool isEnabled)
		{
			var cc = npc.GetComponent<CharacterController>();
			if (cc == null)
			{
				return;
			}

			cc.enabled = isEnabled;
		}
		
		public void SetRagdollActive(bool active)
		{
			// toggling the character controller
			SetCharacterControllerEnabled(!active);

			// toggling the animator
			npc.Animator.enabled = !active;
			
			// we don't need to disable colliders on the ragdoll
			// in fact, these colliders are useful for limb-based damage
			//
			//foreach (var c in RagdollColliders)
			//{
			//	c.enabled = active;
			//}

			// toggling the kinematic state of rigidbodies
			foreach (var b in ragdollBodies)
			{
				b.isKinematic = active ? false : true;
			}
		}
	}
}
