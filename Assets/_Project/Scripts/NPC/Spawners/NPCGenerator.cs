using GTAI.Areas;
using GTAI.NPCs.Components;
using UnityEngine;

namespace GTAI.NPCs.Spawners
{
    public class NPCGenerator : MonoBehaviour
    {
        [SerializeField] private NPC npcPrefab;
        [SerializeField] private Area area;
        [SerializeField] private int npcCount = 15;

        private void Start()
        {
            for (var i = 0; i < npcCount; i++)
            {
                Vector3 position = area.GetRandomPoint();
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                NPC npc = Instantiate(npcPrefab, position, rotation);
                if (npc.TryGetComponent(out NPCWander _))
                {
                    // wander.SetArea(area);
                }
            }
        }
    }
}
