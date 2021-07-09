namespace GameCreator.Core
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    [AddComponentMenu("")]
    public class TouchStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public Image jsContainer;
        public Image joystick;

        private Vector3 direction = Vector3.zero;

		// EVENT METHODS: -------------------------------------------------------------------------

		private void Start()
		{
            EventSystemManager.Instance.Wakeup();
		}

        private void OnEnable()
        {
            this.OnPointerUp(null);
        }

        private void OnDisable()
        {
            this.OnPointerUp(null);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.jsContainer.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out position
            );

            position.x = (position.x / this.jsContainer.rectTransform.sizeDelta.x);
            position.y = (position.y / this.jsContainer.rectTransform.sizeDelta.y);

            float x = (Mathf.Approximately(this.jsContainer.rectTransform.pivot.x, 1f)) ? position.x * 2 + 1 : position.x * 2 - 1;
            float y = (Mathf.Approximately(this.jsContainer.rectTransform.pivot.y, 1f)) ? position.y * 2 + 1 : position.y * 2 - 1;

            this.direction = new Vector3(x, y, 0);
            this.direction = (this.direction.magnitude > 1) ? this.direction.normalized : this.direction;

            this.joystick.rectTransform.anchoredPosition = new Vector3(
                this.direction.x * (this.jsContainer.rectTransform.sizeDelta.x / 3),
                this.direction.y * (this.jsContainer.rectTransform.sizeDelta.y / 3)
            );
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.direction = Vector3.zero;
            this.joystick.rectTransform.anchoredPosition = Vector3.zero;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector2 GetDirection()
        {
            return this.direction;
        }
    }
}