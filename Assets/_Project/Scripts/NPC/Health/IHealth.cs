namespace GTAI.NPCs.Health
{
	public interface IHealth
	{
		float Points { get; set; }

		float MaxPoints { get; set; }

		bool IsAlive { get; }
	}
}