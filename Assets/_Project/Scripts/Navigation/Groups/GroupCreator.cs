using GTAI.Navigation.NPCs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GTAI.Navigation.Groups
{
    [RequireComponent(typeof(GroupManager))]
    public class GroupCreator : MonoBehaviour
    {
        [Header("Data:")]
        [SerializeField] private Formation[] formations;
        [SerializeField] private Area.Area[] areas;
        
        [Header("Prefabs:")]
        [SerializeField] private NPC npcLeaderPrefab;
        [SerializeField] private NPC npcFollowerPrefab;
        
        [Header("Values:")]
        [SerializeField] private int minGroupSize = 3;
        [SerializeField] private int maxGroupSize = 12;
        [SerializeField] private int groupCount = 6;

        private GroupManager _groupManager;

        private void Awake()
        {
            _groupManager = GetComponent<GroupManager>();
        }

        private void Start()
        {
            for (var i = 0; i < groupCount; i++)
            {
                CreateRandomGroup(i);
            }
        }

        private void CreateRandomGroup(int groupIndex)
        {
            Group group = _groupManager.CreateGroup();

            Transform parent = new GameObject($"Group {groupIndex + 1}").transform;
            Area.Area area = GetRandomArea();
            Formation formation = GetRandomFormation();

            // Creating the Leader
            Vector3 leaderPosition = area.GetRandomPoint();
            Quaternion leaderRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            NPC leader = Instantiate(npcLeaderPrefab, leaderPosition, leaderRotation);
            leader.transform.SetParent(parent);
            if (leader.TryGetComponent(out NPCWander wander))
            {
                wander.SetArea(area);
            }

            // Adding Leader to group
            group.AddMember(leader);
            group.SetFormation(formation);

            // Creating Followers
            int followerCount = Random.Range(minGroupSize, maxGroupSize + 1);
            for (var i = 0; i < followerCount; i++)
            {
                NPC follower = Instantiate(npcFollowerPrefab, leaderPosition, leaderRotation);
                follower.transform.SetParent(parent);
                group.AddMember(follower);

                Vector3 memberPosition = group.GetPositionInGroup(follower);
                follower.transform.position = memberPosition;
            }
        }

        private Area.Area GetRandomArea()
        {
            return areas[Random.Range(0, areas.Length)];
        }
        
        private Formation GetRandomFormation()
        {
            return formations[Random.Range(0, areas.Length)];
        }
    }
}