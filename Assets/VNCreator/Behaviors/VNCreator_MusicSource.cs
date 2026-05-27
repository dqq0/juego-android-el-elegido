using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNCreator
{
    [RequireComponent(typeof(AudioSource))]
    public class VNCreator_MusicSource : MonoBehaviour
    {
        private AudioSource[] sources = new AudioSource[2];
        private int activeSourceIndex = 0;
        private Coroutine fadeCoroutine;

        public static VNCreator_MusicSource instance;
        public float crossfadeDuration = 1.5f;

        private void Awake()
        {
            instance = this;
            sources[0] = GetComponent<AudioSource>();
            sources[0].playOnAwake = false;
            sources[0].loop = true;
            sources[0].volume = GameOptions.musicVolume;

            // Creamos un segundo AudioSource para hacer el cruce
            sources[1] = gameObject.AddComponent<AudioSource>();
            sources[1].playOnAwake = false;
            sources[1].loop = true;
            sources[1].volume = 0f;
        }

        public void Play(AudioClip clip)
        {
            AudioSource activeSource = sources[activeSourceIndex];

            // Si es la misma cancion y esta sonando, no hacemos nada
            if (activeSource.clip == clip && activeSource.isPlaying)
                return;

            // Cambiamos al otro reproductor
            int nextSourceIndex = (activeSourceIndex + 1) % 2;
            AudioSource nextSource = sources[nextSourceIndex];

            nextSource.clip = clip;
            nextSource.Play();

            // Si habia un fade en proceso, lo detenemos
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // Iniciamos el nuevo crossfade
            fadeCoroutine = StartCoroutine(Crossfade(activeSource, nextSource));
            activeSourceIndex = nextSourceIndex;
        }

        private IEnumerator Crossfade(AudioSource fadingOut, AudioSource fadingIn)
        {
            float targetVolume = GameOptions.musicVolume;
            float t = 0;

            fadingIn.volume = 0f;

            while (t < crossfadeDuration)
            {
                t += Time.deltaTime;
                float fraction = t / crossfadeDuration;
                
                if (fadingOut.isPlaying)
                    fadingOut.volume = Mathf.Lerp(targetVolume, 0f, fraction);

                fadingIn.volume = Mathf.Lerp(0f, targetVolume, fraction);

                yield return null;
            }

            fadingOut.Stop();
            fadingOut.volume = 0f;
            fadingIn.volume = targetVolume;
        }
        public void UpdateVolume(float vol)
        {
            sources[0].volume = vol;
            sources[1].volume = vol;
        }
    }
}
