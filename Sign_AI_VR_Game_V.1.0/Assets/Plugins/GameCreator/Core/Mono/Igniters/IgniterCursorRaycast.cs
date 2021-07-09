namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using GameCreator.Variables;
    using GameCreator.Core.Hooks;
    using UnityEditor;

    [AddComponentMenu("")]
    public class IgniterCursorRaycast : Igniter
    {
        #if UNITY_EDITOR    
        public new static string NAME = "Input/On Cursor Raycast";
        public new static bool REQUIRES_COLLIDER = true;
        #endif

        public KeyCode key = KeyCode.Mouse0;

        [Space][VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private void Update()
        {
            if (Input.GetKeyUp(this.key))
            {
                Ray ray = HookCamera.Instance.Get<Camera>().ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitGO = hit.collider.gameObject;
                    if (hitGO.GetInstanceID() == gameObject.GetInstanceID())
                    {
                        this.storeCollider.Set(hitGO, gameObject);
                        this.ExecuteTrigger(gameObject);
                    }
                }
            }
        }
    }
}