using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GTAI.NPCs.Component
{
    public class NPCWander : NPCComponent
    {
        private enum WanderState
        {
            Wandering,
            Waiting
        }
        
        [Header("State:")]
        [SerializeField] private WanderState state = WanderState.Wandering;
        
        [Header("Waiting:")]
        [SerializeField] private float minWaitTime = 3f;
        [SerializeField] private float maxWaitTime = 8f;
        
        [Header("Wandering:")]
        [SerializeField] private float maxWanderTime = 5f;
        [SerializeField] private float searchRadius = 10f;

        private Vector3 _wanderDestination;
        private float _waitTime;
        private float _wanderTime;

        #region Debug & Validation

        private void OnDrawGizmosSelected()
        {
            if (npc == null)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, searchRadius);

            Gizmos.DrawLine(transform.position, _wanderDestination);
            Gizmos.DrawSphere(_wanderDestination, 0.5f);
        }

        #endregion
        
        #region Unity Event Methods

        private void OnEnable()
        {
            switch (state)
            {
                case WanderState.Wandering:
                    npc.OnSetDestination?.Invoke(_wanderDestination);
                    npc.CanMove = true;
                    break;
                case WanderState.Waiting:
                    ChangeState(WanderState.Wandering);
                    break;
            }
        }

        private void Start()
        {
            ChangeState(WanderState.Wandering);
        }

        private void Update()
        {
            UpdateWandering();
        }

        #endregion
        
        private void UpdateWandering()
        {
            if (state == WanderState.Waiting)
            {
                _waitTime -= Time.deltaTime;
                if (_waitTime < 0f)
                {
                    ChangeState(WanderState.Wandering);
                }
            }
            else if (state == WanderState.Wandering)
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
            }
        }
        
        private void ChangeState(WanderState newState)
        {
            state = newState;
            switch (state)
            {
                case WanderState.Wandering:
                    _wanderTime = maxWanderTime;
                    npc.CanMove = true;
                    SetRandomDestination();
                    break;
                case WanderState.Waiting:
                    _waitTime = Random.Range(minWaitTime, maxWaitTime);
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
                Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
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