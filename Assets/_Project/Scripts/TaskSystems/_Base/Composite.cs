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