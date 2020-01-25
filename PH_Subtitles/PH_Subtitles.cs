using System;
using HarmonyLib;
using BepInEx;

using BepInEx.Harmony;
using UnityEngine;
using UnityEngine.UI.Translation;

namespace PH_Subtitles {
    [BepInPlugin(nameof(PH_Subtitles), nameof(PH_Subtitles), "1.0.0")]
    public class PH_Subtitles : BaseUnityPlugin
    {
        private void Awake()
        {
            HarmonyWrapper.PatchAll(typeof(PH_Subtitles));
        }

        [HarmonyPrefix, HarmonyPatch(typeof(AudioSource), "Play", new Type[]{})]
        public static void AudioSource_Play_Patch(AudioSource __instance)
        {
            SetSubtitle(__instance);
        }

        private static void SetSubtitle(AudioSource source)
        {
            AudioSourceSubtitle.Instance.Add(source);
        }
    }
}