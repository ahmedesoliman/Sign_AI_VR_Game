namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[ExecuteInEditMode]
	public abstract class IAction : MonoBehaviour 
	{
        public virtual bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            return false;
        }

        public virtual bool InstantExecute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            return this.InstantExecute(target, actions, index);
        }

        public virtual IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            yield return 0;
        }

        public virtual IEnumerator Execute(GameObject target, IAction[] actions, int index, params object[] parameters)
        {
            IEnumerator execute = this.Execute(target, actions, index);
            object result = null;

            while (execute.MoveNext())
            {
                result = execute.Current;
                yield return result;
            }
        }

        public virtual void Stop()
        {
            return;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        // PROPERTIES: ----------------------------------------------------------------------------

        public static string NAME = "General/Empty Action";

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

		public void OnEnableEditor(UnityEngine.Object action)
		{
			this.serializedObject = new SerializedObject(action);
			this.serializedObject.Update();

			this.OnEnableEditorChild();
		}

		public void OnDisableEditor()
		{
			this.serializedObject = null;
			this.OnDisableEditorChild();
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

        public virtual float GetOpacity()
        {
            return 1.0f;
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

        protected virtual void OnEnableEditorChild() { }
        protected virtual void OnDisableEditorChild() { }

		#endif
	}
}