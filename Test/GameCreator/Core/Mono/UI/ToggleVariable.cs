namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor.Events;
    #endif

    [AddComponentMenu("Game Creator/UI/Toggle", 10)]
    public class ToggleVariable : Toggle
    {
        [VariableFilter(Variable.DataType.Bool)]
        public VariableProperty variable = new VariableProperty();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            EventSystemManager.Instance.Wakeup();
        }

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;

            object current = this.variable.Get(gameObject);

            if (current != null)
            {
                this.isOn = (bool)current;
                this.onValueChanged.AddListener(this.SyncVariableWithValue);
            }

            switch (this.variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        SyncValueWithVariable,
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        SyncValueWithVariable,
                        this.variable.GetLocalVariableGameObject(gameObject),
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        SyncValueWithList,
                        this.variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!Application.isPlaying) return;

            switch (this.variable.variableType)
            {
                case VariableProperty.GetVarType.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        SyncValueWithVariable,
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        SyncValueWithVariable,
                        this.variable.GetLocalVariableGameObject(gameObject),
                        this.variable.GetVariableID()
                    );
                    break;

                case VariableProperty.GetVarType.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        SyncValueWithList,
                        this.variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SyncValueWithVariable(string variableName)
        {
            object current = this.variable.Get(gameObject);
            if (current != null)
            {
                bool variableValue = (bool)current;
                this.isOn = variableValue;
            }
        }

        private void SyncValueWithList(int index, object prevElem, object newElem)
        {
            this.SyncValueWithVariable(string.Empty);
        }

        private void SyncVariableWithValue(bool state)
        {
            this.variable.Set(state, gameObject);
        }
    }
}