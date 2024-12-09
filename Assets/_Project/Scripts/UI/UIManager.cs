using GTAI.Formations;
using GTAI.Groups;
using GTAI.NPCControllers.Player;
using GTAI.NPCs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GTAI.UI
{
	public class UIManager : MonoBehaviour
	{
		#region Singleton Definition

		private static UIManager _instance;

		public static UIManager Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}
				
				_instance = FindObjectOfType<UIManager>();
				if (_instance == null)
				{
					Debug.LogError("No UIManager in scene");
				}

				return _instance;
			}
		}

		#endregion
		
		#region Properties

		private PlayerController _player;

		public PlayerController Player
		{
			set
			{
				_player = value;
				OnPlayerChanged?.Invoke(_player);
			}

			get => _player;
		}

		#endregion
		
		#region Actions and Events

		public UnityAction<PlayerController> OnPlayerChanged = delegate {};

		#endregion
		
		[Header("Menus")]
		[SerializeField] private GameObject pauseMenu;
		
		[Header("Prefabs")]
		[SerializeField] private NPC memberPrefab;

		[Header("Debugging (ReadOnly)")]
		[SerializeField] private bool pauseMode;

		public Group PlayerGroup
		{
			get
			{
				if (_player == null || _player.Npc == null)
				{
					return null;
				}

				return _player.Npc.Group;
			}
		}
		
		#region Unity Methods

		private void Start()
		{
			SetPauseMode(false);
		}

		#endregion
		
		#region UI Control Methods

		public void OnPauseButtonPressed()
		{
			SetPauseMode(!pauseMode);
		}

		public void OnExitButtonPressed()
		{
			Application.Quit();

			// Exiting play mode if we're in the editor
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				UnityEditor.EditorApplication.isPlaying = false;
			}
#endif
		}

		public void IncreasePlayerRank()
		{
			if (PlayerGroup == null)
			{
				return;
			}

			PlayerGroup.IncreaseRank(_player.Npc);
		}
		
		public void DecreasePlayerRank()
		{
			if (PlayerGroup == null)
			{
				return;
			}

			PlayerGroup.DecreaseRank(_player.Npc);
		}
		
		public void AddMemberToPlayerGroup()
		{
			if (PlayerGroup == null)
			{
				return;
			}

			Vector3 spawnPos = PlayerGroup.GetLeader().Position + Vector3.right * 1f;
			if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
			{
				spawnPos = hit.position;
			}
			
			NPC npc = Instantiate(memberPrefab, spawnPos, Quaternion.identity);
			PlayerGroup.AddMember(npc);
		}

		public void SetPlayerFormation(Formation formation)
		{
			if (formation == null || _player == null || _player.Npc == null || _player.Npc.Group == null)
			{
				return;
			}

			Group group = _player.Npc.Group;

			// TODO: only set formation when character is the leader
			group.SetFormation(formation);

			SetPauseMode(false);
		}
		
		private void SetPauseMode(bool pause)
		{
			pauseMode = pause;

			_player.SetInputEnabled(!pause);

			Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = pause;

			pauseMenu.SetActive(pause);
		}

		#endregion
	}
}
