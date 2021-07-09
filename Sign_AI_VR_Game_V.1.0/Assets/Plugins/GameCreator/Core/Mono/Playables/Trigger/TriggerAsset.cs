namespace GameCreator.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using GameCreator.Core;

    [System.Serializable]
    public class TriggerAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<TriggerBehavior> playable = ScriptPlayable<TriggerBehavior>.Create(graph);
            TriggerBehavior behavior = playable.GetBehaviour();
            behavior.invoker = owner;

            return playable;
        }
    }
}