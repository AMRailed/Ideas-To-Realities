using Il2Cpp;
using Il2CppSystem;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class MegaDoomCabbage : MonoBehaviour
    {
        public Plant plant { get { return base.gameObject.GetComponent<Plant>(); } }

        public static void ChangeSprite(Bullet bullet)
        {
            if (bullet.Damage == 600)
            {
                SpriteRenderer comp;
                if (bullet.transform.GetChild(0).TryGetComponent<SpriteRenderer>(out comp))
                {
                    comp.sprite = Plugin.DoomCabbageBullet;
                    comp.color = new Color(0.986f, 1, 1);
                }
            }
        }


        [HarmonyPatch(typeof(Bullet_cabbage))]
        public static class Bullet_cabbagePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("RotateUpdate")]
            public static void PostRotateUpdate(Bullet_cabbage __instance)
            {
                if (__instance.Damage == 600)
                {
                    MegaDoomCabbage.ChangeSprite(__instance);
                }
            }
            [HarmonyPrefix]
            [HarmonyPatch("HitZombie")]
            public static void PreHitZombie(Bullet_cabbage __instance, Zombie zombie)
            {
                if (__instance.Damage == 600)
                {
                    if (Lawnf.TravelUltimate(23))
                    {
                        zombie.TakeDamage(DmgType.NormalAll, 5400);
                    }
                    var pos = __instance.transform.position;
                    ParticleManager.Instance.SetParticle(ParticleType.BombCloud_vision_doom, pos);

                    var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3);
                    foreach (var z in array)
                    {
                        if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zomb) && (zomb.theZombieRow == __instance.theBulletRow || zomb.theZombieRow == __instance.theBulletRow + 1 || zomb.theZombieRow == __instance.theBulletRow - 1))
                        {
                            if (Lawnf.TravelUltimate(23))
                            {
                                zomb.TakeDamage(DmgType.NormalAll, 1500);
                            }
                            else
                            {
                                zomb.TakeDamage(DmgType.NormalAll, 600);
                            }
                            zomb.AddEmberScore();

                            if (Lawnf.TravelUltimate(22))
                            {
                                Plugin.printString(((5 + zomb.GetEmberScore() * 2)).ToString());
                                if ((new Il2CppSystem.Random()).Next(0, 100) <= (5 + zomb.GetEmberScore() * 2))
                                {
                                    Board.Instance.SetDoom(zomb.Column, zomb.theZombieRow, false, default, zomb.transform.position, 12800, default, default, default, (PlantType)846);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
