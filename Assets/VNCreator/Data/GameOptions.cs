using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNCreator
{
    public static class GameOptions
    {
        public static float musicVolume = 0.5f;
        public static float sfxVolume = 0.5f;
        public static float readSpeed = 0.5f;
        public static bool isInstantText = false;
        public static bool isMuted = false;

        public static void InitilizeOptions()
        {
            if (PlayerPrefs.HasKey("MusicVolume"))
                musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            if (PlayerPrefs.HasKey("SfxVolume"))
                sfxVolume = PlayerPrefs.GetFloat("SfxVolume");
            if (PlayerPrefs.HasKey("ReadSpeed"))
                readSpeed = PlayerPrefs.GetFloat("ReadSpeed");
            if (PlayerPrefs.HasKey("InstantText"))
                isInstantText = PlayerPrefs.GetInt("InstantText") == 1 ? true : false;
            if (PlayerPrefs.HasKey("IsMuted"))
                isMuted = PlayerPrefs.GetInt("IsMuted") == 1 ? true : false;

            ApplyMute();
        }

        public static void SetMute(bool mute)
        {
            isMuted = mute;
            PlayerPrefs.SetInt("IsMuted", mute ? 1 : 0);
            ApplyMute();
        }

        private static void ApplyMute()
        {
            AudioListener.volume = isMuted ? 0f : 1f;
        }

        public static void SetMusicVolume(float index)
        {
            musicVolume = index;
            PlayerPrefs.SetFloat("MusicVolume", index);
            if (VNCreator_MusicSource.instance != null)
                VNCreator_MusicSource.instance.UpdateVolume(index);
        }

        public static void SetSFXVolume(float index)
        {
            sfxVolume = index;
            PlayerPrefs.SetFloat("SfxVolume", index);
            if (VNCreator_SfxSource.instance != null)
                VNCreator_SfxSource.instance.UpdateVolume(index);
        }

        public static void SetReadingSpeed(float index)
        {
            readSpeed = index;
            PlayerPrefs.SetFloat("ReadSpeed", index);
        }

        public static void SetInstantText(bool index)
        {
            isInstantText = index;
            PlayerPrefs.SetInt("InstantText", index ? 1 : 0);
        }
    }
}
