﻿using GTAI.NPCs;
using GTAI.TaskSystem;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.NPCControllers
{
	public class AIController : NPCController
	{
		[Header("Components:")]
		[SerializeField] private NavMeshAgent agent;
		
		#region Debug & Validation

		protected virtual void OnDrawGizmos()
		{
			// utilitySelector.OnDrawGizmos();
		}

		protected virtual void OnDrawGizmosSelected()
		{
			// utilitySelector.OnDrawGizmosSelected();
		}

		#endregion
		
		#region Unity Event Methods

		protected virtual void Update()
		{
			if (!agent)
			{
				return;
			}

			npc.Velocity = agent.velocity;
			agent.isStopped = !npc.CanMove;
			agent.updateRotation = npc.CanTurn;
		}

		#endregion

		protected override void SetNPC(NPC newNpc)
		{
			if (npc != null)
			{
				npc.OnSetDestination -= NPC_OnSetDestination;
				npc.OnSetMaxSpeed -= NPC_OnSetMaxSpeed;
				npc.HasPathToDestination -= NPC_OnHasPathToDestination;
			}

			base.SetNPC(newNpc);
			if (newNpc == null)
			{
				return;
			}
			
			agent = GetComponentInParent<NavMeshAgent>();
			agent.speed = newNpc.WalkSpeed;
			
			newNpc.OnSetDestination += NPC_OnSetDestination;
			newNpc.OnSetMaxSpeed += NPC_OnSetMaxSpeed;
			newNpc.HasPathToDestination += NPC_OnHasPathToDestination;

			if (agent == null)
			{
				Debug.LogError($"NavMeshAgent not found on NPC '{newNpc.gameObject.name}'.");
			}
			
			CreateTasks();
		}

		#region Event Listeners

		private void NPC_OnSetDestination(Vector3 destination)
		{
			agent.SetDestination(destination);
		}

		private void NPC_OnSetMaxSpeed(float maxSpeed)
		{
			agent.speed = maxSpeed;
		}

		private bool NPC_OnHasPathToDestination()
		{
			return agent.pathStatus != NavMeshPathStatus.PathInvalid;
		}
		
		#endregion
		
		#region Task Management Methods

		protected virtual void CreateTasks() { }
		
		#endregion
	}
}