using GTAI.NPCs;
using UnityEditor;
using UnityEngine;

namespace GTAI.TaskSystem
{
	[CustomEditor(typeof(BehaviorTree))]
	public class BehaviorTreeEditor : Editor
	{
		private bool _showData = true;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var behaviorTree = (BehaviorTree)target;

			EditorGUILayout.Space(15f);

			if (GUILayout.Button("Open Viewer"))
			{
				BehaviorTreeViewer.ShowWindow();
			}

			EditorGUILayout.Space(15f);

			_showData = EditorGUILayout.Foldout(_showData, "Data");

			if (_showData)
			{
				// Indent contents within the foldout
				EditorGUI.indentLevel++;
				DrawData(behaviorTree.Data);
				EditorGUI.indentLevel--;
			}

			// Apply any changes made to serialized properties
			if (GUI.changed)
			{
				EditorUtility.SetDirty(behaviorTree);
			}
		}

		private static void DrawData(BehaviorTreeData data)
		{
			GUI.enabled = false; // disabling GUI to make the fields read-only

			foreach (SharedVariable<NPC> v in data.npcList.List)
			{
				EditorGUILayout.ObjectField(v.Name, v.Value, typeof(NPC), true);
			}

			foreach (SharedVariable<float> v in data.floatList.List)
			{
				EditorGUILayout.FloatField(v.Name, v.Value);
			}

			foreach (SharedVariable<bool> v in data.boolList.List)
			{
				EditorGUILayout.Toggle(v.Name, v.Value);
			}

			foreach (SharedVariable<int> v in data.intList.List)
			{
				EditorGUILayout.IntField(v.Name, v.Value);
			}

			foreach (SharedVariable<string> v in data.stringList.List)
			{
				EditorGUILayout.TextField(v.Name, v.Value);
			}

			foreach (SharedVariable<Vector3> v in data.vector3List.List)
			{
				EditorGUILayout.Vector3Field(v.Name, v.Value);
			}

			foreach (SharedVariable<Vector2> v in data.vector2List.List)
			{
				EditorGUILayout.Vector2Field(v.Name, v.Value);
			}

			foreach (SharedVariable<Transform> v in data.transformList.List)
			{
				EditorGUILayout.ObjectField(v.Name, v.Value, typeof(Transform), true);
			}

			foreach (SharedVariable<GameObject> v in data.gameObjectList.List)
			{
				EditorGUILayout.ObjectField(v.Name, v.Value, typeof(GameObject), true);
			}

			GUI.enabled = true;
		}
	}
}
