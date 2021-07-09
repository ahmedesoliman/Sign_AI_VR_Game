namespace GameCreator.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Timeline;
    using UnityEngine.Playables;
    using GameCreator.Core;

    [TrackColor(0.59f, 0.79f, 0.75f)]
    [TrackClipType(typeof(ConditionsAsset))]
    [TrackBindingType(typeof(Conditions))]
    public class ConditionsTrack : TrackAsset 
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            clip.displayName = "Conditions";
            clip.duration = 1.0f;
        }
    }
}