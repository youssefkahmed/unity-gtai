using GTAI.Navigation.Groups;
using UnityEngine;
using UnityEngine.AI;

namespace GTAI.Navigation.NPCs
{
    [SelectionBase]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPC : MonoBehaviour
    {
        public Animator Animator { get; private set; }
        public NavMeshAgent NavMeshAgent { get; private set; }

        public bool IsAlive { get; private set; } = true;
        public float CurrentSpeed => NavMeshAgent.velocity.magnitude;
        public Vector3 Direction { get; private set; } = Vector3.zero;
        public Vector3 Position => transform.position;
        public Group Group { get; set; }
        
        [field: SerializeField] public bool Wander { get; set; }
        
        private void Awake()
        {
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (CurrentSpeed > 0.1f)
            {
                Direction = NavMeshAgent.velocity.normalized;
            }
        }
    }
}
