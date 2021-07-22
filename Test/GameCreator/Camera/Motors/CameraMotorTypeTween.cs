namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeTween : ICameraMotorType 
	{
		private static readonly Vector3 CAMERA_END = new Vector3(5.0f,  0.0f, 0.0f);

		public static new string NAME = "Tween Camera";

		// PROPERTIES: ----------------------------------------------------------------------------

		public Vector3 cameraEndPoint = CAMERA_END;
		public float duration = 2.0f;
		public Easing.EaseType easing = Easing.EaseType.Linear;

        public TargetDirection lookAt = new TargetDirection(TargetDirection.Target.CurrentDirection);

		private float beginTime = 0.0f;
        private GameObject cameraHead;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.cameraHead = new GameObject("Motor Head");
            this.cameraHead.transform.SetParent(transform);
        }

        // UPDATE: --------------------------------------------------------------------------------

        public override void EnableMotor()
        {
            this.beginTime = Time.time;
            this.UpdateMotor();
        }

        public override void UpdateMotor()
        {
            float t = (Time.time - this.beginTime) / this.duration;
            Vector3 positionA = this.cameraMotor.transform.position;
            Vector3 positionB = this.cameraEndPoint;

            this.cameraHead.transform.position = new Vector3(
                Easing.GetEase(this.easing, positionA.x, positionB.x, t),
                Easing.GetEase(this.easing, positionA.y, positionB.y, t),
                Easing.GetEase(this.easing, positionA.z, positionB.z, t)
            );
        }

        public override Vector3 GetPosition(CameraController camera, bool withoutSmoothing = false)
		{
            return this.cameraHead.transform.position;
		}

        public override Vector3 GetDirection(CameraController camera, bool withoutSmoothing = false)
        {
            return this.lookAt.GetDirection(this.cameraHead);
        }
	}
}