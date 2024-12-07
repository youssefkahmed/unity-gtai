using System.Collections.Generic;
using UnityEngine;

namespace GTAI.Groups
{
    public class GroupManager : MonoBehaviour
    {
        [SerializeField] private List<Group> groups = new();

        [ContextMenu("Create Group")]
        public Group CreateGroup()
        {
            var obj = new GameObject($"Group {groups.Count + 1}")
            {
                transform = { parent = transform }
            };

            var group = obj.AddComponent<Group>();
            groups.Add(group);
            return group;
        }
    }
}
