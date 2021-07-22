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

    [AddComponentMenu("Game Creator/UI/Slider", 10)]
    public class SliderVariable : Slider
    {
        [VariableFilter(Variable.DataType.Number)]
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
                this.value = (float)current;
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

        private void SyncValueWithList(int index, object prevElem, object newElem)
        {
            this.SyncValueWithVariable(string.Empty);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual void SyncValueWithVariable(string variableName)
        {
            object current = this.variable.Get(gameObject);
            if (current != null)
            {
                float newValue = (float)current;
                if (!Mathf.Approximately(newValue, this.value))
                {
                    this.value = newValue;
                }
            }
        }

        protected virtual void SyncVariableWithValue(float newValue)
        {
            this.variable.Set(newValue, gameObject);
        }
    }
}