namespace GameCreator.Messages
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using GameCreator.Core;

	[AddComponentMenu("Game Creator/Managers/SimpleMessageManager", 100)]
	public class SimpleMessageManager : Singleton<SimpleMessageManager> 
	{
		private const string CANVAS_ASSET_PATH = "GameCreator/Messages/SimpleMessage";

		private static int ANIMATOR_HASH_SHOW = -1;
		private static int ANIMATOR_HASH_HIDE = -1;
		private static int ANIMATOR_HASH_OPEN = -1;

		private static bool MESSAGE_STATE_OPEN = false;

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private Animator messageAnimator;
		private Text text;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		protected override void OnCreate ()
		{
			EventSystemManager.Instance.Wakeup();

			ANIMATOR_HASH_SHOW = Animator.StringToHash("Show");
			ANIMATOR_HASH_HIDE = Animator.StringToHash("Hide");
			ANIMATOR_HASH_OPEN = Animator.StringToHash("IsOpen");

            DatabaseGeneral general = DatabaseGeneral.Load();
            GameObject prefab = general.prefabSimpleMessage;
            if (prefab == null) prefab = Resources.Load<GameObject>(CANVAS_ASSET_PATH);

            GameObject instance = Instantiate<GameObject>(prefab, transform);
			this.messageAnimator = instance.GetComponentInChildren<Animator>();
			this.text = instance.GetComponentInChildren<Text>();
		}

        protected override bool ShouldNotDestroyOnLoad()
        {
            return false;
        }

        // PUBLIC METHODS: ---------------------------------------------------------------------------------------------

        public void ShowText(string text, Color color)
		{
			this.text.text = text;
			this.text.color = color;
			SimpleMessageManager.MESSAGE_STATE_OPEN = true;
			this.messageAnimator.SetTrigger(ANIMATOR_HASH_SHOW);
			this.messageAnimator.SetBool(ANIMATOR_HASH_OPEN, true);
		}

		public void HideText()
		{
			MESSAGE_STATE_OPEN = false;
			StartCoroutine(this.HideTextDelayed());
		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private IEnumerator HideTextDelayed()
		{
			YieldInstruction waitForSeconds = new WaitForSeconds(0.1f);
			yield return waitForSeconds;

			if (!SimpleMessageManager.MESSAGE_STATE_OPEN)
			{
				this.messageAnimator.SetTrigger(ANIMATOR_HASH_HIDE);
				this.messageAnimator.SetBool(ANIMATOR_HASH_OPEN, false);
			}
		}
	}
}