using GTAI.NPCs.Health;
using UnityEngine;

namespace GTAI.NPCs.Components
{
	public class NPCHealth : NPCComponent, IDamageable, IHealth
	{
		#region Properties

		public bool IsAlive => alive;
		
		public float Points
		{
			get => points;
			
			set
			{
				points = value;

				if (alive && points <= 0f)
				{
					alive = false;

					npc.OnDied?.Invoke();
				}
				else if (points > 0f && !npc.IsAlive)
				{
					alive = true;

					npc.OnRevived?.Invoke();
				}
			}
		}
		
		public float MaxPoints
		{
			get => maxPoints;
			set => maxPoints = Mathf.Max(value, 0f);
		}

		#endregion
		
		[SerializeField] protected float points = 100.0f;
		[SerializeField] protected float maxPoints = 100.0f;
		
		protected bool alive = true;

		#region Overridden Virtual Methods

		protected override void SetNPC(NPC newNpc)
		{
			base.SetNPC(newNpc);

			newNpc.Health = this;
		}

		#endregion
		
		public void Damage(DamageInfo damageInfo)
		{
			Points -= damageInfo.damage;
		}

		#region Testing Methods

		[ContextMenu("Test Kill")]
		private void TestKill()
		{
			Points = 0f;
		}

		[ContextMenu("Test Revive")]
		private void TestRevive()
		{
			Points = 1f;
		}

		[ContextMenu("Test Damage")]
		private void TestDamage()
		{
			Points -= 10f;
		}

		#endregion
	}
}
