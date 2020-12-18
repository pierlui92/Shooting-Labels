using UnityEngine;
using System.Collections;

namespace EZEffects
{
    [AddComponentMenu("EZ Effects/Complex Light")]
    public class ComplexLight : MonoBehaviour
    {
        [Header("Fade Properties")]
        /// <summary>
        /// The light to use.
        /// </summary>
        public Light Light;
        /// <summary>
        /// A gradient that represents the change in color of the light.
        /// </summary>
        public Gradient ColorGradient;
        /// <summary>
        /// How long to fade the light, in seconds.
        /// </summary>
        public float FadeTime;
        /// <summary>
        /// Should the light automatically fade when enabled? Useful when used with an EffectImpact or EffectMuzzleFlash.
        /// </summary>
        public bool FadeOutOnEnable;

        [Header("Flicker Properties")]
        /// <summary>
        /// Should this light flicker?
        /// </summary>
        public bool Flicker;
        /// <summary>
        /// The range in intensity the light uses while flickering.
        /// </summary>
        public Range IntensityRange;
        /// <summary>
        /// How quickly the light should flicker.
        /// </summary>
        public float FlickerSpeed;

        float timer = 0;
        bool fadeIn, off, fading;
        Vector2 randomValue;

        void Start()
        {
            randomValue = Random.insideUnitCircle.normalized;

            if (Light == null)
                Light = GetComponentInChildren<Light>();

            if (Light == null)
            {
                Debug.LogError("LightComplex " + gameObject.name + " does not have a light assigned!");
                this.enabled = false;
                return;
            }
        }


        void OnEnable()
        {
            if (!FadeOutOnEnable)
                return;

            fadeIn = false;
            timer = 0;
            Light.enabled = true;
            off = false;

            StartCoroutine(fade(FadeTime));
        }

        /// <summary>
        /// Starts a fade in of the light.
        /// <param name="time">An optional parameter defining how long to fade the light (in seconds).</param>
        /// </summary>
        public void FadeIn(float time = -1)
        {
            fadeIn = true;
            timer = 0;

            if (time == -1)
                time = FadeTime;

            if (!fading)
                StartCoroutine(fade(time));
        }

        /// <summary>
        /// Starts a fade out of the light.
        /// <param name="time">An optional parameter defining how long to fade the light (in seconds).</param>
        /// </summary>
        public void FadeOut(float time = -1)
        {
            fadeIn = false;
            timer = 0;

            if (time == -1)
                time = FadeTime;

            if (!fading)
                StartCoroutine(fade(time));
        }

        IEnumerator fade(float fadeTime)
        {
            fading = true;

            if (fadeIn)
                Light.enabled = true;

            while (timer < fadeTime)
            {
                timer += Time.deltaTime;

                if (fadeIn)
                    Light.color = ColorGradient.Evaluate(1 - (timer / fadeTime));
                else
                    Light.color = ColorGradient.Evaluate(timer / fadeTime);

                yield return null;
            }

            if (fadeIn)
                off = false;
            else
            {
                off = true;
                Light.enabled = false;
            }

            fading = false;
        }

        void Update()
        {
            if (off || !Flicker)
                return;

            float noise = Mathf.PerlinNoise(Time.time * randomValue.x * FlickerSpeed, Time.time * randomValue.y * FlickerSpeed);
            Light.intensity = IntensityRange.Lerp(noise);
        }
    }
}