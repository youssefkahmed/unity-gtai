using GTAI.NPCs;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace GTAI.TaskSystem
{
	public class BehaviorTreeViewer : EditorWindow
	{
		private const float INDENT_SIZE = 20f;
		private const float MIN_CONTENT_SCALE = 0.5f;
		private const float MAX_CONTENT_SCALE = 2.0f;
		private const float COLOR_ANIMATION_DURATION = 0.75f;
		
		private Vector2 _scrollPosition;
		private BehaviorTree _behaviorTree;
		private int _depth;

		private readonly Color _runningColor = Color.green;
		private readonly Color _successColor = new Color32(0, 255, 0, 255);
		private readonly Color _failureColor = Color.red;
		private readonly Color _utilityColor = Color.yellow;

		private float _contentScale = 1.0f;
		private GUIStyle _labelStyle;

		private void OnGUI()
		{
			// Content scale GUI
			GUILayout.BeginHorizontal();
			_contentScale = EditorGUILayout.Slider("Content scale", _contentScale, MIN_CONTENT_SCALE, MAX_CONTENT_SCALE);
			GUILayout.EndHorizontal();

			// If we change the editor's line height, that would affect all editor GUIs
			// instead we get the same result by modifying the padding in contentStyle
			float scaledLineHeight = EditorGUIUtility.singleLineHeight * _contentScale;

			_labelStyle = new GUIStyle(EditorStyles.label)
			{
				fontSize = Mathf.RoundToInt(EditorStyles.label.fontSize * _contentScale)
			};
			
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			_depth = 0;
			
			if (_behaviorTree)
			{
				var npc = _behaviorTree.GetComponentInParent<NPC>();

				GUILayout.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight * _contentScale));
				EditorGUILayout.LabelField($"Behavior Tree (Restarts:{_behaviorTree.restarts})", _labelStyle);

				if (npc)
				{
					EditorGUILayout.LabelField($"NPC: {npc.gameObject.name}", _labelStyle);
				}
				
				if (!Application.isPlaying)
				{
					if (GUILayout.Button("Draw", new GUIStyle(EditorStyles.miniButton) { fontSize = _labelStyle.fontSize}))
					{
						_behaviorTree.CreateBehaviorTreeInternal();
					}
				}
				
				GUILayout.EndHorizontal();
			}

			if (_behaviorTree && _behaviorTree.Root != null)
			{
				DrawTask(_behaviorTree.Root);
			}
			else
			{
				EditorGUILayout.LabelField("Select a BehaviorTree object to begin.");
			}

			EditorGUILayout.EndScrollView();
		}
		
		[MenuItem("Task System/Behavior Tree Viewer")]
		public static void ShowWindow()
		{
			GetWindow<BehaviorTreeViewer>("Behavior Tree Viewer");
		}
		
		private void OnEnable()
		{
			Selection.selectionChanged += OnSelectionChanged;
			EditorApplication.update += OnUpdate;
		}

		private void OnDisable()
		{
			Selection.selectionChanged -= OnSelectionChanged;
			EditorApplication.update -= OnUpdate;
		}

		private void OnSelectionChanged()
		{
			if (Selection.activeGameObject == null)
			{
				return;
			}

			var bt = Selection.activeGameObject.GetComponent<BehaviorTree>();
			if (bt == null)
			{
				bt = Selection.activeGameObject.GetComponentInChildren<BehaviorTree>();
			}

			if (bt != null)
			{
				_behaviorTree = bt;
			}
		}
		
		private void OnUpdate()
		{
			if (_behaviorTree == null)
			{
				OnSelectionChanged();
			}
			Repaint();
		}

		#region GUI Methods

		private Color GetTaskTextColor(Task task)
		{
			if (task.Active)
			{
				return _runningColor;
			}
			
			if (task.ViewParameters.finishedStatus == TaskStatus.Inactive)
			{
				return GUI.skin.label.normal.textColor;
			}

			Color targetColor = task.ViewParameters.finishedStatus == TaskStatus.Success ? _successColor : _failureColor;

			float t = Mathf.InverseLerp(0f, COLOR_ANIMATION_DURATION, Time.time - task.ViewParameters.finishedTime);

			return Color.Lerp(targetColor, GUI.skin.label.normal.textColor, t);
		}

		private void DrawUtilityLabel(Task task)
		{
			Color oldColor = GUI.color; // Save the current GUI color
			GUI.color = _utilityColor; // Set to the new color

			var s = $" [Utility = {Mathf.Round(task.GetUtility() * 10.0f) / 10.0f}]";

			EditorGUILayout.LabelField(s, _labelStyle);

			// Restore the original color
			GUI.color = oldColor;
		}

		private void DrawTask(Task task, bool drawUtility = false)
		{
			Color oldColor = GUI.color; // Save the current GUI color
			GUI.color = GetTaskTextColor(task);

			if (task is Composite comp)
			{
				DrawComposite(comp, drawUtility);
			}
			else if (task is Decorator dec)
			{
				DrawDecorator(dec, drawUtility);
			}
			else
			{
				BeginTask(task);
				EditorGUILayout.LabelField(task.FullName, _labelStyle);

				if (drawUtility)
				{
					DrawUtilityLabel(task);
				}

				EndTask(task);
			}

			// Restore the original color
			GUI.color = oldColor;
		}

		private void BeginTask(Task task)
		{
			GUILayout.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight * _contentScale));

			// drawing indent
			GUILayout.Space(INDENT_SIZE * _contentScale * (_depth + 1));
		}

		private void EndTask(Task task)
		{
			GUILayout.EndHorizontal();

			float y = GUILayoutUtility.GetLastRect().y;

			// drawing execution box: green if the task is running, and grey if it's not
			Color color;
			var rect = new Rect(0, y, INDENT_SIZE * _contentScale, EditorGUIUtility.singleLineHeight * _contentScale);

			if (task.CompareStatus(TaskStatus.Running))
			{
				color = _runningColor;
			}
			else
			{
				
				if (task.ViewParameters.finishedStatus == TaskStatus.Inactive)
				{
					color = new Color(0.2f, 0.2f, 0.2f);
				}
				else
				{
					Color targetColor = task.ViewParameters.finishedStatus == TaskStatus.Success ? _successColor : _failureColor;

					float t = Mathf.InverseLerp(0f, COLOR_ANIMATION_DURATION, Time.time - task.ViewParameters.finishedTime);

					color = Color.Lerp(targetColor, new Color(0.2f, 0.2f, 0.2f), t);
				}
			}
			
			EditorGUI.DrawRect(rect, color);
			
			// Drawing an arrow to make the hierarchy easier to visualize
			if (task == _behaviorTree.Root)
			{
				return;
			}

			float arrowThickness = 1f * _contentScale;
			float arrowLength = (INDENT_SIZE + arrowThickness) * _contentScale / 2f;

			float x = (INDENT_SIZE * _depth + INDENT_SIZE * 0.5f - arrowThickness / 2f) * _contentScale;

			rect = new Rect(x, y, arrowThickness, arrowLength);
			EditorGUI.DrawRect(rect, GUI.skin.label.normal.textColor);

			y += INDENT_SIZE * 0.5f * _contentScale - arrowThickness;

			rect = new Rect(x, y, arrowLength - arrowThickness, arrowThickness);
			EditorGUI.DrawRect(rect, GUI.skin.label.normal.textColor);
		}

		private void DrawDecorator(Decorator task, bool drawUtility = false)
		{
			BeginTask(task);
			
			task.ViewParameters.showFoldout = EditorGUILayout.Foldout(task.ViewParameters.showFoldout, task.FullName, _labelStyle);

			if (drawUtility)
			{
				DrawUtilityLabel(task);
			}

			EndTask(task);

			_depth++;

			if (task.ViewParameters.showFoldout)
			{
				DrawTask(task.Child);
			}

			_depth--;
		}

		private void DrawComposite(Composite task, bool drawUtility = false)
		{
			bool isUtilitySelector = task is UtilitySelector;

			BeginTask(task);
			
			task.ViewParameters.showFoldout = EditorGUILayout.Foldout(task.ViewParameters.showFoldout, task.FullName, _labelStyle);

			if (drawUtility)
			{
				DrawUtilityLabel(task);
			}

			EndTask(task);

			_depth++;

			if (task.ViewParameters.showFoldout)
			{
				foreach (Task childTask in task.Tasks)
				{
					DrawTask(childTask, isUtilitySelector);
				}
			}

			_depth--;
		}

		#endregion
	}
}
#endif