using System;
using GTAI.TaskSystem;
using Random = UnityEngine.Random;

namespace GTAI.NPCTasks.Actions.Speech
{
	public class SayRandom : NPCAction
	{
		private readonly string[] _sentences;

		public SayRandom(params string[] sentences)
		{
			_sentences = sentences ?? Array.Empty<string>();
		}

		#region Overridden Virtual Methods

		protected override TaskStatus OnUpdate()
		{
			string sentence = _sentences[Random.Range(0, _sentences.Length)];

			npc.OnSay?.Invoke(sentence);
			
			return TaskStatus.Success;
		}

		#endregion
	}
}