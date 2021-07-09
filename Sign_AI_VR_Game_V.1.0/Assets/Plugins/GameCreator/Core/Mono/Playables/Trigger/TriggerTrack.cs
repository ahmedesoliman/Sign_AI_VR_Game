namespace GameCreator.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Timeline;
    using UnityEngine.Playables;
    using GameCreator.Core;

    [TrackColor(0.76f, 0.76f, 0.76f)]
    [TrackClipType(typeof(TriggerAsset))]
    [TrackBindingType(typeof(Trigger))]
    public class TriggerTrack : TrackAsset 
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);

            clip.displayName = "Trigger";
            clip.duration = 1.0f;
        }
    }
}