namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;

	[AddComponentMenu("")]
	public class IgniterTriggerStayTimeout : Igniter 
	{
		public Collider otherCollider;

		#if UNITY_EDITOR
		public new static string NAME = "Object/On Trigger Stay Timeout";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

        public float duration = 2.0f;
        private float startTime = 0.0f;
        private bool hasBeenExecuted = false;

        [Space][VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeSelf = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space][VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private void OnTriggerEnter(Collider c)
        {
            if (this.otherCollider == null)
            {
                this.startTime = Time.time;
                this.hasBeenExecuted = false;
            }
            else if (this.otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
            {
                this.startTime = Time.time;
                this.hasBeenExecuted = false;
            }
        }

        private void OnTriggerExit(Collider c)
        {
            if (this.otherCollider == null)
            {
                this.startTime = Time.time;
            }
            else if (this.otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
            {
                this.startTime = Time.time;
            }
        }

        private void OnTriggerStay(Collider c)
        {
            bool timeout = this.startTime + this.duration < Time.time;
            bool collided = (
                this.otherCollider == null || 
                this.otherCollider.gameObject.GetInstanceID() == c.gameObject.GetInstanceID()
            );
                
            if (collided && timeout && !this.hasBeenExecuted)
            {
                this.storeSelf.Set(gameObject);
                this.storeCollider.Set(c.gameObject);

                this.hasBeenExecuted = true;
                this.ExecuteTrigger(c.gameObject);
            }
        }
	}
}