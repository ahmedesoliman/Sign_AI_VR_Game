namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
    using GameCreator.Core.Hooks;
    using GameCreator.Core.Math;
    using GameCreator.Variables;
    using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionCameraFOV : IAction
	{
        public float duration = 0.5f;
        public NumberProperty fieldOfView = new NumberProperty(60f);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (Mathf.Approximately(this.duration, 0f))
            {
                HookCamera.Instance.Get<Camera>().fieldOfView = this.fieldOfView.GetValue(target);
                return true;
            }

            return false;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            float t = 0f;
            float initValue = HookCamera.Instance.Get<Camera>().fieldOfView;
            float targValue = this.fieldOfView.GetValue(target);

            while (t <= 1f)
            {
                t += Time.deltaTime / this.duration;

                float value = Easing.ExpoInOut(initValue, targValue, t);
                HookCamera.Instance.Get<Camera>().fieldOfView = value;

                yield return null;
            }

            HookCamera.Instance.Get<Camera>().fieldOfView = targValue;
            yield return 0;
        }

        #if UNITY_EDITOR

        public static new string NAME = "Camera/Change FOV";
        private const string NODE_TITLE = "Change FOV to {0} ({1})";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.fieldOfView,
                Mathf.Approximately(this.duration, 0f)
                    ? "instant"
                    : this.duration.ToString("0.00")
            );
        }

        #endif
    }
}
