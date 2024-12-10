using System.Collections;
using UnityEngine;

namespace GTAI.NPCs.Combat
{
	[RequireComponent(typeof(Gun))]
	public class TracerSpawner : MonoBehaviour
	{
		[SerializeField] private LineRenderer tracerPrefab;
		[SerializeField] private float tracerSpeed = 100f;
		[SerializeField] private float tracerLifetime = 0.1f;

		private void Start ()
		{
			var gun = GetComponent<Gun>();

			gun.DrawTracer += (start, end) => { StartCoroutine(SpawnTracerRoutine(start, end)); };
		}

		private IEnumerator SpawnTracerRoutine(Vector3 start, Vector3 end)
		{
			var tracer = Instantiate(tracerPrefab, start, Quaternion.identity);

			float distance = Vector3.Distance(start, end);
			var elapsedTime = 0f;

			while (elapsedTime < tracerLifetime)
			{
				elapsedTime += Time.deltaTime;

				float t = Mathf.Clamp01(elapsedTime * tracerSpeed / distance);

				// Update tracer position to move towards the end point
				Vector3 currentPoint = Vector3.Lerp(start, end, t);
				tracer.SetPosition(0, start);
				tracer.SetPosition(1, currentPoint);

				yield return null;
			}

			// Finalize position and destroy tracer
			tracer.SetPosition(1, end);
			Destroy(tracer.gameObject, tracerLifetime);
		}
	}
}