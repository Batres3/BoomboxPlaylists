using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BoomboxPlaylists.Managers;
using UnityEngine;

namespace BoomboxPlaylists.Patches
{
    [HarmonyPatch(typeof(BoomboxItem))]
    internal class BoomboxItemPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start_Postfix(BoomboxItem __instance)
        {
            if (AudioManager.finishedLoading)
            {
                AudioManager.ApplyClips(ref __instance);
                return;
            }
            AudioManager.OnAllSongsLoaded += delegate
            {
                AudioManager.ApplyClips(ref __instance);
            };
            AudioManager.SetTooltip(ref __instance);
        }

        [HarmonyPatch("StartMusic")]
        [HarmonyPostfix]
        private static void StartMusic_Postfix(BoomboxItem __instance)
        {
            AudioManager.SetTooltip(ref __instance);
            AudioManager.SetVolume(ref __instance);
        }

        [HarmonyPatch("PocketItem")]
        [HarmonyPostfix]
        private static void PocketItem_Postfix(BoomboxItem __instance)
        {
            AudioManager.ApplyClips(ref __instance);

        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void Update_Postfix(BoomboxItem __instance)
        {
            if (__instance.isHeld)
            {
                if (AudioManager.lowerVolume.IsPressed())
                {
                    AudioManager.ModifyVolume(false, ref __instance);
                }
                else if (AudioManager.increaseVolume.IsPressed())
                {
                    AudioManager.ModifyVolume(true, ref __instance);
                }
            }
        }
    }
}
