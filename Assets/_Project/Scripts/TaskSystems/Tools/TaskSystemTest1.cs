using UnityEngine;

namespace GTAI.TaskSystem
{
	public class TaskSystemTest1 : MonoBehaviour
	{
		private Task _root;

		private void Start()
		{
			Sequence sequence = new();

			Log log = new("Hello World!");
			Wait wait = new();

			sequence.CreateTasks(log, wait);

			_root = new Repeater(sequence);

			Task.Awaken(_root);
			Task.Start(_root);
		}

		private void Update()
		{
			if (_root.Status == TaskStatus.Running)
			{
				Task.Update(_root);
			}
			else if (_root.Status is TaskStatus.Success or TaskStatus.Failure)
			{
				Task.Stop(_root);
			}
		}
	}
}