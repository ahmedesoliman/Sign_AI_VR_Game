namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [AddComponentMenu("")]
    public class GlobalVariablesManager : Singleton<GlobalVariablesManager>, IGameSave
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<string, Variable> variables;
        private bool igamesaveInitialized = false;

        // INITIALIZERS: --------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnLoad()
        {
            GlobalVariablesManager.Instance.WakeUp();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this.RequireInit();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(string name)
        {
            this.RequireInit();
            if (this.variables.ContainsKey(name))
            {
                return this.variables[name];
            }

            return null;
        }

        public string[] GetNames()
        {
            this.RequireInit();

            string[] names = new string[this.variables.Count];
            int index = 0;

            foreach (KeyValuePair<string, Variable> item in this.variables)
            {
                names[index] = item.Key;
                index += 1;
            }

            return names;
        }

        private void RequireInit(bool force = false)
        {
            if (this.variables != null && !force) return;

            if (!this.igamesaveInitialized)
            {
                SaveLoadManager.Instance.Initialize(this);
                this.igamesaveInitialized = true;
            }

            DatabaseVariables database = IDatabase.LoadDatabaseCopy<DatabaseVariables>();
            GlobalVariables globalVariables = database.GetGlobalVariables();

            this.variables = new Dictionary<string, Variable>();
            if (globalVariables == null) return;

            for (int i = 0; i < globalVariables.references.Length; ++i)
            {
                Variable variable = Instantiate(globalVariables.references[i]).variable;

                if (variable == null) continue;
                string variableName = variable.name;

                if (!this.variables.ContainsKey(variableName))
                {
                    this.variables.Add(variableName, variable);
                }
            }
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string GetUniqueName()
        {
            return "variables:global";
        }

        public Type GetSaveDataType()
        {
            return typeof(DatabaseVariables.Container);
        }

        public object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();

            foreach (KeyValuePair<string, Variable> item in this.variables)
            {
                if (item.Value.CanSave() && item.Value.save)
                {
                    container.variables.Add(item.Value);
                }
            }

            return container;
        }

        public void ResetData()
        {
            this.RequireInit(true);
        }

        public void OnLoad(object generic)
        {
            this.RequireInit();

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
                        VariablesManager.events.OnChangeGlobal(varName);
                    }
                }
            }
        }
    }
}