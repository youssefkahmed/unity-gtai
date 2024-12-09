using UnityEngine;

namespace GTAI.TaskSystem
{
	public enum TaskStatus
	{
		Success,
		Failure,
		Running,
		Inactive // default status of tasks, when a task is stopped without finishing, it goes back to this status.
	}

	internal class TaskViewParameters
	{
		internal bool showFoldout = true;
		internal float finishedTime = -1000.0f;

		/// <summary>
		/// Set when a task finishes to either success or failure,
		/// if set to inactive it means the task hasn't started yet or has been stopped before finishing.
		/// </summary>
		internal TaskStatus finishedStatus = TaskStatus.Inactive;
	}
	
	public abstract class Task
	{
		/// <summary>
		/// Used as to detect when Awake is being called more than once.
		/// </summary>
		public bool hasAwoken;
		
		/// <summary>
		/// The game object on which the behavior tree component is.
		/// </summary>
		protected GameObject Owner { get; set; }

		/// <summary>
		/// You can set the default utility of any task without having to override it by setting this value.
		/// for example: if you want to set the utility of a sequence, you'll have to have a custom child task
		/// where you define the utility, with this value, that won't be necessary.
		/// </summary>
		/// Each class that overrides GetUtility has the freedom to choose what to do with this value.
		/// They can add it to another value, return their own, or do anything they want with it.
		public float DefaultUtility { get; set; } = 0f;

		#region Status

		public TaskStatus Status { get; set; } = TaskStatus.Inactive;

		public bool Active => Status != TaskStatus.Inactive;

		public bool CompareStatus(TaskStatus other)
		{
			return Status == other;
		}
		
		#endregion
		
		#region Name and BehaviorTree Viewer Parameters

		/// <summary>
		/// Can be overloaded to display custom text in the BehaviorTree viewer window.
		/// </summary>
		public string Name { get; set; } = "";

		public virtual string FullName => string.IsNullOrEmpty(Name) ? GetType().Name : $"{Name} ({GetType().Name})";

		internal readonly TaskViewParameters ViewParameters = new();

		#endregion

		#region Static Task Manipulation Methods
		
		public static void Awaken(Task task)
		{
			if (task == null)
			{
				Debug.LogError("Trying to awaken null task.");
				return;
			}

			if (task.hasAwoken)
			{
				Debug.LogError($"Trying to awaken already awake task '{task.FullName}'.");
				return;
			}

			task.OnAwake();
			task.hasAwoken = true;
		}

		public static void Start(Task task)
		{
			if (task == null)
			{
				Debug.LogError("Trying to start a null task.");
				return;
			}

			if (task.hasAwoken == false)
			{
				Debug.LogError($"Starting task '{task.FullName}' without awakening it first.");
			}

			if (task.CompareStatus(TaskStatus.Running))
			{
				Debug.LogError($"Starting task '{task.FullName}' but it's already running.");
				return;
			}

			task.Status = TaskStatus.Running;
			task.OnEntry();
		}

		public static void Stop(Task task)
		{
			if (task == null)
			{
				Debug.LogError("Trying to stop a null task");
				return;
			}

			if (task.hasAwoken == false)
			{
				Debug.LogError($"Stopping task '{task.FullName}' without awakening it first.");
			}

			if (task.CompareStatus(TaskStatus.Inactive))
			{
				Debug.LogError($"Stopping task '{task.FullName}' but it's already inactive.");
				return;
			}


			task.Status = TaskStatus.Inactive;
			task.OnExit();
		}

		public static void Restart(Task task)
		{
			if (task.CompareStatus(TaskStatus.Inactive))
			{
				Start(task);
			}
			else
			{
				Stop(task);
				Start(task);
			}
		}

		public static TaskStatus Update(Task task)
		{
			if (task == null)
			{
				Debug.LogError("Trying to update a null task.");
				return TaskStatus.Failure;
			}

			// We do nothing if task has already finished
			// the parent should take care of stopping the task at this point.
			if (task.Status is TaskStatus.Success or TaskStatus.Failure)
			{
				return task.Status;
			}

			if (task.Status == TaskStatus.Inactive)
			{
				Debug.LogError($"Trying to update an inactive task {task.FullName}.");
				return TaskStatus.Inactive;
			}

			TaskStatus status = task.OnUpdate();
			task.Status = status;
			
			// Updating the finished time and status
			if (task.Status is TaskStatus.Success or TaskStatus.Failure)
			{
				task.ViewParameters.finishedTime = Time.time;
				task.ViewParameters.finishedStatus = task.Status;
			}
			
			return status;
		}

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Composites and decorators should override this method to set owner of children.
		/// </summary>
		public virtual void SetOwner(GameObject owner)
		{
			Owner = owner; 
		}
		
		public virtual float GetUtility()
		{
			return 0f;
		}
		
		public virtual void OnDrawGizmos() { }

		public virtual void OnDrawGizmosSelected() { }
		
		/// <summary>
		/// Called when the behavior tree is initialized, think of it as a constructor.
		/// </summary>
		protected virtual void OnAwake() { }
		
		/// <summary>
		/// Called before the task begins running.
		/// </summary>
		// If you have to start this task manually, use Task.Start.
		protected virtual void OnEntry() { }

		protected virtual TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}
		
		/// <summary>
		/// Called after the task ends on success or failure.
		/// </summary>
		// If you have to stop this task manually, use Task.Stop.
		protected virtual void OnExit() { }

		#endregion
	}
}