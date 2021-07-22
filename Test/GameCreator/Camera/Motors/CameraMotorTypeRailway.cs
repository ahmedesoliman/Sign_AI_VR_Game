namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Core;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeRailway : ICameraMotorType 
	{
		private static readonly Vector3 CAM_DEFLT_ENDPOINT = new Vector3(5.0f,  0.0f, 0.0f);
		private static readonly Vector3 TAR_DEFAULT_POINT1 = new Vector3(0.0f, -5.0f, 0.0f);
		private static readonly Vector3 TAR_DEFAULT_POINT2 = new Vector3(5.0f, -5.0f, 0.0f);

		public static new string NAME = "Railway Camera";

		// PROPERTIES: ----------------------------------------------------------------------------

		public Vector3 cameraEndPoint = CAM_DEFLT_ENDPOINT;
		public Vector3[] targetPoints = new Vector3[2] {TAR_DEFAULT_POINT1, TAR_DEFAULT_POINT2};
        public TargetPosition target = new TargetPosition(TargetPosition.Target.Player);
        public TargetDirection lookAt = new TargetDirection(TargetDirection.Target.Player);

        private GameObject cameraHead;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.cameraHead = new GameObject("Motor Head");
            this.cameraHead.transform.SetParent(transform);
        }

        // UPDATE: --------------------------------------------------------------------------------

        public override void UpdateMotor()
        {
            Vector3 targetPosition = this.target.GetPosition(gameObject);
            Vector3 aVector = targetPosition - this.targetPoints[0];

            Vector3 bVector = this.targetPoints[1] - this.targetPoints[0];
            Vector3 vectorProjection = Vector3.Project(aVector, bVector);

            float sign = Mathf.Sign(Vector3.Dot(vectorProjection, bVector));
            float t = vectorProjection.magnitude / bVector.magnitude * sign;

            this.cameraHead.transform.position = Vector3.Lerp(
                this.cameraMotor.transform.position, 
                this.cameraEndPoint, 
                t
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