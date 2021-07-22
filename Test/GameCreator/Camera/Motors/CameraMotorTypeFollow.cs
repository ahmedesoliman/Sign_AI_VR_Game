namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core;
	using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeFollow : ICameraMotorType 
	{
		public static new string NAME = "Follow Camera";

        // PROPERTIES: ----------------------------------------------------------------------------

        public TargetGameObject anchor = new TargetGameObject(TargetGameObject.Target.Player);
        public Vector3 anchorOffset = new Vector3(0, 2, -3);
        public TargetDirection lookAt = new TargetDirection(TargetDirection.Target.Player);

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void UpdateMotor()
        {
            GameObject targetGO = this.anchor.GetGameObject(gameObject);
            if (targetGO == null) return;

            Transform target = targetGO.transform;
            if (target == null) return;

            transform.position = target.position + this.anchorOffset;
            transform.rotation = Quaternion.FromToRotation(
                transform.forward,
                this.lookAt.GetDirection(gameObject)
            );
        }

		public override Vector3 GetPosition (CameraController camera, bool withoutSmoothing = false)
		{
            return transform.position;
		}

        public override Vector3 GetDirection(CameraController camera, bool withoutSmoothing = false)
        {
            return this.lookAt.GetDirection(gameObject);
        }
	}
}