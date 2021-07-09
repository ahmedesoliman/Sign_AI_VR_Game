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

    [AddComponentMenu("Game Creator/UI/Slider Vector", 10)]
    public class SliderVectorVariable : Slider
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        [VariableFilter(Variable.DataType.Vector2, Variable.DataType.Vector3)]
        public VariableProperty variable = new VariableProperty();

        public Axis component = Axis.X;

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
                Variable.DataType dataType = this.variable.GetVariableDataType(gameObject);
                switch (dataType)
                {
                    case Variable.DataType.Vector2:
                        if (this.component == Axis.X) this.value = ((Vector2)current).x;
                        if (this.component == Axis.Y) this.value = ((Vector2)current).y;
                        break;

                    case Variable.DataType.Vector3:
                        if (this.component == Axis.X) this.value = ((Vector3)current).x;
                        if (this.component == Axis.Y) this.value = ((Vector3)current).y;
                        if (this.component == Axis.Z) this.value = ((Vector3)current).z;
                        break;
                }

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

        protected void SyncValueWithVariable(string variableName)
        {
            object current = this.variable.Get(gameObject);
            if (current != null)
            {
                Variable.DataType dataType = this.variable.GetVariableDataType(gameObject);
                switch (dataType)
                {
                    case Variable.DataType.Vector2:
                        Vector2 newValue2 = (Vector2) current;
                        if (this.component == Axis.X) this.value = newValue2.x;
                        if (this.component == Axis.Y) this.value = newValue2.y;
                        break;

                    case Variable.DataType.Vector3:
                        Vector3 newValue3 = (Vector3)current;
                        if (this.component == Axis.X) this.value = newValue3.x;
                        if (this.component == Axis.Y) this.value = newValue3.y;
                        if (this.component == Axis.Z) this.value = newValue3.z;
                        break;
                }
            }
        }

        protected void SyncVariableWithValue(float newValue)
        {
            Variable.DataType dataType = this.variable.GetVariableDataType(gameObject);
            switch (dataType)
            {
                case Variable.DataType.Vector2:
                    Vector2 vec2 = (Vector2)this.variable.Get(gameObject);
                    if (this.component == Axis.X) vec2.x = newValue;
                    if (this.component == Axis.Y) vec2.y = newValue;
                    this.variable.Set(vec2, gameObject);
                    break;

                case Variable.DataType.Vector3:
                    Vector3 vec3 = (Vector3)this.variable.Get(gameObject);
                    if (this.component == Axis.X) vec3.x = newValue;
                    if (this.component == Axis.Y) vec3.y = newValue;
                    if (this.component == Axis.Z) vec3.z = newValue;
                    this.variable.Set(vec3, gameObject);
                    break;
            }
        }
    }
}