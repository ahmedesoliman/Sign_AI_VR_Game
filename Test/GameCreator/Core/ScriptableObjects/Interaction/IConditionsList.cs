namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [ExecuteInEditMode][AddComponentMenu("")]
	public class IConditionsList : MonoBehaviour 
	{
		// PROPERTIES: ----------------------------------------------------------------------------

        public ICondition[] conditions  = new ICondition[0];

        // CONSTRUCTORS: --------------------------------------------------------------------------

        #if UNITY_EDITOR
        private void Awake()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

        private void OnEnable()
        {
            this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            SerializedProperty spConditions = null;
            for (int i = 0; i < this.conditions.Length; ++i)
            {
                ICondition condition = this.conditions[i];
                if (condition != null && condition.gameObject != this.gameObject)
                {
                    ICondition newCondition = gameObject.AddComponent(condition.GetType()) as ICondition;
                    EditorUtility.CopySerialized(condition, newCondition);

                    if (spConditions == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spConditions = serializedObject.FindProperty("conditions");
                    }

                    spConditions.GetArrayElementAtIndex(i).objectReferenceValue = newCondition;
                }
            }

            if (spConditions != null) spConditions.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        #endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Check(GameObject invoker = null, params object[] parameters)
        {
            if (this.conditions == null) return true;

            for (int i = 0; i < this.conditions.Length; ++i)
            {
                if (this.conditions[i] == null) continue;
                if (!this.conditions[i].Check(invoker, parameters)) return false;
            }

            return true;
        }
	}
}