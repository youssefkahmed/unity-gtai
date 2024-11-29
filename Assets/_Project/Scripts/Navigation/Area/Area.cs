using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GTAI.Navigation.Area
{
    public class Area : MonoBehaviour
    {
        [SerializeField] private float radius = 20f;

        #region Debug / Validation

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        #endregion

        #region Public Methods

        public Vector3 GetRandomPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection.y = 0f;

            Vector3 randomPosition = transform.position + randomDirection;
            Vector3 finalPosition = transform.position;

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 2f, 1))
            {
                finalPosition = hit.position;
            }

            return finalPosition;
        }

        #endregion
    }
}