namespace GameCreator.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Playables;
    using GameCreator.Core;

    [System.Serializable]
    public class ConditionsAsset : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<ConditionsBehavior> playable = ScriptPlayable<ConditionsBehavior>.Create(graph);
            ConditionsBehavior behavior = playable.GetBehaviour();
            behavior.invoker = owner;

            return playable;
        }
    }
}