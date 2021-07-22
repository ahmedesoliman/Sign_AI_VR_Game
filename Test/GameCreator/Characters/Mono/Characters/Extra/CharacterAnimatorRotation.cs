namespace GameCreator.Characters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class CharacterAnimatorRotation
    {
        private const float ROTATION_SMOOTH = 0.1f;

        [Serializable]
        private class AnimFloat
        {
            public float target { get; private set; }
            public float value { get; private set; }
            private float time;

            public AnimFloat(float value)
            {
                this.target = value;
                this.value = value;
                this.time = 0f;
            }

            public float Update()
            {
                float t = (Time.time - this.time) / ROTATION_SMOOTH;
                this.value = Mathf.LerpAngle(this.value, this.target, t);

                return this.value;
            }

            public void SetTarget(float value)
            {
                this.target = value;
                this.time = Time.time;
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private readonly AnimFloat x = new AnimFloat(0f);
        private readonly AnimFloat y = new AnimFloat(0f);
        private readonly AnimFloat z = new AnimFloat(0f);

        // UPDATER: -------------------------------------------------------------------------------

        public Quaternion Update()
        {
            Quaternion rotation = Quaternion.Euler(
                this.x.Update(),
                this.y.Update(),
                this.z.Update()
            );

            return rotation;
        }

        // PUBLIC GETTERS: ------------------------------------------------------------------------

        public Quaternion GetCurrentRotation()
        {
            return Quaternion.Euler(this.x.value, this.y.value, this.z.value);
        }

        public Quaternion GetTargetRotation()
        {
            return Quaternion.Euler(this.x.target, this.y.target, this.z.target);
        }

        // PUBLIC SETTERS: ------------------------------------------------------------------------

        public void SetPitch(float value)
        {
            this.x.SetTarget(value);
        }

        public void SetYaw(float value)
        {
            this.y.SetTarget(value);
        }

        public void SetRoll(float value)
        {
            this.z.SetTarget(value);
        }

        public void SetQuaternion(Quaternion rotation)
        {
            this.SetPitch(rotation.eulerAngles.x);
            this.SetYaw(rotation.eulerAngles.y);
            this.SetRoll(rotation.eulerAngles.z);
        }
    }
}