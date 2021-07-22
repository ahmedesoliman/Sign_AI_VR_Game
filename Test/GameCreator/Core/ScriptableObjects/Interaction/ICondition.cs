namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	public abstract class ICondition : MonoBehaviour 
	{
        public virtual bool Check()
		{
			return true;
		}

        public virtual bool Check(GameObject target)
        {
            return this.Check();
        }

        public virtual bool Check(GameObject target, params object[] parameters)
        {
            return this.Check(target);
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		// PROPERTIES: ----------------------------------------------------------------------------

		public static string NAME = "General/Empty Condition";
		protected SerializedObject serializedObject;
        public bool isExpanded = false;

		// METHODS: -------------------------------------------------------------------------------
        
		private void Awake()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		private void OnEnable()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		private void OnValidate()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		public void OnEnableEditor(UnityEngine.Object condition)
		{
			this.serializedObject = new SerializedObject(condition);
			this.serializedObject.Update();

			this.OnEnableEditorChild();
		}

        public void OnInspectorGUIEditor()
        {
            if (this.serializedObject == null) return;
            this.OnInspectorGUI();
        }

        // VIRTUAL AND ABSTRACT METHODS: ----------------------------------------------------------

        public virtual string GetNodeTitle()
        {
            return this.GetType().Name;
        }

        public virtual void OnInspectorGUI()
        {
            if (this.serializedObject.targetObject != null)
            {
                this.serializedObject.Update();
                SerializedProperty iterator = this.serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if ("m_Script" == iterator.propertyPath) continue;
                    if ("isExpanded" == iterator.propertyPath) continue;
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }

                this.serializedObject.ApplyModifiedProperties();
            }
        }

        protected virtual void OnEnableEditorChild()  {}
        protected virtual void OnDisableEditorChild() {}

		#endif
	}
}