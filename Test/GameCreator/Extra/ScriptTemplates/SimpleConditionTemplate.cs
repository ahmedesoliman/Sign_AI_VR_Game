/* ##HEADER##
namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	[AddComponentMenu("")]
	public class __CLASS_NAME__ : ICondition
	{
		public bool satisfied = true;

		public override bool Check(GameObject target)
		{
			return this.satisfied;
		}
        
		#if UNITY_EDITOR
        public static new string NAME = "Custom/__CLASS_NAME__";
		#endif
	}
}
##FOOTER## */
