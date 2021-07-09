namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public abstract class IActionNearest : IAction
	{
        [VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeInVariable = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space]
        public TargetGameObject origin = new TargetGameObject(TargetGameObject.Target.Player);
        [Indent] public float radius = 10f;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform originTransform = this.origin.GetTransform(target);
            if (originTransform == null) return true;

            Collider[] colliders = this.GatherColliders(target);

            GameObject nearestGameObject = null;
            float nearestDistance = Mathf.Infinity;

            for (int i = 0; i < colliders.Length; ++i)
            {
                GameObject item = colliders[i].gameObject;
                if (!this.FilterCondition(item)) continue;

                float distance = Vector3.Distance(item.transform.position, originTransform.position);
                if (distance < nearestDistance)
                {
                    nearestGameObject = item;
                    nearestDistance = distance;
                }
            }

            this.storeInVariable.Set(nearestGameObject, target);
            return true;
        }

        protected virtual bool FilterCondition(GameObject item)
        {
            return true;
        }

        protected virtual int FilterLayerMask()
        {
            return -1;
        }

        public Collider[] GatherColliders(GameObject target)
        {
            Transform transformOrigin = this.origin.GetTransform(target);
            if (transformOrigin == null) return new Collider[0];

            Vector3 position = transformOrigin.position;
            QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal;
            int layerMask = this.FilterLayerMask();

            return Physics.OverlapSphere(position, this.radius, layerMask, query);
        }
	}
}
