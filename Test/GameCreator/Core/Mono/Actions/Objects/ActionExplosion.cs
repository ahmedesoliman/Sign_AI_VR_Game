namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
    using GameCreator.Core.Math;
    using GameCreator.Variables;
    using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class ActionExplosion : IAction
	{
        public bool useActionPosition = true;
        public TargetPosition position = new TargetPosition(TargetPosition.Target.Invoker);

        [Space] public NumberProperty radius = new NumberProperty(5f);
        [Space] public NumberProperty force = new NumberProperty(10f);

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Vector3 originPosition = (this.useActionPosition
                ? transform.position : this.position.GetPosition(target)
            );

            float originRadius = this.radius.GetValue(target);
            float originForce = this.force.GetValue(target);

            QueryTriggerInteraction query = QueryTriggerInteraction.Ignore;
            Collider[] colliders = Physics.OverlapSphere(
                originPosition, originRadius,
                Physics.AllLayers, query
            );

            for (int i = 0; i < colliders.Length; ++i)
            {
                Rigidbody rb = colliders[i].GetComponent<Rigidbody>();
                if (rb != null) rb.AddExplosionForce(
                    originForce, originPosition, originRadius,
                    0f, ForceMode.Impulse
                );
            }

            return true;
        }

		#if UNITY_EDITOR
        public static new string NAME = "Physics/Explosion Force";
        private const string NODE_TITLE = "Explode force at {0}";

        public override string GetNodeTitle()
        {
            string pos = this.useActionPosition ? "Self" : this.position.ToString();
            return string.Format(NODE_TITLE, pos);
        }
        #endif
    }
}
