using System;
using GTAI.NPCs;
using UnityEngine;

namespace GTAI.Sensors
{
    public class DetectionEntry : ICloneable
    {
        // Reference to our NPC
        // There is never a reason why NPC would be null here.
        public NPC npc;

        // The alive status of this entry, only updated when this entry is visible.
        // If instead we use NPC.Alive, it would make NPCs omniscient
        // (i.e. aware of the life status of other NPCs even when they're not visible)
        // maybe that's fine for some gameplay scenarios.
        public bool isAlive = true;
        
        public bool isVisible; // Do we currently see the NPC?
        public bool isHostile;
        
        public float distance; // Distance to last known position
        public Vector3 lastKnownPosition; // Last position of NPC since last seen
        public float lastSeenTime; // last time NPC was seen
        
        public object Clone()
        {
            var clone = new DetectionEntry
            {
                npc = npc,
                isAlive = isAlive,
                isVisible =isVisible,
                isHostile = isHostile,
                distance = distance,
                lastKnownPosition = lastKnownPosition,
                lastSeenTime = lastSeenTime
            };

            return clone;
        }
    }
}
