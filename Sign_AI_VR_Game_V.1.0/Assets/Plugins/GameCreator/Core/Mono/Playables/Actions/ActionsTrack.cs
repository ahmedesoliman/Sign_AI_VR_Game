namespace GameCreator.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Timeline;
    using UnityEngine.Playables;
    using GameCreator.Core;

    [TrackColor(0.8f, 0.76f, 0.64f)]
    [TrackClipType(typeof(ActionsAsset))]
    [TrackBindingType(typeof(Actions))]
    public class ActionsTrack : TrackAsset 
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            clip.displayName = "Actions";
            clip.duration = 1.0f;
        }
    }
}