using GTAI.Tasks;
using GTAI.TaskSystem;
using UnityEngine;

namespace GTAI.NPCControllers
{
    public class GuardAIController : AIController
    {
        [SerializeField] private WanderParameters wanderParameters;

        private Wander _wander;
        private FollowGroup _followGroup;
        
        protected override void CreateTasks()
        {
            _wander = new Wander(npc, wanderParameters);
            _followGroup = new FollowGroup(npc);

            utilitySelector.CreateTasks(_wander, _followGroup);
        }
    }
}