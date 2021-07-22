namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionAnimatorLayer : IAction 
	{
		public Animator animator;
		public int layerIndex = 1;
		[Range(0.0f, 1.0f)] public float weight = 0.0f;

		// EXECUTABLE: ----------------------------------------------------------------------------
		
        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            this.animator.SetLayerWeight(this.layerIndex, weight);
            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Animation/Animator Layer";
		private const string NODE_TITLE = "Change {0}'s layer {1} to weight {2}";

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spAnimator;
		private SerializedProperty spLayerIndex;
		private SerializedProperty spWeight;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return string.Format(NODE_TITLE, 
				(this.animator == null ? "nothing" : this.animator.gameObject.name),
				this.layerIndex, this.weight
			);
		}

		protected override void OnEnableEditorChild()
		{
			this.spAnimator = this.serializedObject.FindProperty("animator");
			this.spLayerIndex = this.serializedObject.FindProperty("layerIndex");
			this.spWeight = this.serializedObject.FindProperty("weight");
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.spAnimator);
			EditorGUILayout.PropertyField(this.spLayerIndex);
			EditorGUILayout.PropertyField(this.spWeight);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}