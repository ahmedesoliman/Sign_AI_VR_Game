namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Variables;

	[System.Serializable]
	public class TargetPosition
	{
		public enum Target
		{
			Player,
            Camera,
			Invoker,
			Transform,
			Position,
            LocalVariable,
            GlobalVariable,
            ListVariable
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		public Target target = Target.Position;
		public Vector3 offset = Vector3.zero;

        public Transform targetTransform = null;
        public Vector3 targetPosition = Vector3.zero;
        public HelperLocalVariable local = new HelperLocalVariable();
        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperGetListVariable list = new HelperGetListVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetPosition() 
        { }

        public TargetPosition(TargetPosition.Target target)
        {
            this.target = target;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 GetPosition(GameObject invoker, Space offsetSpace = Space.World)
		{
			Vector3 resultPosition = Vector3.zero;
            Vector3 resultOffset = Vector3.zero;

			switch (this.target)
			{
    			case Target.Player :
    				if (HookPlayer.Instance != null)
    				{
                        resultPosition = HookPlayer.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World: 
                                resultOffset = this.offset; 
                                break;

                            case Space.Self: 
                                resultOffset = HookPlayer.Instance.transform.TransformDirection(this.offset);
                                break;
                        }
                    }
    				break;

                case Target.Camera:
                    if (HookCamera.Instance != null)
                    {
                        resultPosition = HookCamera.Instance.transform.position;
                        switch (offsetSpace)
                        {
                            case Space.World:
                                resultOffset = this.offset;
                                break;

                            case Space.Self:
                                resultOffset = HookCamera.Instance.transform.TransformDirection(this.offset);
                                break;
                        }
                    }
                    break;

                case Target.Invoker:
                    resultPosition = invoker.transform.position;
                    resultOffset = this.offset;
                    break;

    			case Target.Transform:
    				if (this.targetTransform != null)
    				{
                        if (this.targetTransform != null)
                        {
                            resultPosition = this.targetTransform.position;
                            switch (offsetSpace)
                            {
                                case Space.World:
                                    resultOffset = this.offset;
                                    break;

                                case Space.Self:
                                    resultOffset = this.targetTransform.TransformDirection(this.offset);
                                    break;
                            }
                        }
    				}
    				break;

    			case Target.Position:
                    resultPosition = this.targetPosition;
                    resultOffset = Vector3.zero;
    				break;

                case Target.LocalVariable:
                    resultOffset = Vector3.zero;
                    switch (this.local.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            resultPosition = (Vector3)this.local.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject _object = this.local.Get(invoker) as GameObject;
                            if (_object != null)
                            {
                                resultPosition = _object.transform.position;
                            }
                            break;
                    }
                    break;

                case Target.GlobalVariable:
                    resultOffset = Vector3.zero;
                    switch (this.global.GetDataType())
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            resultPosition = (Vector3)this.global.Get();
                            break;

                        case Variable.DataType.GameObject:
                            GameObject _object = this.global.Get() as GameObject;
                            if (_object != null)
                            {
                                resultPosition = _object.transform.position;
                            }
                            break;
                    }
                    break;

                case Target.ListVariable:
                    resultOffset = Vector3.zero;
                    switch (this.list.GetDataType(invoker))
                    {
                        case Variable.DataType.Vector2:
                        case Variable.DataType.Vector3:
                            resultPosition = (Vector3)this.list.Get(invoker);
                            break;

                        case Variable.DataType.GameObject:
                            GameObject _object = this.list.Get(invoker) as GameObject;
                            if (_object != null)
                            {
                                resultPosition = _object.transform.position;
                            }
                            break;
                    }
                    break;
            }

			return resultPosition + resultOffset;
		}

        public Quaternion GetRotation(GameObject invoker)
		{
			Quaternion rotation = invoker.transform.rotation;
			switch (this.target)
			{
    			case Target.Player :
    				if (HookPlayer.Instance != null) rotation = HookPlayer.Instance.transform.rotation;
    				break;

    			case Target.Transform:
                    if (this.targetTransform != null) rotation = this.targetTransform.rotation;
    				break;

                case Target.LocalVariable:
                    if (this.local.GetDataType(invoker) == Variable.DataType.GameObject)
                    {
                        GameObject localResult = this.local.Get(invoker) as GameObject;
                        if (localResult != null)
                        {
                            rotation = localResult.transform.rotation;
                        }
                    }
                    break;

                case Target.GlobalVariable:
                    if (this.global.GetDataType() == Variable.DataType.GameObject)
                    {
                        GameObject globalResult = this.global.Get() as GameObject;
                        if (globalResult != null)
                        {
                            rotation = globalResult.transform.rotation;
                        }
                    }
                    break;

                case Target.ListVariable:
                    if (this.list.GetDataType(invoker) == Variable.DataType.GameObject)
                    {
                        GameObject listResult = this.list.Get(invoker) as GameObject;
                        if (listResult != null)
                        {
                            rotation = listResult.transform.rotation;
                        }
                    }
                    break;
            }

			return rotation;
		}

		public override string ToString()
		{
			string result = "(unknown)";
			switch (this.target)
			{
			    case Target.Player : 
                    result = "Player"; 
                    break;

			    case Target.Invoker: 
                    result = "Invoker"; 
                    break;

                case Target.Transform: 
                    result = (this.targetTransform == null 
                        ? "(none)" 
                        : this.targetTransform.gameObject.name
                    ); 
                    break;

			    case Target.Position:
                    result = this.targetPosition.ToString(); 
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