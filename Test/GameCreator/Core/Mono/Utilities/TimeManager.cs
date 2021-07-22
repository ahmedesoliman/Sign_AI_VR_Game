namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("")]
    public class TimeManager : Singleton<TimeManager> 
    {
        private const int LAYER_DEFAULT = 0;
        private const float PHYSICS_TIMESTEP = 0.02f;

        private class TimeData
        {
            private readonly float to;
            private readonly float from;

            private readonly float duration;
            private readonly float startTime;

            public TimeData(float duration, float to, float from = 1.0f)
            {
                this.to = to;
                this.from = from;

                this.duration = duration;
                this.startTime = Time.time;
            }

            public float Get()
            {
                if (Mathf.Approximately(this.duration, 0f)) return this.to;

                float t = (Time.time - this.startTime) / this.duration;
                return Mathf.SmoothStep(this.from, this.to, t);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<int, TimeData> timeScales = new Dictionary<int, TimeData>();
        private float iterateTime = 0.0f;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetTimeScale(float timeScale, int layer = LAYER_DEFAULT)
        {
            this.timeScales[layer] = new TimeData(0f, timeScale);
            this.RecalculateTimeScale();
        }

        public void SetSmoothTimeScale(float timeScale, float duration, int layer = LAYER_DEFAULT)
        {
            this.iterateTime = Mathf.Max(this.iterateTime, Time.time + duration);

            float from = 1.0f;
            if (this.timeScales.ContainsKey(layer))
            {
                from = this.timeScales[layer].Get();
            }

            this.timeScales[layer] = new TimeData(duration, timeScale, from);         
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        private void Update()
        {
            if (Time.time > this.iterateTime) return;
            this.RecalculateTimeScale();
        }

        private void RecalculateTimeScale()
        {
            float scale = this.timeScales.Count > 0 ? 99f : 1.0f;
            foreach (KeyValuePair<int, TimeData> item in this.timeScales)
            {
                scale = Mathf.Min(scale, item.Value.Get());
            }

            Time.timeScale = scale;
            Time.fixedDeltaTime = PHYSICS_TIMESTEP * scale;
        }
    }
}