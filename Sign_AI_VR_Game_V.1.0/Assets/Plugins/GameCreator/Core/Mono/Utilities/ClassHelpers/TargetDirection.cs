namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core.Hooks;
    using GameCreator.Variables;

    [System.Serializable]
    public class TargetDirection
    {
        public enum Target
        {
            Player,
            Camera,
            CurrentDirection,
            Transform,
            Point,
            LocalVariable,
            GlobalVariable,
            ListVariable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.CurrentDirection;
        public Vector3 offset = Vector3.zero;

        public Transform targetTransform = null;
        public Vector3 targetPoint = Vector3.zero;

        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetDirection()
        { }

        public TargetDirection(TargetDirection.Target target)
        {
            this.target = target;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 GetDirection(GameObject invoker, Space offsetSpace = Space.World)
        {
            Vector3 direction = Vector3.zero;

            switch (this.target)
            {
                case Target.Player:
                    if (HookPlayer.Instance != null)
                    {
                        Vector3 playerPosition = HookPlayer.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                playerPosition += this.offset;
                                break;

                            case Space.Self:
                                playerPosition += HookPlayer.Instance.transform.TransformDirection(this.offset);
                                break;
                        }

                        direction = playerPosition - invoker.transform.position;
                    }
                    break;

                case Target.Camera:
                    if (HookCamera.Instance != null)
                    {
                        Vector3 cameraPosition = HookCamera.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                cameraPosition += this.offset;
                                break;

                            case Space.Self:
                                cameraPosition += HookCamera.Instance.transform.TransformDirection(this.offset);
                                break;
                        }
                        direction = cameraPosition - invoker.transform.position;
                    }
                    break;

                case Target.CurrentDirection:
                    direction = invoker.transform.forward;
                    break;

                case Target.Transform:
                    if (this.targetTransform != null)
                    {
                        Vector3 transformPosition = this.targetTransform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                transformPosition += this.offset;
                                break;

                            case Space.Self:
                                transformPosition += this.targetTransform.TransformDirection(this.offset);
                                break;
                        }
                        direction = transformPosition - invoker.transform.position;
                    }
                    break;

                case Target.Point:
                    direction = this.targetPoint - invoker.transform.position;
                    break;

                case Target.LocalVariable:
                    Vector3 localPosition = Vector3.zero;

                    switch (this.local.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            localPosition = (Vector3)this.local.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject @object = this.local.Get(invoker) as GameObject;
                            if (@object != null)
                            {
                                localPosition = @object.transform.position;
                            }
                            break;
                    }
                    direction = localPosition - invoker.transform.position;
                    break;

                case Target.GlobalVariable:
                    Vector3 globalPosition = Vector3.zero;
                    switch (this.global.GetDataType())
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            globalPosition = (Vector3)this.global.Get();
                            break;

                        case Variable.DataType.GameObject:
                            GameObject @object = this.global.Get() as GameObject;
                            if (@object != null)
                            {
                                globalPosition = @object.transform.position;
                            }
                            break;
                    }
                    direction = globalPosition - invoker.transform.position;
                    break;

                case Target.ListVariable:
                    Vector3 listPosition = Vector3.zero;
                    switch (this.list.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            listPosition = (Vector3)this.list.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject @object = this.list.Get(invoker) as GameObject;
                            if (@object != null)
                            {
                                listPosition = @object.transform.position;
                            }
                            break;
                    }
                    direction = listPosition - invoker.transform.position;
                    break;
            }

            return direction.normalized;
        }

        public override string ToString()
        {
            string result = "(unknown)";
            switch (this.target)
            {
                case Target.Player:
                    result = "Player";
                    break;

                case Target.Camera:
                    result = "Camera";
                    break;

                case Target.CurrentDirection:
                    result = "Current Direction";
                    break;

                case Target.Transform:
                    result = (this.targetTransform == null
                        ? "(none)"
                        : this.targetTransform.gameObject.name
                    );
                    break;

                case Target.Point:
                    result = this.targetPoint.ToString();
                    break;

                case Target.LocalVariable:
                    result = this.local.ToString();
                    break;

                case Target.GlobalVariable:
                    result = this.global.ToString();
                    break;

                case Target.ListVariable:
                    result = this.list.ToString();
                    break;
            }

            return result;
        }
    }
}