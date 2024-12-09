using UnityEngine;

namespace GTAI.TaskSystem
{
	/// <summary>
	/// Decorators are fully responsible for starting and stopping their child class.
	/// Any class derived from this should abide by the above rule.
	/// </summary>
	public class Decorator : Task
	{
		public Task Child 
		{ 
			get => child;
			set => child = value;
		}
		
		protected Task child;

		public Decorator() { }

		public Decorator(Task childTask)
		{
			child = childTask;
		}

		#region Overridden Virtual Methods

		public override void SetOwner(GameObject owner)
		{
			base.SetOwner(owner);
			Child.SetOwner(owner);
		}

		protected override void OnAwake()
		{
			Awaken(child);
		}

		protected override void OnEntry()
		{
			Start(child);
		}

		public override void OnExit()
		{
			if (child.Active)
			{
				Stop(child);
			}
		}

		#endregion
	}
}