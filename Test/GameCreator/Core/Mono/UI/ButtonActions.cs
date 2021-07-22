namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    #if UNITY_EDITOR
    using UnityEditor.Events;
    #endif

    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Actions))]
    [AddComponentMenu("Game Creator/UI/Button", 10)]
    public class ButtonActions : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        public class ButtonActionsEvent : UnityEvent<GameObject> { }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Actions actions = null;
        public ButtonActionsEvent onClick = new ButtonActionsEvent();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying) return;
            EventSystemManager.Instance.Wakeup();
        }

        // VALIDATE: ------------------------------------------------------------------------------

        #if UNITY_EDITOR
        private new void OnValidate()
        {
            base.OnValidate();

            if (this.actions == null)
            {
                this.actions = gameObject.GetComponent<Actions>();
                if (this.actions == null) return;

                this.onClick.RemoveAllListeners();
                UnityEventTools.AddObjectPersistentListener<GameObject>(
                    this.onClick,
                    this.actions.ExecuteWithTarget,
                    gameObject
                );
            }

            this.actions.hideFlags = HideFlags.HideInInspector;
        }
        #endif

        // INTERFACES: ----------------------------------------------------------------------------

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            this.Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            this.Press();

            if (!IsActive() || !IsInteractable()) return;
            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Press()
        {
            if (!IsActive() || !IsInteractable()) return;
            if (this.onClick != null) this.onClick.Invoke(gameObject);
        }
    }
}