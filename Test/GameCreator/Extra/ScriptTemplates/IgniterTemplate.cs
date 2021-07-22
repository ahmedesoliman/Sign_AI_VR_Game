/* ##HEADER##
namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

    [AddComponentMenu("")]
    public class __CLASS_NAME__ : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "My Igniters/Igniter Example";
        //public new static string COMMENT = "Uncomment to add an informative message";
        //public new static bool REQUIRES_COLLIDER = true; // uncomment if the igniter requires a collider
        #endif

        public bool example = false;

        private void Update()
        {
            if (this.example)
            {
                this.ExecuteTrigger(gameObject);
                this.example = false;
            }
        }
	}
}
##FOOTER## */