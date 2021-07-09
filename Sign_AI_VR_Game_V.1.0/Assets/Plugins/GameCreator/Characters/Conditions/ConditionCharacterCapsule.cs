namespace GameCreator.Characters
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
	public class ConditionCharacterCapsule : ICondition
	{
        private const float OFFSET = 0.01f;

        public TargetCharacter character = new TargetCharacter();

        public float height = 1.0f;
        public float radius = 0.3f;
        public LayerMask layerMask = -1;

        private Collider[] colliderBuffer = new Collider[50];

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override bool Check(GameObject target)
		{
            Character charTarget = this.character.GetCharacter(target);
            if (charTarget == null) return false;

            float minHeight = Mathf.Max(0.0f, this.height - this.radius * 2.0f);
            Vector3 point1 = charTarget.transform.position + (Vector3.up * (this.radius + OFFSET));
            Vector3 point2 = point1 + (Vector3.up * minHeight);

            int colliderCount = Physics.OverlapCapsuleNonAlloc(
                point1,
                point2,
                this.radius,
                this.colliderBuffer,
                this.layerMask,
                QueryTriggerInteraction.Ignore
            );

            Debug.DrawLine(
                point1,
                point2,
                new Color(256f, 0f, 0f, 1f),
                1.0f
            );

            Debug.DrawLine(
                point2,
                point2 + Vector3.up * this.radius,
                new Color(256f, 0f, 0f, 0.25f),
                1.0f
            );
                
            for (int i = 0; i < colliderCount; ++i)
            {
                if (this.colliderBuffer[i].gameObject.GetInstanceID() != charTarget.gameObject.GetInstanceID() &&
                    !this.colliderBuffer[i].transform.IsChildOf(charTarget.transform))
                {
                    return false;
                }
            }

            return true;
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

	    public static new string NAME = "Characters/Character Capsule";
        private const string NODE_TITLE = "Character capsule fits";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spCharacter;
        private SerializedProperty spHeight;
        private SerializedProperty spRadius;
        private SerializedProperty spLayerMask;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
            this.spCharacter = this.serializedObject.FindProperty("character");
            this.spHeight = this.serializedObject.FindProperty("height");
            this.spRadius = this.serializedObject.FindProperty("radius");
            this.spLayerMask = this.serializedObject.FindProperty("layerMask");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spCharacter = null;
            this.spHeight = null;
            this.spRadius = null;
            this.spLayerMask = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spCharacter);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spHeight);
            EditorGUILayout.PropertyField(this.spRadius);
            EditorGUILayout.PropertyField(this.spLayerMask);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}
