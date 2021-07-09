namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class VariableProperty
    {
        public enum GetVarType
        {
            GlobalVariable,
            LocalVariable,
            ListVariable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public GetVarType variableType = GetVarType.GlobalVariable;

        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableProperty()
        {
			this.SetupVariables();
		}

        public VariableProperty(Variable.VarType variableType)
        {
			this.SetupVariables();
            switch (variableType)
            {
                case Variable.VarType.GlobalVariable: 
                    this.variableType = GetVarType.GlobalVariable;
                    break;

                case Variable.VarType.LocalVariable: 
                    this.variableType = GetVarType.LocalVariable;
                    break;

                case Variable.VarType.ListVariable:
                    this.variableType = GetVarType.ListVariable;
                    break;
            }
        }

        public VariableProperty(GetVarType variableType)
        {
            this.SetupVariables();
            this.variableType = variableType;
        }

        private void SetupVariables()
        {
            this.global = this.global ?? new HelperGlobalVariable();
            this.local = this.local ?? new HelperLocalVariable();
            this.list = this.list ?? new HelperGetListVariable();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get(GameObject invoker = null)
        {
            switch (this.variableType)
            {
                case GetVarType.GlobalVariable: return this.global.Get(invoker);
                case GetVarType.LocalVariable: return this.local.Get(invoker);
                case GetVarType.ListVariable: return this.list.Get(invoker);
            }

            return null;
        }

        public void Set(object value, GameObject invoker = null)
        {
            switch (this.variableType)
            {
                case GetVarType.GlobalVariable: this.global.Set(value, invoker); break;
                case GetVarType.LocalVariable: this.local.Set(value, invoker); break;
                case GetVarType.ListVariable: this.list.Set(value, invoker); break;
            }
        }

        public string GetVariableID()
        {
            switch (this.variableType)
            {
                case GetVarType.GlobalVariable: return this.global.name;
                case GetVarType.LocalVariable: return this.local.name;
                case GetVarType.ListVariable: return this.list.select.ToString();
            }

            return "";
        }

        public Variable.VarType GetVariableType()
        {
            switch (this.variableType)
            {
                case GetVarType.GlobalVariable: return Variable.VarType.GlobalVariable;
                case GetVarType.LocalVariable: return Variable.VarType.LocalVariable;
                case GetVarType.ListVariable: return Variable.VarType.ListVariable;
            }

            return Variable.VarType.GlobalVariable;
        }

        #region TEMPORAL_FIX

        private const string UPGRADE_WARN1 = "<b>Game Creator Warning:</b> Unhandled method upgrade. ";
        private const string UPGRADE_WARN2 = "Please, report this message to hello@gamecreator.io";

        // TODO: Remove in upgrade
        public GameObject GetLocalVariableGameObject()
        {
            Debug.LogWarning(UPGRADE_WARN1 + UPGRADE_WARN2);
            return this.GetLocalVariableGameObject(null);
        }

        // TODO: Remove in upgrade
        public GameObject GetListVariableGameObject()
        {
            Debug.LogWarning(UPGRADE_WARN1 + UPGRADE_WARN2);
            return this.GetListVariableGameObject(null);
        }

        #endregion

        public GameObject GetLocalVariableGameObject(GameObject invoker)
        {
            return this.local.GetGameObject(invoker);
        }

        public GameObject GetListVariableGameObject(GameObject invoker)
        {
            return this.list.GetGameObject(invoker);
        }

        public Variable.DataType GetVariableDataType(GameObject invoker)
        {
            switch (this.variableType)
            {
                case GetVarType.GlobalVariable: 
                    return this.global.GetDataType(null);

                case GetVarType.LocalVariable:
                    GameObject targetLocal = this.local.GetGameObject(invoker);
                    return this.local.GetDataType(targetLocal);

                case GetVarType.ListVariable:
                    GameObject targetList = this.local.GetGameObject(invoker);
                    return this.list.GetDataType(targetList);
            }

            return Variable.DataType.Null;
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override string ToString()
		{
            string varName = "";
            switch (this.variableType)
            {
                case GetVarType.GlobalVariable : varName = this.global.ToString(); break;
                case GetVarType.LocalVariable: varName = this.local.ToString(); break;
                case GetVarType.ListVariable: varName = this.list.ToString(); break;
            }

            return (string.IsNullOrEmpty(varName) ? "(unknown)" : varName);
		}

        public string ToStringValue(GameObject invoker)
        {
            switch (this.variableType)
            {
                case GetVarType.GlobalVariable: return this.global.ToStringValue(invoker);
                case GetVarType.LocalVariable: return this.local.ToStringValue(invoker);
                case GetVarType.ListVariable: return this.list.ToStringValue(invoker);
            }

            return "unknown";
        }
	}
}