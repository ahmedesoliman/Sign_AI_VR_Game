namespace GameCreator.Camera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraShake
    {
        public const float COEF_SHAKE_POSITION = 01.0f;
        public const float COEF_SHAKE_ROTATION = 25.0f;

        private const float SEED_MIN = 0.0f;
        private const float SEED_MAX = 1000.0f;

        private static readonly AnimationCurve EASING = AnimationCurve.EaseInOut(
            0.0f, 
            1.0f, 
            1.0f, 
            0.0f
        );

        // PROPERTIES: ----------------------------------------------------------------------------

        bool shakePosition = true;
        bool shakeRotation = true;

        private float magnitude;
        private float roughness;

        private Vector3 seed;
        private float perlinSpeed;

        private float startTime;
        private float duration;

        private Transform origin;
        private float radius;

        // INITIALIZERS: --------------------------------------------------------------------------

        public CameraShake(float duration, float roughness, float magnitude = 1.0f, 
            bool shakePosition = true, bool shakeRotation = true, 
            Transform origin = null, float radius = 10f)
        {
            this.Initialize();

            this.shakePosition = shakePosition;
            this.shakeRotation = shakeRotation;

            this.duration = duration;
            this.roughness = roughness;
            this.magnitude = magnitude;

            if (origin != null)
            {
                this.origin = origin;
                this.radius = radius;
            }
        }

        public CameraShake(float duration, CameraShake cameraShake)
        {
            this.startTime = Time.time;
            this.seed = cameraShake.seed;
            this.perlinSpeed = cameraShake.perlinSpeed;

            this.shakePosition = cameraShake.shakePosition;
            this.shakeRotation = cameraShake.shakeRotation;

            this.duration = duration;
            this.roughness = cameraShake.roughness;
            this.magnitude = cameraShake.magnitude;

            this.origin = cameraShake.origin;
            this.radius = cameraShake.radius;
        }

        private void Initialize()
        {
            this.magnitude = 1.0f;
            this.roughness = 1.0f;

            this.perlinSpeed = 0.0f;
            this.seed = new Vector3(
                Random.Range(SEED_MIN, SEED_MAX),
                Random.Range(SEED_MIN, SEED_MAX),
                Random.Range(SEED_MIN, SEED_MAX)
            );

            this.startTime = Time.time;
            this.duration = 1.0f;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 Update(CameraController camera)
        {
            Vector3 amount = new Vector3(
                Mathf.PerlinNoise(this.perlinSpeed, this.seed.x) - 0.5f,
                Mathf.PerlinNoise(this.perlinSpeed, this.seed.y) - 0.5f,
                Mathf.PerlinNoise(this.perlinSpeed, this.seed.z) - 0.5f
            );

            this.perlinSpeed += Time.deltaTime * this.roughness;

            float coefficient = 1.0f;
            if (this.origin != null)
            {
                float distance = Vector3.Distance(this.origin.position, camera.transform.position);
                coefficient = 1f - Mathf.Clamp01(distance/this.radius);
            }

            return amount * this.magnitude * coefficient;
        }

        public float GetEasing()
        {
            return EASING.Evaluate(this.GetNormalizedProgress());
        }

        public bool IsComplete()
        {
            return this.GetNormalizedProgress() >= 1.0f;
        }

        public Vector3 GetInfluencePosition()
        {
            return (this.shakePosition ? Vector3.one : Vector3.zero);
        }

        public Vector3 GetInfluenceRotation()
        {
            return (this.shakeRotation ? Vector3.one : Vector3.zero);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private float GetNormalizedProgress()
        {
            return Mathf.Clamp01((Time.time - this.startTime) / this.duration);
        }
    }
}