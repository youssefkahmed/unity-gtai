using System;
using System.Collections.Generic;
using System.Linq;
using GTAI.NPCs;
using UnityEngine;

namespace GTAI.Sensors
{
	[Serializable]
    public class DetectionList
    {
		public IReadOnlyList<DetectionEntry> List => _list.AsReadOnly();
		public int Count => _list.Count;
		
        private List<DetectionEntry> _list = new();
        
		#region Useful Data

		/// <summary>
		/// Returns the number of visible and hostile NPCs
		/// </summary>
		public int VisibleHostilesCount
		{
			get
			{
				return _list.Count(entry => entry.isHostile && entry.isVisible);
			}
		}
		
		public int OutOfSightHostilesCount
		{
			get
			{
				return _list.Count(entry => entry.isHostile && !entry.isVisible);
			}
		}

		/// <summary>
		/// Returns the number of all visible NPCs including hostiles and non-hostiles.
		/// </summary>
		public int VisibleNPCsCount
		{
			get
			{
				return _list.Count(entry => entry.isVisible);
			}
		}
		
		#endregion
		
		public bool IsVisible(NPC npc)
		{
			return Contains(npc, out DetectionEntry entry) && entry.isVisible;
		}

		public DetectionEntry GetEntry(NPC npc)
		{
			return _list.FirstOrDefault(entry => npc == entry.npc);
		}
		
		public DetectionEntry GetEntryAtIndex(int index)
		{
			return _list[index];
		}

		/// <summary>
		/// Sorts by distance ascending.
		/// </summary>
		public void Sort()
		{
			_list.Sort((a, b) => a.distance.CompareTo(b.distance));
		}
		
		public void Clear()
		{
			_list.Clear();
		}

		public void Add(DetectionEntry entry)
		{
			_list.Add(entry);
		}

		public void Remove(DetectionEntry entry)
		{
			if (entry == null)
			{
				Debug.LogError("Trying to remove a null entry.");
				return;
			}

			_list.Remove(entry);
		}

		public void Remove(NPC npc)
		{
			DetectionEntry entry = GetEntry(npc);

			Remove(entry);
		}
		
		public bool Contains(NPC npc)
		{
			return Contains(npc, out _);
		}
		
		public bool Contains(NPC npc, out DetectionEntry outputEntry)
		{
			DetectionEntry entry = _list.FirstOrDefault(entry => npc == entry.npc);
			if (entry == null)
			{
				outputEntry = null;
				return false;
			}
			
			outputEntry = entry; 
			return true;
		}
		
		public void RemoveDeadEntries()
		{
			for (int i = _list.Count - 1; i >= 0; i--)
			{
				DetectionEntry entry = _list[i];

				// there should never be a null NPC field in any entry, it's better to get an error message
				// than to check for null NPCs here. Something would be very wrong if NPCs were null,
				// and we'd want to investigate that.
				if (!entry.isAlive) 
				{
					_list.RemoveAt(i);
				}
			}
		}

		public void RemoveOldEntries(float maxTime = 120f)
		{
			for (int i = _list.Count - 1; i >= 0; i--)
			{
				DetectionEntry entry = _list[i];

				float timeSinceVisible = Time.time - entry.lastSeenTime;
				if (timeSinceVisible >= maxTime)
				{
					_list.RemoveAt(i);
				}
			}
		}
    }
}