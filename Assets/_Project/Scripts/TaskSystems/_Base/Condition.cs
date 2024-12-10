namespace GTAI.TaskSystem
{
	public class Condition : Task
	{
		/// <summary>
		/// If true the return value of OnUpdate will be inverted.
		/// </summary>
		public bool invert = false;
		
		/// <summary>
		/// Conditions should not override the OnEntry/OnExit methods, so we seal them.
		/// </summary>
		protected sealed override void OnEntry() { }

		/// <summary>
		/// Conditions should not override the OnEntry/OnExit methods, so we seal them.
		/// </summary>
		protected sealed override void OnExit() { }
	}
}