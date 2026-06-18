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
using Il2CppInterop.Runtime.Attributes;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class FrostUmbrella : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public static bool SBlockEffect(CabbageUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)807 && !zombie.isMindControlled)
            {
                zombie.SetCold(4);
                if ((new System.Random()).Next(1, 10) == 1)
                {
                    zombie.SetFreeze(4);
                }
                zombie.KnockBack(1.5f * (__instance.UmbrellaPot is not null ? 2 : 1));
                return false;
            }
            return true;
        }

        public CabbageUmbrella plant
        {
            get
            {
                return base.gameObject.GetComponent<CabbageUmbrella>();
            }
        }
    }

    [HarmonyPatch(typeof(CabbageUmbrella), "BlockEffect")]
    public static class CabbageUmbrellaPatch
    {
        public static bool Prefix(CabbageUmbrella __instance, ref Zombie zombie)
        {
            return FrostUmbrella.SBlockEffect(__instance, ref zombie) && UmbrellaMine.SBlockEffect(__instance, ref zombie);
        }
    }
}
