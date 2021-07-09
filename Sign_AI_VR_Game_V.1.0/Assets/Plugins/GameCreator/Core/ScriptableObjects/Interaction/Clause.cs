namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Events;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
    public class Clause : MonoBehaviour 
	{
		public string description = "Clause";
        public IConditionsList conditionsList;
		public Actions actions;

        public bool isExpanded = false;

        // EVENTS: --------------------------------------------------------------------------------

        public UnityEvent onCheckConditions = new UnityEvent();
        public UnityEvent onExecuteActions = new UnityEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        #if UNITY_EDITOR
        private void Awake()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		private void OnEnable()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            if (this.conditionsList != null &&
                this.conditionsList.gameObject != this.gameObject)
            {
                IConditionsList newConditionsList = gameObject.AddComponent<IConditionsList>();
                EditorUtility.CopySerialized(this.conditionsList, newConditionsList);

                SerializedObject serializedObject = new SerializedObject(this);
                serializedObject.FindProperty("conditionsList").objectReferenceValue = newConditionsList;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private void OnValidate()
        {
            this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }
        #endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual bool CheckConditions(GameObject target = null, params object[] parameters)
		{
            if (this.onCheckConditions != null) this.onCheckConditions.Invoke();
            return this.conditionsList.Check(target, parameters);
		}

        public virtual void ExecuteActions(GameObject target = null, params object[] parameters)
		{
			if (this.actions != null) 
			{
                if (this.onExecuteActions != null) this.onExecuteActions.Invoke();
                this.actions.Execute(target, parameters);
			}
		}
	}
}