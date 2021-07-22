namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using GameCreator.Core;
    using GameCreator.Localization;

    [AddComponentMenu("Game Creator/UI/Text (Variable)", 20)]
    public class TextVariable : Text
    {
        public string format = "Variable: {0}";
        public VariableProperty variable = new VariableProperty();

        private bool exitingApplication = false;

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                this.UpdateText();
                this.SubscribeOnChange();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                this.UpdateText();
                this.SubscribeOnChange();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying && !this.exitingApplication)
            {
                this.UpdateText();
                this.UnsubscribeOnChange();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDisable();
            if (Application.isPlaying && !this.exitingApplication)
            {
                this.UpdateText();
                this.UnsubscribeOnChange();
            }
        }

        private void OnApplicationQuit()
        {
            this.exitingApplication = true;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnUpdateVariable(string variableID)
        {
            if (string.IsNullOrEmpty(variableID)) return;
            if (this.variable.GetVariableID() != variableID) return;

            this.UpdateText();
        }

        private void OnUpdateList(int index, object prevElem, object newElem)
        {
            this.UpdateText();
        }

        private void UpdateText()
        {
            string value = this.format;
            if (this.format.Contains("{0}"))
            {
                value = string.Format(
                    this.format, 
                    this.variable.Get(gameObject)
                );
            }

            this.text = value;
        }

        private void SubscribeOnChange()
        {
            switch (this.variable.GetVariableType())
            {
                case Variable.VarType.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        this.OnUpdateVariable,
                        this.variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        this.OnUpdateVariable,
                        this.variable.GetLocalVariableGameObject(gameObject),
                        this.variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        this.OnUpdateList,
                        this.variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }

        private void UnsubscribeOnChange()
        {
            switch (this.variable.GetVariableType())
            {
                case Variable.VarType.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        this.OnUpdateVariable,
                        this.variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        this.OnUpdateVariable,
                        this.variable.GetLocalVariableGameObject(gameObject),
                        this.variable.GetVariableID()
                    );
                    break;

                case Variable.VarType.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        this.OnUpdateList,
                        this.variable.GetListVariableGameObject(gameObject)
                    );
                    break;
            }
        }
    }
}