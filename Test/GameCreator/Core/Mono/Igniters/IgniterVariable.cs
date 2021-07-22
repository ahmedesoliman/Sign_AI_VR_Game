namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;

	[AddComponentMenu("")]
	public class IgniterVariable : Igniter 
	{
        public VariableProperty variable = new VariableProperty();

		#if UNITY_EDITOR
        public new static string NAME = "Variables/On Variable Change";
        #endif

        private void Start()
        {
            switch (this.variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        this.OnVariableChange,
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        this.OnVariableChange,
                        this.variable.GetLocalVariableGameObject(gameObject),
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        this.OnListChange,
                        this.variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        private void OnVariableChange(string variableID)
        {
            this.ExecuteTrigger(gameObject);
        }

        private void OnListChange(int index, object prevElem, object newElem)
        {
            this.ExecuteTrigger(gameObject);
        }

        private void OnDestroy()
        {
            switch (this.variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        this.OnVariableChange,
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        this.OnVariableChange,
                        this.variable.GetLocalVariableGameObject(gameObject),
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        this.OnListChange,
                        this.variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }
    }
}