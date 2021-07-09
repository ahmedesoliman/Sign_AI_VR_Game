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
	public class IActionsList : MonoBehaviour 
	{
		public class ActionCoroutine
		{
			public Coroutine coroutine {get; private set;}
			public object result {get; private set;}
			private IEnumerator target;

            public ActionCoroutine(IEnumerator target) 
			{
				this.target = target;
                this.coroutine = CoroutinesManager.Instance.StartCoroutine(Run());
			}

			private IEnumerator Run() 
			{
				while (this.target.MoveNext()) 
				{
					this.result = this.target.Current;
					yield return this.result;
				}
			}

            public void Stop()
            {
                CoroutinesManager.Instance.StopCoroutine(this.coroutine);
            }
        }

		// PROPERTIES: ----------------------------------------------------------------------------

		public IAction[] actions  = new IAction[0];
		public int executingIndex = -1;
		public bool isExecuting   = false;

        private ActionCoroutine actionCoroutine;
        private bool cancelExecution = false;

		// CONSTRUCTORS: --------------------------------------------------------------------------

		#if UNITY_EDITOR
		private void Awake()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

        private void OnEnable()
        {
            this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            SerializedProperty spActions = null;
            for (int i = 0; i < this.actions.Length; ++i)
            {
                IAction action = this.actions[i];
                if (action != null && action.gameObject != this.gameObject)
                {
                    IAction newAction = gameObject.AddComponent(action.GetType()) as IAction;
                    EditorUtility.CopySerialized(action, newAction);

                    if (spActions == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spActions = serializedObject.FindProperty("actions");
                    }

                    spActions.GetArrayElementAtIndex(i).objectReferenceValue = newAction;
                }
            }

            if (spActions != null) spActions.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        #endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Execute(GameObject target, System.Action callback, params object[] parameters)
		{
			this.isExecuting = true;
            CoroutinesManager.Instance.StartCoroutine(
                this.ExecuteCoroutine(target, callback, parameters)
            );
		}

        public IEnumerator ExecuteCoroutine(GameObject target, System.Action callback, params object[] parameters)
		{
            this.isExecuting = true;
            this.cancelExecution = false;

            for (int i = 0; i < this.actions.Length && !this.cancelExecution; ++i)
			{
				if (this.actions[i] == null) continue;

                this.executingIndex = i;

                if (!this.actions[i].InstantExecute(target, this.actions, i, parameters))
                {
                    this.actionCoroutine = new ActionCoroutine(
                        this.actions[i].Execute(target, this.actions, i, parameters)
                    );

                    yield return this.actionCoroutine.coroutine;

                    if (this.actionCoroutine == null || this.actionCoroutine.result == null)
                    {
                        yield break;
                    }

                    if (this.actionCoroutine.result is int)
                    {
                        i += (int)this.actionCoroutine.result;
                    }
                }

                if (i >= this.actions.Length) break;
                if (i < 0) i = -1;
			}

			this.executingIndex = -1;
			this.isExecuting = false;

			if (callback != null) callback();
		}

        public void Cancel()
        {
            if (!this.isExecuting) return;
            this.cancelExecution = true;
        }

        public void Stop()
        {
            this.Cancel();
            if (!this.isExecuting) return;

            this.actions[this.executingIndex].Stop();
            this.executingIndex = 0;
        }
    }
}