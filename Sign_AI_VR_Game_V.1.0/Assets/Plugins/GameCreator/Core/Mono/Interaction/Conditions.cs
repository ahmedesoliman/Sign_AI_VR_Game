namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[ExecuteInEditMode]
    [AddComponentMenu("Game Creator/Conditions", 0)]
    public class Conditions : MonoBehaviour 
	{
        public Clause[] clauses = new Clause[0];
		public Actions defaultActions;

        // EVENTS: --------------------------------------------------------------------------------

        public UnityEvent onInteract = new UnityEvent();

		// INTERACT METHOD: -----------------------------------------------------------------------

        public virtual void Interact()
        {
            this.Interact(null, new object[0]);
        }

        public virtual void Interact(GameObject target, params object[] parameters)
		{
            if (this.onInteract != null) this.onInteract.Invoke();
			for (int i = 0; i < this.clauses.Length; ++i)
			{
                if (this.clauses[i].CheckConditions(target, parameters))
				{
                    this.clauses[i].ExecuteActions(target, parameters);
					return;
				}
			}

			if (this.defaultActions != null) 
			{
				this.defaultActions.Execute(target);
			}
		}

        public virtual IEnumerator InteractCoroutine(GameObject target = null)
        {
            for (int i = 0; i < this.clauses.Length; ++i)
            {
                if (this.clauses[i].CheckConditions(target))
                {
                    Actions actions = this.clauses[i].actions;
                    if (actions != null)
                    {
                        Coroutine coroutine = CoroutinesManager.Instance.StartCoroutine(
                            actions.actionsList.ExecuteCoroutine(target, null)
                        );

                        yield return coroutine;
                    }

                    yield break;
                }
            }

            if (this.defaultActions != null)
            {
                Coroutine coroutine = CoroutinesManager.Instance.StartCoroutine(
                    this.defaultActions.actionsList.ExecuteCoroutine(target, null)
                );

                yield return coroutine;
                yield break;
            }
        }

        #if UNITY_EDITOR
        private void OnEnable()
        {
            SerializedProperty spClauses = null;
            for (int i = 0; i < this.clauses.Length; ++i)
            {
                Clause clause = this.clauses[i];
                if (clause != null && clause.gameObject != this.gameObject)
                {
                    Clause newClause = gameObject.AddComponent(clause.GetType()) as Clause;
                    EditorUtility.CopySerialized(clause, newClause);

                    if (spClauses == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spClauses = serializedObject.FindProperty("clauses");
                    }

                    spClauses.GetArrayElementAtIndex(i).objectReferenceValue = newClause;
                }
            }

            if (spClauses != null) spClauses.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        #endif

        // GIZMO METHODS: -------------------------------------------------------------------------

        private void OnDrawGizmos()
		{
            int numClauses = (this.clauses == null ? 0 : this.clauses.Length);
            switch (numClauses)
            {
                case 0:  Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions0", true); break;
                case 1:  Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions1", true); break;
                case 2:  Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions2", true); break;
                default: Gizmos.DrawIcon(transform.position, "GameCreator/Conditions/conditions3", true); break;
            }
		}
	}
}