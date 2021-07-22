namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class GlobalTags : ScriptableObject
    {
        public Tag[] tags = new Tag[32];
    }
}