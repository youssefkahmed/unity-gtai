using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GTAI.TaskSystem
{
	/// <summary>
	/// Composites are fully responsible for starting and stopping their child classes.
	/// Any class derived from this should abide by the above rule.
	/// </summary>
	public abstract class Composite : Task
	{
		public IEnumerable<Task> Tasks => tasks.AsReadOnly();
		protected readonly List<Task> tasks = new();
		
		public bool interruptLowerPriority = false;

		/// <summary>
		/// These conditions are reevaluated as long as this task is running,
		/// if at least one condition returns a failure, the composite fails.
		/// 
		/// These conditions exist within the composite task and don't have to be present in the behavior tree.
		/// 
		/// </summary>
		protected List<Condition> conditions = new();

		protected Composite() { }

		protected Composite(params Task[] tasks)
		{
			this.tasks = new List<Task>(tasks);
		}

		#region Task Manipulation

		public void ClearTasks()
		{
			tasks.Clear();
		}

		public void AddTask(Task task)
		{
			tasks.Add(task);
		}

		public void CreateTasks(params Task[] newTasks)
		{
			ClearTasks();
			foreach (Task task in newTasks)
			{
				AddTask(task);
			}
		}

		#endregion

		#region Conditions Methods

		public void SetConditions(params Condition[] cond)
		{
			conditions = new List<Condition>(cond);
		}

		public void AddCondition(Condition condition)
		{
			conditions.Add(condition);
		}
		
		public bool EvaluateConditions()
		{
			foreach (Condition c in conditions)
			{
				TaskStatus status = Update(c);

				if (status == TaskStatus.Failure)
				{
					return false;
				}
			}

			return true;
		}

		#endregion
		
		#region Overridden Virtual Methods

		public override void SetOwner(GameObject owner)
		{
			base.SetOwner(owner);

			foreach (Task task in tasks)
			{
				task.SetOwner(owner);
			}
		}
		
		public override float GetUtility()
		{
			var maxUtility = 0f;
			foreach (Task task in tasks)
			{
				float utility = task.GetUtility();

				if (utility > maxUtility)
				{
					maxUtility = utility;
				}
			}

			return maxUtility + DefaultUtility;
		}

		protected override void OnAwake()
		{
			foreach (Task task in tasks)
			{
				Awaken(task);
			}
		}

		protected override void OnExit()
		{
			// Stop any child task that's running
			foreach (Task task in tasks.Where(task => task.Active))
			{
				Stop(task);
			}
		}

		#endregion
	}
}