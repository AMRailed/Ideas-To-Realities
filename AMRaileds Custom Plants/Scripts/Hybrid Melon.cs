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

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class Hybrid_Melon : MonoBehaviour
    {
        public WinterMelon plant
        {
            get
            {
                return base.gameObject.GetComponent<WinterMelon>();
            }
        }
        
        public void AnimShoot()
        {
            if (Lawnf.TravelAdvanced(Hybrid_Melon.buff1))
            {
                plant.thePlantAttackCountDown = 0.5f;
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }

    [HarmonyPatch(typeof(Bullet_winterMelon))]
    public static class Bullet_winterMelonPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_winterMelon __instance, Zombie zombie)
        {
            if (__instance is not null && __instance.theBulletType == BulletType.Bullet_winterMelon && __instance.Damage == 300)
            {
                if (Lawnf.TravelAdvanced(Hybrid_Melon.buff1))
                {
                    CreatePlant.Instance.SetPlant(Mouse.Instance.GetColumnFromX(zombie.transform.position.x), zombie.theZombieRow, (PlantType)266);
                    if ((new System.Random()).Next(1, 5) <= 1)
                    {
                        GameObject success = null;
                        for (int x = -1; x < 2; x++)
                        {
                            for (int y = -1; y < 2; y++)
                            {
                                success = CreatePlant.Instance.SetPlant(Mouse.Instance.GetColumnFromX(zombie.transform.position.x) + x, zombie.theZombieRow + y, (PlantType)266);
                                if (success is not null) { break; }
                            }
                            if (success is not null) { break; }
                        }
                    }
                }
                else
                {
                    CreatePlant.Instance.SetPlant(Mouse.Instance.GetColumnFromX(zombie.transform.position.x), zombie.theZombieRow, (PlantType)266);
                }
            }
        }
    }
}
