using UnityEngine;

namespace GTAI.TaskSystem
{
	public class BehaviorTree : MonoBehaviour
	{
		[SerializeField] private bool restartWhenDone = true;
		[SerializeField] private ExternalBehaviorTree externalBehaviorTree;

		public BehaviorTreeData Data = new();
		public Task Root;
		
		internal int restarts;
		private bool _hasAwoken;

		#region Debug & Validation

		private void OnDrawGizmos()
		{
			Root?.OnDrawGizmos();
		}

		private void OnDrawGizmosSelected()
		{
			Root?.OnDrawGizmosSelected();
		}
		
		#endregion

		#region Unity Event Methods

		private void Awake()
		{
			_hasAwoken = false;
		}

		private void Update()
		{
			if (!_hasAwoken)
			{
				AwakenRootTask();

				_hasAwoken = true;
			}

			if (Root == null)
			{
				return;
			}
			
			// We don't execute the tree if the root task is inactive
			if (Root is { Status: TaskStatus.Inactive })
			{
				return;
			}
			
			TaskStatus status = Task.Update(Root);
			if (status is TaskStatus.Success or TaskStatus.Failure)
			{
				if (restartWhenDone)
				{
					Task.Restart(Root);
					restarts++;
				}
				else
				{
					Task.Stop(Root);
				}
			}
		}

		#endregion

		internal void CreateBehaviorTreeInternal()
		{
			if (externalBehaviorTree)
			{
				Root = externalBehaviorTree.CreateBehaviorTree();
				externalBehaviorTree.CreateData(Data);
			}
		}

		private void AwakenRootTask()
		{
			// If we have an external behavior tree, we create it here
			if (externalBehaviorTree)
			{
				Root = externalBehaviorTree.CreateBehaviorTree();
				externalBehaviorTree.CreateData(Data);
			}
			
			if (Root == null)
			{
				Debug.LogError($"Behavior Tree in object '{gameObject.name}' has a null root task.");
				return;
			}

			// Setting the owner for each task of the tree
			Root.SetOwner(gameObject);

			// Calling OnAwake on each task on the tree
			Task.Awaken(Root);

			// Starting the root
			Task.Start(Root);
		}
	}
}