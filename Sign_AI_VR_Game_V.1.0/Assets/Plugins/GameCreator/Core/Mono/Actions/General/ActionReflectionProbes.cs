namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionReflectionProbes : IAction
	{
        public ReflectionProbe reflectionProbe;
        public bool waitTillComplete = true;

        private int renderID;

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.reflectionProbe == null) return true;

            this.renderID = this.reflectionProbe.RenderProbe();
            return !this.waitTillComplete;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            while (!this.reflectionProbe.IsFinishedRendering(this.renderID))
            {
                yield return null;
            }

            yield return 0;
        }

        #if UNITY_EDITOR

        public static new string NAME = "General/Render Reflection Probe";
        private const string NODE_TITLE = "Render probe {0} {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                (this.reflectionProbe == null
                    ? "(none)"
                    : this.reflectionProbe.gameObject.name
                ),
                this.waitTillComplete ? "(and wait)" : string.Empty
            );
        }

        #endif
    }
}
