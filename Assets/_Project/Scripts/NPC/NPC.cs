using GTAI.Groups;
using UnityEngine;
using UnityEngine.Events;

namespace GTAI.NPCs
{
    [SelectionBase]
    [RequireComponent(typeof(Animator))]
    public class NPC : MonoBehaviour
    {
        #region Events

        public UnityAction<Vector3> OnSetDestination = delegate {};
        public UnityAction<float> OnSetMaxSpeed = delegate {};

        #endregion

        #region Properties

        public Animator Animator { get; private set; }
        public AudioSource AudioSource { get; private set; }

        public bool IsAlive { get; private set; } = true;
        public bool CanMove { get; set; } = true;
        public Vector3 Velocity { get; set; } = Vector3.zero;
        public Vector3 Direction { get; private set; } = Vector3.zero;
        public Vector3 Position => transform.position;
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public Group Group { get; set; }

        #endregion

        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float runSpeed = 7f;

        #region Unity Event Methods

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            AudioSource = GetComponent<AudioSource>();
        }

        private void LateUpdate()
        {
            Vector3 horizontalVelocity = new(Velocity.x, 0f, Velocity.z);   
            if (horizontalVelocity.sqrMagnitude > 3f)
            {
                Direction = horizontalVelocity.normalized;
            }
        }

        #endregion

        #region Public Methods

        public bool HasGroup()
        {
            return Group != null;
        }

        public bool IsGroupLeader()
        {
            return Group.IsLeader(this);
        }

        public bool IsGroupFollower()
        {
            return !IsGroupLeader();
        }

        public bool GroupHasStragglers()
        {
            return Group.HasStragglers();
        }

        #endregion
    }
}
