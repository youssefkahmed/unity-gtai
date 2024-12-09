using UnityEngine;

namespace GTAI.TaskSystem
{
	public abstract class ExternalBehaviorTree : ScriptableObject
	{
		/// <summary>
		/// Override this method to create a behavior tree and return the root.
		/// </summary>
		public abstract Task CreateBehaviorTree();

		/// <summary>
		/// Add shared variables here.
		/// </summary>
		public virtual void CreateData(BehaviorTreeData data) { }
	}
}