namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using GameCreator.Core;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("Game Creator/Local Variables")]
    public class LocalVariables : GlobalID, IGameSave
    {
        public static Dictionary<string, LocalVariables> REGISTER = new Dictionary<string, LocalVariables>();

        // PROPERTIES: ----------------------------------------------------------------------------

        public MBVariable[] references = new MBVariable[0];

        protected bool initalized = false;
        private Dictionary<string, Variable> variables;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(string name)
        {
            this.Initialize();
            if (this.variables != null && this.variables.ContainsKey(name))
            {
                return this.variables[name];
            }

            return null;
        }

        // INITIALIZERS: --------------------------------------------------------------------------

		protected virtual void Start()
		{
            if (!Application.isPlaying) return;
            this.Initialize();
            SaveLoadManager.Instance.Initialize(this);
		}

		protected void Initialize(bool force = false)
		{
            string gid = this.GetID();
            if (!REGISTER.ContainsKey(gid))
            {
                REGISTER.Add(gid, this);
            }

            this.RequireInit(force);
		}

        protected virtual void OnDestroy()
		{
            this.OnDestroyGID();
            if (!Application.isPlaying) return;
            if (this.exitingApplication) return;

            string gid = this.GetID();
            if (REGISTER.ContainsKey(gid))
            {
                REGISTER.Remove(gid);
            }

            if (SaveLoadManager.IS_EXITING) return;
            SaveLoadManager.Instance.OnDestroyIGameSave(this);
		}

        #if UNITY_EDITOR
        private void Reset()
        {
            SerializedProperty spReferences = null;
            for (int i = 0; i < this.references.Length; ++i)
            {
                MBVariable reference = this.references[i];
                if (reference != null && reference.gameObject != this.gameObject)
                {
                    MBVariable newVariable = gameObject.AddComponent<MBVariable>();
                    EditorUtility.CopySerialized(reference, newVariable);

                    if (spReferences == null)
                    {
                        SerializedObject serializedObject = new SerializedObject(this);
                        spReferences = serializedObject.FindProperty("references");
                    }

                    spReferences.GetArrayElementAtIndex(i).objectReferenceValue = newVariable;
                }
            }

            if (spReferences != null) spReferences.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        #endif

        // PRIVATE METHODS: -----------------------------------------------------------------------

        protected virtual void RequireInit(bool force = false)
        {
            if (this.initalized && !force) return;
            this.initalized = true;

            this.variables = new Dictionary<string, Variable>();
            for (int i = 0; i < this.references.Length; ++i)
            {
                Variable variable = this.references[i].variable;
                if (variable == null) continue;

                string variableName = variable.name;
                if (!this.variables.ContainsKey(variableName))
                {
                    this.variables.Add(variableName, new Variable(variable));
                }
            }
        }

		// IGAMESAVE: -----------------------------------------------------------------------------

		public virtual string GetUniqueName()
        {
            string uniqueName = string.Format(
                "variables:local:{0}",
                this.GetID()
            );

            return uniqueName;
        }

        public Type GetSaveDataType()
        {
            return typeof(DatabaseVariables.Container);
        }

        public virtual object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();
            if (this.variables == null || this.variables.Count == 0)
            {
                return container;
            }

            foreach (KeyValuePair<string, Variable> item in this.variables)
            {
                if (item.Value != null && item.Value.CanSave() && item.Value.save)
                {
                    container.variables.Add(item.Value);
                }
            }

            return container;
        }

        public virtual void ResetData()
        {
            this.RequireInit(true);
        }

        public virtual void OnLoad(object generic)
        {
            if (generic == null) return;

            DatabaseVariables.Container container = (DatabaseVariables.Container)generic;
            int variablesContainerCount = container.variables.Count;

            for (int i = 0; i < variablesContainerCount; ++i)
            {
                Variable variablesContainerVariable = container.variables[i];
                string varName = variablesContainerVariable.name;

                if (this.variables.ContainsKey(varName) && this.variables[varName].CanSave() && 
                    this.variables[varName].save)
                {
                    if (this.variables[varName].Get() != variablesContainerVariable.Get())
                    {
                        this.variables[varName] = variablesContainerVariable;
                        VariablesManager.events.OnChangeLocal(gameObject, varName);
                    }
                }
            }
        }
    }
}