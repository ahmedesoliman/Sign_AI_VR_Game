using System;
using UnityEngine;

namespace GameCreator.Core
{
    [AddComponentMenu("Game Creator/Remember/Remember Active")]
    public class RememberActive : RememberBase
    {
        public enum State
        {
            Active,
            Inactive,
            Destroyed
        }

        [Serializable]
        public class Memory
        {
            public State state;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private bool defaultState;
        private State state = State.Active;

        // METHODS: -------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying || this.exitingApplication) return;
            this.defaultState = this.gameObject.activeSelf;
        }

        private void OnEnable()
        {
            if (!Application.isPlaying || this.exitingApplication) return;
            this.UpdateState();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying || this.exitingApplication) return;
            this.UpdateState();
        }

        private void UpdateState()
        {
            switch (this.gameObject.activeSelf)
            {
                case true: this.state = State.Active; break;
                case false: this.state = State.Inactive; break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (!Application.isPlaying || this.exitingApplication) return;
            this.state = State.Destroyed;
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public override object GetSaveData()
        {
            return new Memory()
            {
                state = this.state
            };
        }

        public override Type GetSaveDataType()
        {
            return typeof(Memory);
        }

        public override string GetUniqueName()
        {
            return this.GetID();
        }

        public override void OnLoad(object generic)
        {
            Memory memory = generic as Memory;
            if (memory == null || this.isDestroyed) return;

            switch (memory.state)
            {
                case State.Active: this.gameObject.SetActive(true); break;
                case State.Inactive: this.gameObject.SetActive(false); break;
                case State.Destroyed: Destroy(this.gameObject); break;
            }
        }

        public override void ResetData()
        {
            this.gameObject.SetActive(this.defaultState);
        }
    }
}