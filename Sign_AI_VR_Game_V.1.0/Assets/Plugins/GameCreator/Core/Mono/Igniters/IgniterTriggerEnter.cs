namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
	public class IgniterTriggerEnter : Igniter 
	{
		public Collider otherCollider;

		#if UNITY_EDITOR
        public new static string NAME = "Object/On Trigger Enter";
        public new static string COMMENT = "Leave empty to trigger any object";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

        [Space][VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeSelf = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space][VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private void Start()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }
        }

		private void OnTriggerEnter(Collider c)
		{
            this.storeSelf.Set(gameObject);
            this.storeCollider.Set(c.gameObject);

            if (this.otherCollider == null)
            {
                this.ExecuteTrigger(c.gameObject);
            }
			else if (this.otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
			{
                this.ExecuteTrigger(c.gameObject);
			}
		}
	}
}