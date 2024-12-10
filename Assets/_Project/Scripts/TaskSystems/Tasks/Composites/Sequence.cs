using UnityEngine;

namespace GTAI.TaskSystem.Composites
{
    public class Sequence : Composite
    {
        protected int currentTaskIndex;
        protected Task CurrentTask
        {
            get
            {
                try
                {
                    return tasks[currentTaskIndex];
                }
                catch
                {
                    Debug.LogError($"Current task index {currentTaskIndex} out of bounds, task count = {tasks.Count}");

                    return null;
                }
            }
        }
        
        public Sequence() { }

        public Sequence(params Task[] tasks) : base(tasks) { }
        
        protected bool IsCurrentTaskIndexValid()
        {
            return currentTaskIndex >= 0 && currentTaskIndex < tasks.Count;
        }

        #region Overridden Virtual Methods

        public override void OnDrawGizmos()
        {
            if (IsCurrentTaskIndexValid())
            {
                CurrentTask.OnDrawGizmos();
            }
        }

        public override void OnDrawGizmosSelected()
        {
            if (IsCurrentTaskIndexValid())
            {
                CurrentTask.OnDrawGizmosSelected();
            }
        }

        protected override void OnEntry()
        {
            currentTaskIndex = 0;

            if (IsCurrentTaskIndexValid())
            {
                Start(CurrentTask);
            }
        }

        protected override TaskStatus OnUpdate()
        {
            if (tasks.Count <= 0)
            {
                return TaskStatus.Failure;
            }

            for (var i = 0; i < currentTaskIndex; i++)
            {
                if (ShouldInterruptLowerPriority(tasks[i]))
                {
                    Stop(CurrentTask);
                    currentTaskIndex = i;
                    Start(CurrentTask);

                    break;
                }
            }

            TaskStatus status = Update(CurrentTask);
            if (status == TaskStatus.Success)
            {
                Stop(CurrentTask);

                currentTaskIndex++;

                if (currentTaskIndex >= tasks.Count)
                {
                    return TaskStatus.Success;
                }
                
                Start(CurrentTask);
            }
            else if (status == TaskStatus.Failure)
            {
                Stop(CurrentTask);
                return TaskStatus.Failure;
            }

            return TaskStatus.Running;
        }

        #endregion

        private static bool ShouldInterruptLowerPriority(Task task)
        {
            if (task is Composite comp)
            {
                if (comp.interruptLowerPriority && comp.EvaluateConditions() == false)
                {
                    return true;
                }
            }

            return false;
        }
    }
}