using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using Il2Cpp;
using HarmonyLib;
using CustomizeLib;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [HarmonyPatch(typeof(DoomShroom))]
    public static class DoomShroomPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AnimExplode")]
        public static void PreAnimExplode(DoomShroom __instance)
        {
            if (__instance.thePlantType == (PlantType)1918)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 9f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled)
                    {
                        Board.Instance.SetDoom(__instance.thePlantColumn, zombie.theZombieRow, false, default, zombie.transform.position);
                    }
                }
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    internal class Clustroom : MonoBehaviour
    {
        public DoomShroom plant
        {
            get
            { 
                return base.gameObject.GetComponent<DoomShroom>();
            }
        }
    }
}
