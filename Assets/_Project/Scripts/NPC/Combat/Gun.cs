using GTAI.NPCs.Components;
using GTAI.NPCs.Health;
using UnityEngine;
using UnityEngine.Events;

namespace GTAI.NPCs.Combat
{
	public class Gun : NPCComponent
	{
		#region Events and Actions

		public UnityAction<Vector3, Vector3> DrawTracer;

		#endregion
		
		[Header("Firing")]
		[SerializeField] private float maxRange = 30f;
		[SerializeField] private float rateOfFire = 0.1f;
		
		[Header("Physics")]
		[SerializeField] private float hitImpulse = 200f;
		[SerializeField] private LayerMask hitMask = Physics.DefaultRaycastLayers;

		[Header("Damage")]
		[SerializeField] private float damageValue = 10.0f;
		[SerializeField] private float damageRandom = 5f;
		
		[Header("Accuracy")]
		[SerializeField] private float maxSpreadAngle = 5f;
		
		[Header("Components")]
		[SerializeField] private Transform muzzle;
		[SerializeField] private ParticleSystem muzzleFlashFX;
		
		private float _nextShootTime;

		#region Overridden Virtual Methods

		protected override void SetNPC(NPC newNpc)
		{
			if (npc != null)
			{
				npc.OnTryShoot -= TryShoot;
				npc.OnLookAt -= OnLookAt;
			}

			base.SetNPC(newNpc);

			if (newNpc != null)
			{
				newNpc.OnTryShoot += TryShoot;
				newNpc.OnLookAt += OnLookAt;
			}
		}

		private void OnLookAt(Vector3 target)
		{
			// we orient the muzzle towards the target, like they did in games made in the 90s.
			// if you want realistic aiming, you should use animation rigging
			// Code Monkey has a great tutorial on that on Youtube.
			muzzle.forward = target - muzzle.position;
		}
		
		#endregion

		private void PlayMuzzleFlashFX()
		{
			if (muzzleFlashFX == null)
			{
				return;
			}

			if (muzzleFlashFX.isPlaying)
			{
				muzzleFlashFX.Stop();
			}

			muzzleFlashFX.Play();
		}

		private void TryShoot()
		{
			if (Time.time >= _nextShootTime)
			{
				PlayMuzzleFlashFX();
				SpawnHitScanProjectile();

				_nextShootTime = Time.time + rateOfFire;
			}
		}

		private Vector3 ComputeShootingDirection()
		{
			var direction = muzzle.forward;

			float spreadX = Random.Range(-maxSpreadAngle, maxSpreadAngle) * 0.5f;
			float spreadY = Random.Range(-maxSpreadAngle, maxSpreadAngle) * 0.5f;

			return Quaternion.Euler(spreadX, spreadY, 0f) * direction;
		}

		private void ApplyHitImpulse(RaycastHit hit, Vector3 direction)
		{
			if (hit.rigidbody != null)
			{
				hit.rigidbody.AddForceAtPosition(direction * hitImpulse, hit.point, ForceMode.Impulse);
			}
		}

		private void SpawnHitScanProjectile()
		{
			var ray = new Ray(muzzle.position, ComputeShootingDirection());

			var hits = Physics.RaycastAll(ray, maxRange, hitMask, QueryTriggerInteraction.Ignore);

			System.Array.Sort(hits, (a, b) => { return a.distance.CompareTo(b.distance); });

			foreach (var hit in hits)
			{
				// ignoring hits on our own colliders
				var hitNPC = hit.collider.GetComponentInParent<NPC>();
				if (hitNPC != null && hitNPC == npc)
				{
					continue;
				}

				// looking and damaging a component that implements the IDamageable interface
				var damageable = hit.collider.GetComponentInParent<IDamageable>();
				if (damageable != null)
				{
					float damage = damageValue + Random.Range(0f, damageRandom);

					damageable.Damage(damage);

					DrawTracer?.Invoke(muzzle.position, hit.point);

					ApplyHitImpulse(hit, ray.direction);

					return;
				}
				
				DrawTracer?.Invoke(muzzle.position, hit.point);

				ApplyHitImpulse(hit, ray.direction);

				// We hit a wall or obstacle, we need to return.
				return;
			}

			DrawTracer?.Invoke(muzzle.position, ray.origin + ray.direction * maxRange);
		}
	}
}
