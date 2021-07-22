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
    public class IgniterTriggerEnterTag : Igniter 
	{
        [TagSelector]
        public string objectWithTag = "";

		#if UNITY_EDITOR
        public new static string NAME = "Object/On Tag Enter";
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

            if (c.CompareTag(this.objectWithTag))
            {
                this.ExecuteTrigger(c.gameObject);
            }
		}
	}
}