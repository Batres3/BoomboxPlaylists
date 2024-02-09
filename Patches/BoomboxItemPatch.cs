using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BoomboxPlaylists.Managers;
using UnityEngine;
using System.Reflection;

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
        [HarmonyPrefix]
        private static void StartMusic_Postfix(BoomboxItem __instance, bool startMusic)
        {
            AudioManager.SetVolume(ref __instance);
            AudioManager.NextSong(ref __instance, ref startMusic);
        }

        [HarmonyPatch("PocketItem")]
        [HarmonyPrefix]
        private static bool PocketItem_Postfix(BoomboxItem __instance)
        {
            AudioManager.ApplyClips(ref __instance);
            AudioManager.SetTooltip(ref __instance);
            GrabbableObject component = __instance.GetComponent<GrabbableObject>();
            if (component != null)
            {
                component.EnableItemMeshes(enable: false);
            }
            return false;
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
