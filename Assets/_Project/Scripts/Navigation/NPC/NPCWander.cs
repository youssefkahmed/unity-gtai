using UnityEngine;

namespace GTAI.Navigation.NPCs
{
    public class NPCWander : NPCComponent
    {
        private enum WanderState
        {
            Wandering,
            Waiting
        }
        
        [SerializeField] private Area.Area area;
        [SerializeField] private WanderState state = WanderState.Wandering;
        [SerializeField] private float minWaitTime = 3f;
        [SerializeField] private float maxWaitTime = 8f;
        [SerializeField] private float maxWanderTime = 5f;
        
        private float _waitTime;
        private float _wanderTime;

        private void Start()
        {
            if (npc.Wander)
            {
                float rnd = Random.Range(0f, 1f);
                if (rnd > 0.5f)
                {
                    ChangeState(WanderState.Wandering);
                }
                else
                {
                    ChangeState(WanderState.Waiting);
                }
            }
        }

        private void Update()
        {
            Wander();
        }

        public void SetArea(Area.Area newArea)
        {
            area = newArea;
        }
        
        private void Wander()
        {
            if (!npc.Wander)
            {
                return;
            }
            
            switch (state)
            {
                case WanderState.Waiting:
                {
                    _waitTime -= Time.deltaTime;
                    if (_waitTime <= 0f)
                    {
                        ChangeState(WanderState.Wandering);
                    }
                    break;
                }
                case WanderState.Wandering:
                {
                    _wanderTime -= Time.deltaTime;
                    if (HasArrived() || _wanderTime <= 0f)
                    {
                        ChangeState(WanderState.Waiting);
                    }
                    break;
                }
            }
        }
        
        private void ChangeState(WanderState newState)
        {
            state = newState;
            switch (state)
            {
                case WanderState.Wandering:
                    npc.NavMeshAgent.isStopped = false;
                    _wanderTime = maxWanderTime;
                    SetRandomDestination();
                    break;
                case WanderState.Waiting:
                    npc.NavMeshAgent.isStopped = true;
                    _waitTime = Random.Range(minWaitTime, maxWaitTime);
                    break;
            }
        }
        
        private bool HasArrived()
        {
            return npc.NavMeshAgent.remainingDistance <= npc.NavMeshAgent.stoppingDistance;
        }
        
        private void SetRandomDestination()
        {
            npc.NavMeshAgent.SetDestination(area.GetRandomPoint());
        }
    }
}