namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public abstract class Igniter : MonoBehaviour 
	{
        private static readonly object[] NO_OBJECT = new object[0];

		// PROPERTIES: ----------------------------------------------------------------------------

		[HideInInspector][SerializeField] private Trigger trigger;
        protected bool isExitingApplication = false;

		// INITIALIZER: ---------------------------------------------------------------------------

		public virtual void Setup(Trigger trigger)
		{
			this.trigger = trigger;
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		protected virtual void Awake()
		{
            #if UNITY_EDITOR
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            #endif
		}

		protected void OnEnable()
		{
            #if UNITY_EDITOR
            this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            #endif
		}

		protected void OnValidate()
		{
            #if UNITY_EDITOR
            this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            #endif
		}

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void ExecuteTrigger()
        {
            this.ExecuteTrigger(null);
        }

        protected virtual void ExecuteTrigger(GameObject target)
        {
            this.ExecuteTrigger(target, NO_OBJECT);
        }

        protected virtual void ExecuteTrigger(GameObject target, params object[] parameters)
        {
            if (target == null) target = gameObject;
            if (this.trigger == null) return;
            this.trigger.Execute(target, parameters);
        }

        protected virtual void OnApplicationQuit()
        {
            this.isExitingApplication = true;
        }

        // PROTECTED UTILITY METHODS: -------------------------------------------------------------

        protected bool IsColliderPlayer(Collider c)
        {
            int cInstanceID = c.gameObject.GetInstanceID();
            if (HookPlayer.Instance != null && 
                HookPlayer.Instance.gameObject.GetInstanceID() == cInstanceID)
            {
                return true;
            }

            return false;
        }

		// VIRTUAL EDITOR METHODS: ----------------------------------------------------------------

		#if UNITY_EDITOR

		public static string NAME = "Never";
		public static string COMMENT = "";
		public static bool REQUIRES_COLLIDER = false;
		public static string ICON_PATH = "Assets/Plugins/GameCreator/Extra/Icons/Igniters/";

		public static bool PaintEditor(SerializedObject serialObject)
		{
			EditorGUI.BeginChangeCheck();
            if (serialObject.targetObject != null)
            {
                serialObject.Update();
                SerializedProperty iterator = serialObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if ("m_Script" == iterator.propertyPath) continue;
                    EditorGUILayout.PropertyField(iterator, true);
                }

                serialObject.ApplyModifiedProperties();
            }

			return EditorGUI.EndChangeCheck();
		}

		#endif
	}
}