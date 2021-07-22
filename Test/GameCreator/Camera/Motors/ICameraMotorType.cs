namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Core;

    [AddComponentMenu("")]
	[System.Serializable]
    public class ICameraMotorType : MonoBehaviour
	{
        public enum Projection
        {
            Orthographic = 0,
            Perspective = 1
        }

        public static string NAME = "ICameraMotor";

		// PROPERTIES: ----------------------------------------------------------------------------

		protected CameraMotor cameraMotor;

        public bool setCameraProperties = false;
        public Projection projection = Projection.Perspective;

        public float cameraSize = 5.0f;
        [Range(1f, 179f)] public float fieldOfView = 60f;

        // INITIALIZE: ----------------------------------------------------------------------------

        public void Initialize(CameraMotor cameraMotor)
		{
			this.cameraMotor = cameraMotor;
		}

        private void Update()
        {
            this.UpdateMotor();
        }

        // ABSTRACT AND VIRTUAL METHODS: ----------------------------------------------------------

        public virtual void EnableMotor()  { return; }
        public virtual void DisableMotor() { return; }
		public virtual void UpdateMotor()  { return; }

		public virtual Vector3 GetPosition(CameraController camera, bool withoutSmoothing = false) 
		{ 
			return this.cameraMotor.transform.position; 
		}

		public virtual Vector3 GetDirection(CameraController camera, bool withoutSmoothing = false) 
		{
            return this.transform.TransformDirection(Vector3.forward);
		}

        public virtual bool UseSmoothPosition()
        {
            return true;
        }

        public virtual bool UseSmoothRotation()
        {
            return true;
        }
	}
}