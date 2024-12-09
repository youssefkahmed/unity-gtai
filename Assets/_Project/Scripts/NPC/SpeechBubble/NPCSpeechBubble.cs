using GTAI.NPCs.Component;
using UnityEngine;

namespace GTAI.NPCs.Speech
{
	public class NPCSpeechBubble : NPCComponent
	{
		[SerializeField] private GameObject root;
		[SerializeField] private TypingEffect typingEffect;

		[SerializeField] private float typingDuration = 1f;
		[SerializeField] private float visibleDuration = 3f;

		private Camera _mainCam;
		private float _visibleTimer;

		#region Unity Event Methods

		private void Start()
		{
			_mainCam = Camera.main;
			
			root.SetActive(false);

			npc.OnSay += OnSay;
		}
		
		private void Update()
		{
			if (_visibleTimer <= 0.01f)
			{
				_visibleTimer = 0f;
				root.SetActive(false);
			}
			else
			{
				_visibleTimer -= Time.deltaTime;

				root.SetActive(true);

				// Align with camera
				if (_mainCam)
				{
					root.transform.forward = _mainCam.transform.forward;
				}
			}
		}

		private void OnDisable()
		{
			if (npc)
			{
				npc.OnSay -= OnSay;
			}
		}

		#endregion
		
		#region Event Listeners

		private void OnSay(string text)
		{
			_visibleTimer = visibleDuration;

			root.SetActive(true);
			typingEffect.StartTyping(text, typingDuration);
		}

		#endregion
	}
}
