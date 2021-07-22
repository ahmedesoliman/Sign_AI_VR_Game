namespace GameCreator.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using GameCreator.Core;

    public class ActionsBehavior : IGenericBehavior<Actions>
    {
        protected override void Execute()
        {
            this.interactable.Execute(this.invoker);
        }
    }
}