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

    [AddComponentMenu("Game Creator/UI/Input Field", 10)]
    public class InputFieldVariable : InputField
    {
        [VariableFilter(Variable.DataType.String, Variable.DataType.Number)]
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
                this.text = (string)current;
                this.onValueChanged.AddListener(this.SyncVariable);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SyncVariable(string value)
        {
            this.variable.Set(value, gameObject);
        }
    }
}