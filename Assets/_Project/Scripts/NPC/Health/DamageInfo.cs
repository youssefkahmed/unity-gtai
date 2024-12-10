namespace GTAI.NPCs.Health
{
	public struct DamageInfo
	{
		public float damage;

		public static implicit operator DamageInfo(float damage)
		{
			return new DamageInfo{damage = damage};
		}
	}
}