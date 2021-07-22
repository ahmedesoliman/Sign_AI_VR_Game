namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Core;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeFixed : ICameraMotorType 
	{
		public static new string NAME = "Fixed Camera";

        public TargetDirection lookAt = new TargetDirection(TargetDirection.Target.CurrentDirection);

		public override Vector3 GetDirection(CameraController camera, bool withoutSmoothing = false)
		{
            return this.lookAt.GetDirection(gameObject);
		}
	}
}