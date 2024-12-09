using System;
using GTAI.Groups;
using UnityEngine;
using UnityEngine.Events;

namespace GTAI.NPCs
{
    public enum Team
    {
        Blue,
        Red
    }
    
    [SelectionBase]
    [RequireComponent(typeof(Animator))]
    public class NPC : MonoBehaviour
    {
        #region Events

        // Locomotion
        public UnityAction<Vector3> OnSetDestination = delegate {};
        public UnityAction<float> OnSetMaxSpeed = delegate {};
        /// <summary>
        /// After setting destination, this returns whether there is a path or not.
        /// </summary>
        public Func<bool> HasPathToDestination;

        /// <summary>
        /// Look at a point in world space.
        /// </summary>
        public UnityAction<Vector3> OnLookAt = delegate {};

        /// <summary>
        /// Returns true if looking at point in world space.
        /// </summary>
        public Func<Vector3, bool> IsLookingAt;
        
        // Speech
        public UnityAction<string> OnSay = delegate {}; // say something in a speech bubble
        public UnityAction<bool> OnSetAim = delegate {};
        
        #endregion

        #region Properties

        public Animator Animator { get; private set; }
        public AudioSource AudioSource { get; private set; }

        public bool IsAlive { get; private set; } = true;
        public bool IsCarryingGun { get; private set; }
        
        public bool CanMove { get; set; } = true;
        public bool CanTurn { get; set; } = true;
        
        public Vector3 Velocity { get; set; } = Vector3.zero;
        public Vector3 Direction { get; private set; } = Vector3.zero;
        public Vector3 Position => transform.position;
        
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        
        public Group Group { get; set; }
        public Team Team => Team.Blue;
        
        // NPC sensor will start raycasts from and to this position
        // TODO sensor should start from the "eyes", NPCs can turn their heads and we should take that into account.
        // also when checking if an NPC is visible, should we shoot a raycast to its head, torso, and limbs as well?
        public Vector3 SensorPosition => transform.position + Vector3.up * 1.5f;

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
            return Group;
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

        public bool IsHostile(NPC otherNpc)
        {
            return otherNpc.Team != Team;
        }
        
        public bool IsWithinDistance(Vector3 point, float distance)
        {
            return Vector3.Distance(Position, point) <= distance;
        }

        public bool IsWithinDistance(NPC otherNPC, float distance)
        {
            return Vector3.Distance(Position, otherNPC.Position) <= distance;
        }

        public float GetDistance(NPC otherNPC)
        {
            return Vector3.Distance(Position, otherNPC.Position);
        }
        
        #endregion
    }
}
