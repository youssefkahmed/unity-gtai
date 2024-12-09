using System.Collections;
using TMPro;
using UnityEngine;

namespace GTAI.NPCs.Speech
{
	public class TypingEffect : MonoBehaviour
	{
		[SerializeField] private TextMeshPro textMeshPro;
		
		private Coroutine _typingCoroutine;

		public void StartTyping(string text, float duration)
		{
			if (_typingCoroutine != null)
			{
				StopCoroutine(_typingCoroutine);
			}
			_typingCoroutine = StartCoroutine(TypeTextRoutine(text, duration));
		}

		private IEnumerator TypeTextRoutine(string text, float duration)
		{
			if (!textMeshPro)
			{
				Debug.LogError("TextMeshPro reference is missing!");
				yield break;
			}
			textMeshPro.text = "";

			float timePerCharacter = duration / text.Length;
			for (var i = 0; i <= text.Length; i++)
			{
				textMeshPro.text = text[..i];
				yield return new WaitForSeconds(timePerCharacter);
			}
		}
	}
}
