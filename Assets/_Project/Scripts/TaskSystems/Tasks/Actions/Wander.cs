using System;
using GTAI.NPCs;
using GTAI.TaskSystem;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GTAI.Tasks
{
	[Serializable]
	public class WanderParameters
	{
		public float searchRadius = 10.0f;
		
		public float maxWanderTime = 3f;
		
		public float minWaitTime = 3f;
		public float maxWaitTime = 8f;
	}
	
	public class Wander : NPCTask
	{
		private enum WanderState
		{
			Wandering,
			Waiting
		}

		private readonly WanderParameters _parameters;

		private WanderState _state = WanderState.Wandering;

		private Vector3 _wanderDestination;
		private float _waitTime;
		private float _wanderTime;
		
		public Wander(NPC npc, WanderParameters parameters) : base(npc)
		{
			_parameters = parameters;
		}

		#region Overridden NPCTask Methods

		public override float GetUtility()
		{
			return 1.0f;
		}

		public override void OnDrawGizmosSelected()
		{
			if (npc == null)
			{
				return;
			}

			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(npc.Position, _parameters.searchRadius);

			Gizmos.DrawLine(npc.Position, _wanderDestination);
			Gizmos.DrawSphere(_wanderDestination, 0.5f);
		}

		protected override void OnEntry()
		{
			if (_state == WanderState.Waiting)
			{
				ChangeState(WanderState.Wandering);
			}
			else
			{
				npc.OnSetDestination?.Invoke(_wanderDestination);
				npc.CanMove = true;
			}
		}

		protected override TaskStatus OnUpdate()
		{
			switch (_state)
			{
				case WanderState.Waiting:
				{
					_waitTime -= Time.deltaTime;
					if (_waitTime < 0f)
					{
						ChangeState(WanderState.Wandering);
					}
					break;
				}
				case WanderState.Wandering:
				{
					_wanderTime -= Time.deltaTime;
					if (HasArrived())
					{
						SetRandomDestination();
					}

					if (_wanderTime < 0f)
					{
						ChangeState(WanderState.Waiting);
					}
					break;
				}
			}

			return TaskStatus.Running;
		}

		#endregion

		private void ChangeState(WanderState newState)
		{
			_state = newState;
			switch (_state)
			{
				case WanderState.Wandering:
					npc.CanMove = true;

					SetRandomDestination();
					_wanderTime = _parameters.maxWanderTime;
					break;
				case WanderState.Waiting:
					_waitTime = Random.Range(_parameters.minWaitTime, _parameters.maxWaitTime);

					npc.CanMove = false;
					break;
			}
		}

		private bool HasArrived()
		{
			return Vector3.Distance(npc.Position, _wanderDestination) <= 1f;
		}

		private void SetRandomDestination()
		{
			for (var i = 0; i < 1000; i++)
			{
				Vector3 randomDirection = Random.insideUnitSphere * _parameters.searchRadius;
				randomDirection += npc.Position;

				if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 5f, NavMesh.AllAreas))
				{
					_wanderDestination = hit.position;
					npc.OnSetDestination?.Invoke(_wanderDestination);
					return;
				}
			}
		}
	}
}