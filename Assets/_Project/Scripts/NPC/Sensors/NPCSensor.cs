using System.Collections;
using GTAI.NPCs;
using GTAI.NPCs.Components;
using UnityEngine;

namespace GTAI.Sensors
{
    public class NPCSensor : NPCComponent
    {
	    [Header("Values:")]
        [SerializeField] private float range = 10.0f;
		[SerializeField, Range(-1f, 1f)] private float fov = 0.5f;
		
		[SerializeField] private float scanRate = 0.25f;
		[SerializeField] private LayerMask scanMask = Physics.DefaultRaycastLayers;

		/// <summary>
		/// Contains all detected NPCs, including hostile targets
		/// </summary>
		[Header("Components:")]
		[SerializeField] public DetectionList detectionList = new();
	    
		[Header("Debug & Validation:")]
		[SerializeField] private bool drawGizmos;
		[SerializeField] private Color defaultGizmoColor = Color.yellow;
		[SerializeField] private Color detectedGizmoColor = Color.red;

		private readonly RaycastHit[] _hits = new RaycastHit[1000];
		private readonly Collider[] _colliders = new Collider[1000];
		
		#region Properties

		public int VisibleHostilesCount => detectionList.VisibleHostilesCount;

		public int OutOfSightHostileCount => detectionList.OutOfSightHostilesCount;
		
		public int HostilesCount => detectionList.HostilesCount;
		
		public bool IsVisible(NPC target) => detectionList.IsVisible(target);
		
		#endregion

		#region Debug & Validation

		private void OnDrawGizmos()
		{
			if (!drawGizmos)
			{
				return;
			}

			DrawFOVArc();

			// Drawing a line to each detected NPC
			foreach (DetectionEntry entry in detectionList.List)
			{
				Gizmos.color = defaultGizmoColor;
				if (entry.isVisible)
				{
					Gizmos.color = detectedGizmoColor;
				}

				Gizmos.DrawLine(npc.SensorPosition, entry.lastKnownPosition);
				Gizmos.DrawWireSphere(entry.npc.SensorPosition, 0.25f);
				Gizmos.DrawSphere(entry.lastKnownPosition, 0.1f);

				Gizmos.DrawLine(entry.lastKnownPosition, entry.npc.SensorPosition);
			}
		}

		private void DrawFOVArc()
		{
			Gizmos.color = defaultGizmoColor;

			// Calculate the half-angle in degrees from the cosine `fov` value
			float halfFOVAngle = Mathf.Acos(fov) * Mathf.Rad2Deg;

			// Left and right boundary angles for the FOV arc
			float angleLeft = -halfFOVAngle;
			float angleRight = halfFOVAngle;

			Vector3 verticalOffset = Vector3.up * 0.5f;

			const int segments = 24;
			float angleStep = (angleRight - angleLeft) / segments;
			Vector3 previousPoint = transform.position + Quaternion.Euler(0, angleLeft, 0) * transform.forward * range + verticalOffset;

			Gizmos.DrawLine(transform.position + verticalOffset, previousPoint);

			for (var i = 1; i <= segments; i++)
			{
				float currentAngle = angleLeft + angleStep * i;
				Vector3 currentPoint = transform.position + Quaternion.Euler(0, currentAngle, 0) * transform.forward * range;

				currentPoint += verticalOffset;

				Gizmos.DrawLine(previousPoint, currentPoint);
				previousPoint = currentPoint;
			}

			Gizmos.DrawLine(transform.position + verticalOffset, previousPoint);
		}
		
		#endregion
		
		#region Unity Event Methods

		private IEnumerator Start()
		{
			detectionList.Clear();

			yield return new WaitForSeconds(Random.Range(0f, scanRate));

			var scanTime = new WaitForSeconds(scanRate);
			while (true)
			{
				if (!enabled)
				{
					yield return scanTime;
				}

				Scan();
				UpdateDetectedEntries();

				detectionList.Sort();

				yield return scanTime;
			}
			// ReSharper disable once IteratorNeverReturns
		}

		private void Update()
		{
			// Updating entry values that don't require a raycast or heavier operations every frame.
			foreach (DetectionEntry entry in detectionList.List)
			{
				if (entry.isVisible)
				{
					entry.isAlive = entry.npc.IsAlive;
					entry.isHostile = IsHostile(entry.npc);

					entry.lastKnownPosition = entry.npc.Position;
					entry.lastSeenTime = Time.time;
				}
			}
		}

		private void OnDisable()
		{
			detectionList.Clear();
		}
		
		#endregion

		private void Scan()
		{
			int size = Physics.OverlapSphereNonAlloc(npc.SensorPosition, range, _colliders, scanMask, QueryTriggerInteraction.Ignore);
			for (var i = 0; i < size; i++)
			{
				var col = _colliders[i];
				var otherNPC = col.GetComponentInParent<NPC>();
				bool valid = otherNPC != null && otherNPC != npc && otherNPC.IsAlive;

				if (valid && !detectionList.Contains(otherNPC) && IsWithinFOV(otherNPC) && CanRaycast(otherNPC))
				{
					var entry = new DetectionEntry
					{
						npc = otherNPC,
						isAlive = true,
						isVisible = true,
						isHostile = IsHostile(otherNPC),
						distance = Vector3.Distance(npc.Position, otherNPC.Position),
						lastKnownPosition = otherNPC.Position,
						lastSeenTime = Time.time,
					};

					detectionList.Add(entry);
				}
			}
		}

		private bool IsHostile(NPC otherNPC)
		{
			return npc.IsHostile(otherNPC);
		}

		private bool IsWithinFOV(NPC target)
		{
			var toTarget = target.Position - npc.Position;
			toTarget.Normalize();

			return Vector3.Dot(npc.transform.forward, toTarget) > fov;
		}

		private bool CanRaycast(NPC target)
		{
			var ray = new Ray(npc.SensorPosition, target.SensorPosition - npc.SensorPosition);

			int size = Physics.RaycastNonAlloc(ray, _hits, range, scanMask, QueryTriggerInteraction.Ignore);
			if (size == 0)
			{
				return false;
			}
			
			var validHits = _hits[..size];
			System.Array.Sort(validHits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));
			
			foreach (var hit in validHits)
			{
				var hitNPC = hit.collider.GetComponentInParent<NPC>();
				
				// Ignoring hits on self
				if (hitNPC == npc)
				{
					continue;
				}

				if (hitNPC == null || hitNPC != target)
				{
					// We hit a wall or another NPC
					return false;
				}

				if (hitNPC == target)
				{
					return true;
				}
			}
			
			return false;
		}

		private void UpdateDetectedEntries()
		{
			detectionList.RemoveDeadEntries();
			detectionList.RemoveOldEntries();
			
			for (int i = detectionList.Count - 1; i >= 0; i--)
			{
				DetectionEntry entry = detectionList.GetEntryAtIndex(i);

				float distance = Vector3.Distance(npc.Position, entry.lastKnownPosition);

				entry.distance = distance;
				if (distance > range + 2f || !IsWithinFOV(entry.npc))
				{
					entry.isVisible = false;
				}
				else
				{
					entry.isVisible = CanRaycast(entry.npc);
				}

				if (entry.isVisible)
				{
					// we only update hostility and alive status when entry is visible
					entry.isAlive = entry.npc.IsAlive;
					entry.isHostile = IsHostile(entry.npc);

					entry.lastKnownPosition = entry.npc.Position;
					entry.lastSeenTime = Time.time;
				}
			}
		}
    }
}