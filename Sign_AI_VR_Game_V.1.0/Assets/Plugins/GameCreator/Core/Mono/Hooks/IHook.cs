namespace GameCreator.Core.Hooks
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;

	public abstract class IHook<T> : MonoBehaviour 
	{
		private const string ERR_NOCOMP = "Component of type {0} could not be found in object {1}";

        public static IHook<T> Instance;

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private Dictionary<int, Behaviour> components;

		// INITIALIZERS: -----------------------------------------------------------------------------------------------

		private void Awake()
		{
			Instance = this;
			this.components = new Dictionary<int, Behaviour>();
		}

        // PUBLIC METHODS: ---------------------------------------------------------------------------------------------

        public TComponent Get<TComponent>() where TComponent : Behaviour
		{
			int componentHash = typeof(TComponent).GetHashCode();
			if (!this.components.ContainsKey(componentHash))
			{
				Behaviour mono = gameObject.GetComponent<TComponent>();
                if (mono == null) return null;

				this.components.Add(componentHash, mono);
				return (TComponent)mono;
			}

			return (TComponent)this.components[componentHash];
		}
	}
}
