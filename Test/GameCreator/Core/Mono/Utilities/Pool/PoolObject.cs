namespace GameCreator.Pool
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [AddComponentMenu("Game Creator/Pool/Pool Object")]
    public class PoolObject : MonoBehaviour
    {
        public const int INIT_COUNT = 20;
        public const float DURATION = 10f;

        // PROPERTIES: ---------------------------------------------------------

        public int initCount = INIT_COUNT;
        public float duration = DURATION;

        private IEnumerator coroutine;

        // PRIVATE METHODS: ----------------------------------------------------

        private void OnEnable()
        {
            this.coroutine = this.SetDisable();
            this.StartCoroutine(this.coroutine);
        }

        private void OnDisable()
        {
            this.CancelInvoke();
            this.StopCoroutine(this.coroutine);
        }

        private IEnumerator SetDisable()
        {
            WaitForSeconds wait = new WaitForSeconds(this.duration);
            yield return wait;

            this.gameObject.SetActive(false);
        }
    }
}