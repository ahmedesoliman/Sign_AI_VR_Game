using System;
using UnityEngine;

namespace GameCreator.Core
{
    [AddComponentMenu("Game Creator/Remember/Remember Transform")]
    public class RememberTransform : RememberBase
    {
        [Serializable]
        public class Memory
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool rememberPosition = true;
        public bool rememberRotation = true;
        public bool rememberScale = true;

        // IGAMESAVE: -----------------------------------------------------------------------------

        public override object GetSaveData()
        {
            Memory memory = new Memory()
            {
                position = transform.position,
                rotation = transform.rotation,
                scale = transform.localScale
            };

            Debug.Log("Saving data: " + memory.scale);
            return memory;
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

            Debug.Log("On Load Memory Transform: " + memory.scale);

            if (this.rememberPosition) transform.position = memory.position;
            if (this.rememberRotation) transform.rotation = memory.rotation;
            if (this.rememberScale) transform.localScale = memory.scale;
        }

        public override void ResetData()
        { }
    }
}
