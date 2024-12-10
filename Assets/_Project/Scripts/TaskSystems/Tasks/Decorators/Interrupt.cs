using System.Collections.Generic;
using UnityEngine;

namespace GTAI.TaskSystem.Decorators
{
    /// <summary>
    /// Keeps evaluating a list of conditions while the child is executing,
    /// if the conditions are false, the child is interrupted.
    /// 
    /// The conditions don't need to be present in the behavior tree to work.
    /// </summary>
    public class Interrupt : Decorator
    {
        public bool returnSuccess = true; // When interrupted, should this task return success or failure.

        public List<Condition> Conditions { get; private set; }
        
        public Interrupt(Task childTask, params Condition[] conditions) : base(childTask)
        {
            Conditions = new List<Condition>(conditions);
        }

        public void AddCondition(Condition condition)
        {
            Conditions.Add(condition);
        }

        #region Overridden Virtual Methods

        public override void SetOwner(GameObject owner)
        {
            base.SetOwner(owner);

            foreach (Condition condition in Conditions)
            {
                condition.SetOwner(owner);
            }
        }
        
        protected override void OnAwake()
        {
            base.OnAwake();

            foreach (var condition in Conditions)
            {
                Awaken(condition);
            }
        }

        protected override TaskStatus OnUpdate()
        {
            if (child == null)
            {
                return TaskStatus.Failure;
            }
            
            if (EvaluateConditions() == false)
            {
                return returnSuccess ? TaskStatus.Success : TaskStatus.Failure;
            }
            
            var status = Update(child);
            if (status is TaskStatus.Success or TaskStatus.Failure)
            {
                Stop(child);
                return status;
            }

            return TaskStatus.Running;
        }

        #endregion
        
        private bool EvaluateConditions()
        {
            foreach (Task task in Conditions)
            {
                var status = Update(task);
                if (status == TaskStatus.Failure)
                {
                    return false;
                }
            }

            return true;
        }
    }
}