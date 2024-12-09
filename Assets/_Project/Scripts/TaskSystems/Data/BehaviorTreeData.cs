using GTAI.NPCs;
using UnityEngine;

namespace GTAI.TaskSystem
{
	public class BehaviorTreeData
	{
		public readonly SharedVariableList<NPC> npcList = new();

		public readonly SharedVariableList<float> floatList = new();
		public readonly SharedVariableList<bool> boolList = new();
		public readonly SharedVariableList<int> intList = new();
		public readonly SharedVariableList<string> stringList = new();

		public readonly SharedVariableList<Vector3> vector3List = new();
		public readonly SharedVariableList<Vector2> vector2List = new();

		public readonly SharedVariableList<Transform> transformList = new();
		public readonly SharedVariableList<GameObject> gameObjectList = new();
	}
}