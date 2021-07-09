namespace GameCreator.Core
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Variables;
    using UnityEngine.Events;

    [System.Serializable]
	public class TargetGameObject
	{
		public enum Target
		{
			Player,
            Camera,
			Invoker,
			GameObject,
            LocalVariable,
            ListVariable,
            GlobalVariable,
		}

        [Serializable]
        public class ChangeEvent : UnityEvent { }

		// PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.GameObject;

        public GameObject gameObject;
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        public ChangeEvent eventChangeVariable = new ChangeEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetGameObject() { }

        public TargetGameObject(TargetGameObject.Target target) 
        {
            this.target = target;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject GetGameObject(GameObject invoker)
		{
            GameObject result = null;

			switch (this.target)
			{
    			case Target.Player :
                    if (HookPlayer.Instance != null) result = HookPlayer.Instance.gameObject;
    				break;

                case Target.Camera:
                    if (HookCamera.Instance != null) result = HookCamera.Instance.gameObject;
                    break;

                    case Target.Invoker:
    				result = invoker;
    				break;
                    
                case Target.GameObject:
                    result = this.gameObject;
    				break;

                case Target.ListVariable:
                    result = this.list.Get(invoker) as GameObject;
                    break;

                case Target.LocalVariable:
                    result = this.local.Get(invoker) as GameObject;
                    break;

                case Target.GlobalVariable:
                    result = this.global.Get(invoker) as GameObject;
                    break;
            }

			return result;
		}

        public Transform GetTransform(GameObject invoker)
        {
            GameObject targetGo = this.GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.transform;
        }

        public T GetComponent<T>(GameObject invoker) where T : UnityEngine.Object
        {
            GameObject targetGo = this.GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.GetComponent<T>();
        }

        public object GetComponent(GameObject invoker, string type)
        {
            GameObject targetGo = this.GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.GetComponent(type);
        }

        public T GetComponentInChildren<T>(GameObject invoker) where T : UnityEngine.Object
        {
            GameObject targetGo = this.GetGameObject(invoker);
            if (targetGo == null) return null;
            return targetGo.GetComponentInChildren<T>();
        }

        public T[] GetComponentsInChildren<T>(GameObject invoker) where T : UnityEngine.Object
        {
            GameObject targetGo = this.GetGameObject(invoker);
            if (targetGo == null) return new T[0];
            return targetGo.GetComponentsInChildren<T>();
        }

        // EVENTS: --------------------------------------------------------------------------------

        public void StartListeningVariableChanges(GameObject invoker)
        {
            switch (this.target)
            {
                case Target.GlobalVariable:
                    VariablesManager.events.SetOnChangeGlobal(
                        this.OnChangeVariable,
                        this.global.name
                    );
                    break;

                case Target.LocalVariable:
                    VariablesManager.events.SetOnChangeLocal(
                        this.OnChangeVariable,
                        this.local.GetGameObject(invoker),
                        this.local.name
                    );
                    break;

                case Target.ListVariable:
                    VariablesManager.events.StartListenListAny(
                        this.OnChangeVariable,
                        this.list.GetListVariables(invoker).gameObject
                    );
                    break;
            }
        }

        public void StopListeningVariableChanges(GameObject invoker)
        {
            switch (this.target)
            {
                case Target.GlobalVariable:
                    VariablesManager.events.RemoveChangeGlobal(
                        this.OnChangeVariable,
                        this.global.name
                    );
                    break;

                case Target.LocalVariable:
                    VariablesManager.events.RemoveChangeLocal(
                        this.OnChangeVariable,
                        this.local.GetGameObject(invoker),
                        this.local.name
                    );
                    break;

                case Target.ListVariable:
                    VariablesManager.events.StopListenListAny(
                        this.OnChangeVariable,
                        this.list.GetListVariables(invoker).gameObject
                    );
                    break;
            }
        }

        private void OnChangeVariable(string variableID)
        {
            this.eventChangeVariable.Invoke();
        }

        private void OnChangeVariable(int index, object prev, object next)
        {
            this.eventChangeVariable.Invoke();
        }

        // UTILITIES: -----------------------------------------------------------------------------

        private const string NAME_LOCAL = "local[{0}]";
        private const string NAME_GLOBAL = "global[{0}]";

        public override string ToString ()
		{
			string result = "(unknown)";
			switch (this.target)
			{
    			case Target.Player : result = "Player"; break;
    			case Target.Invoker: result = "Invoker"; break;
                case Target.Camera: result = "Camera"; break;
                case Target.GameObject: 
                    result = this.gameObject != null ? this.gameObject.name : "(null)"; 
                    break;
                
                case Target.LocalVariable:
                    result = string.Format(NAME_LOCAL, this.local.name);
                    break;

                case Target.ListVariable:
                    result = this.list.ToString();
                    break;

                case Target.GlobalVariable:
                    result = string.Format(NAME_GLOBAL, this.global.name);
                    break;
            }

			return result;
		}
	}
}