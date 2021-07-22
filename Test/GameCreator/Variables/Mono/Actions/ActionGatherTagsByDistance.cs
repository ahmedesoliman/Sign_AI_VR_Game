namespace GameCreator.Core
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
    using GameCreator.Variables;

    [AddComponentMenu("")]
	public class ActionGatherTagsByDistance : IAction
    {
        public StringProperty withTag = new StringProperty("Enemy");

        [Space]
        public TargetGameObject origin = new TargetGameObject(TargetGameObject.Target.Player);
        public NumberProperty radius = new NumberProperty(10f);

        [Space]
        public HelperListVariable listVariables = new HelperListVariable();

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            Transform originTransform = this.origin.GetTransform(target);
            if (originTransform == null) return true;

            Collider[] colliders = this.GatherColliders(target);
            List<GameObject> array = new List<GameObject>();

            string compareTag = this.withTag.GetValue(target);
            for (int i = 0; i < colliders.Length; ++i)
            {
                if (colliders[i].gameObject.CompareTag(compareTag))
                {
                    array.Add(colliders[i].gameObject);
                }
            }

            array.Sort((x, y) =>
            {
                float distanceX = Vector3.Distance(originTransform.position, x.transform.position);
                float distanceY = Vector3.Distance(originTransform.position, y.transform.position);
                return distanceX.CompareTo(distanceY);
            });

            ListVariables list = this.listVariables.GetListVariables(target);
            VariablesManager.ListClear(list);

            foreach (GameObject element in array)
            {
                VariablesManager.ListPush(list, ListVariables.Position.Last, element);
            }

            return true;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected virtual int FilterLayerMask()
        {
            return -1;
        }

        protected Collider[] GatherColliders(GameObject target)
        {
            Transform transformOrigin = this.origin.GetTransform(target);
            if (transformOrigin == null) return new Collider[0];

            Vector3 position = transformOrigin.position;
            QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal;
            int layerMask = this.FilterLayerMask();

            return Physics.OverlapSphere(position, this.radius.GetValue(target), layerMask, query);
        }

        #if UNITY_EDITOR
        public static new string NAME = "Variables/Gather Object Tags by Distance";
        private const string NODE_TITLE = "Gather {0} by distance to {1}";

        public override string GetNodeTitle()
        {
            return string.Format(
                NODE_TITLE,
                this.tag,
                this.origin
            );
        }
        #endif
    }
}
