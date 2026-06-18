using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using MelonLoader;
using Unity.Mathematics;
using UnityEngine;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class SuperFireGloom : MonoBehaviour
    {
        public FireGloom plant { get { return base.gameObject.GetComponent<FireGloom>(); } }

        public int shots = 0;
        public int frenzyPoints = 0;
        public bool frenzyMode = false;

        public void Start()
        {
            if (GameAPP.theGameStatus == 0)
            {
                plant.range = 4f;
                plant.center = gameObject.transform.FindChild("Shoot").gameObject;
            }
        }

        [HideFromIl2Cpp]
        public System.Collections.IEnumerator Frenzy()
        {
            frenzyMode = true;
            yield return new WaitForSeconds(8f);
            frenzyMode = false;
        }

        public void AnimShoot()
        {
            if (frenzyMode)
            {
                ParticleManager.Instance.SetParticle(ParticleType.LightningArcExplosion, plant.center.transform.position);
                plant.attackDamage = 240;
            }else { plant.attackDamage = 120; }
                ParticleManager.Instance.SetParticle((ParticleType)118, plant.center.transform.position);
            shots++;
            if (Lawnf.TravelAdvanced(buff2) && !frenzyMode)
            {
                frenzyPoints++;
            }
            if (shots%12==0)
            {
                ParticleManager.Instance.SetParticle(ParticleType.BombCloud_vision_doom, plant.center.transform.position);
                foreach (var zombie in Board.Instance.zombieArray)
                {
                    if (zombie != null)
                    {
                        var distance = (zombie.transform.position - plant.center.transform.position).magnitude;
                        var damage = (int)(900 - (distance * 50));
                        if (Lawnf.TravelAdvanced(buff1))
                        {
                            damage = (int)(1800 - (distance * 12));
                        }
                        zombie.TakeDamage(DmgType.NormalAll, damage);
                        zombie.AddEmberScore();
                    }
                }
            }
            if (frenzyPoints>=36)
            {
                ParticleManager.Instance.SetParticle(ParticleType.LightningArcExplosion, plant.center.transform.position);
                frenzyPoints = 0;
                MelonCoroutines.Start(Frenzy());
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }


    [HarmonyPatch(typeof(DoomBlover))]
    public static class DoomBloverPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("BlowEffect")]
        public static void PreBlowEffect(DoomBlover __instance)
        {
            //Do more damage
            foreach (var zombie in Board.Instance.zombieArray)
            {
                if (zombie != null)
                {
                    zombie.AddEmberScore();
                    zombie.TakeDamage(DmgType.NormalAll, 30 * zombie.GetEmberScore());
                }   
            }
        }
    }
    [HarmonyPatch(typeof(DoomFume))]
    public static class DoomFumePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AnimShoot")]
        public static void PreAnimShoot(DoomFume __instance)
        {
            //Do knockback and shorten cooldown
            foreach (var zombie in Board.Instance.zombieArray)
            {
                if (zombie != null && zombie.theZombieRow == __instance.thePlantRow && zombie.transform.position.x > __instance.transform.position.x)
                {
                    zombie.AddEmberScore();
                    zombie.KnockBack(math.clamp(0.5f * zombie.GetEmberScore(), 0.5f, 6f));
                }
            }
        }
    }
    [HarmonyPatch(typeof(DoomChomper))]
    public static class DoomChomperPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Chomp")]
        public static void PreChomp(DoomChomper __instance, Zombie zombie)
        {
            //Shorten Chew Time
            zombie.SetEmbered();
            __instance.swallowMaxCountDown = math.clamp(30 - zombie.GetEmberScore() * 5, 5, 30);
        }
    }
    [HarmonyPatch(typeof(Bullet_doom))]
    public static class Bullet_doomPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_doom __instance, Zombie zombie)
        {
            //Increase contact damage
            if (__instance.theBulletType == BulletType.Bullet_pea_doom)
            {
                zombie.TakeDamage(DmgType.NormalAll, 5 * zombie.GetEmberScore());
                zombie.AddEmberScore();
                //var bullet = CreateBullet.Instance.SetBullet(__instance.transform.position.x, __instance.transform.position.y, __instance.theBulletRow + 1, BulletType.Bullet_seaStar, BulletMoveWay.MoveRight);
                //Plugin.printString(bullet.ToString());
            }
            if (__instance.theBulletType == BulletType.Bullet_doom)
            {
                zombie.AddEmberScore();
            }
            if (__instance.theBulletType == BulletType.Bullet_doom_big)
            {
                zombie.AddEmberScore(5);
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_seaStar))]
    public static class Bullet_seaStarPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_seaStar __instance, Zombie zombie)
        {
            if (__instance.theBulletType == BulletType.Bullet_seaStar)
            {
                zombie.AddEmberScore();
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_blackPuff))]
    public static class Bullet_blackPuffPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_blackPuff __instance, Zombie zombie)
        {
            //Cause a small explosion
            if (__instance.theBulletType == BulletType.Bullet_blackPuff)
            {
                if (zombie.GetEmberScore() > 5)
                {
                    ParticleManager.Instance.SetParticle(ParticleType.DoomSplat, zombie.transform.position + UnityEngine.Vector3.up * 2f);
                    var reqDistance = math.clamp(0.3 * zombie.GetEmberScore(), 0.5, 2.5);
                    foreach (Zombie otZombie in Board.Instance.zombieArray)
                    {
                        if (otZombie != null)
                        {
                            var distance = (otZombie.transform.position + UnityEngine.Vector3.up * 2f - (__instance.transform.position - UnityEngine.Vector3.down * 0.5f)).magnitude;
                            if (distance <= reqDistance)
                            {
                                otZombie.TakeDamage(DmgType.NormalAll, 20 + math.clamp(5 * zombie.GetEmberScore(), 5, 30));
                            }
                        }
                    }
                }
                zombie.AddEmberScore();
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_doomCactus))]
    public static class Bullet_doomCactusPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_doomCactus __instance, Zombie zombie)
        {
            //Increase ember points
            if (__instance.theBulletType == BulletType.Bullet_doomCactus)
            {
                zombie.AddEmberScore();
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_iceDoom))]
    public static class Bullet_iceDoomPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_iceDoom __instance, Zombie zombie)
        {
            //Increase ember points
            if (__instance.theBulletType == BulletType.Bullet_iceDoom && __instance.hitTimes < 2)
            {
                __instance.penetrationTimes = math.clamp(__instance.penetrationTimes + zombie.GetEmberScore(), 3, 7);
            }
        }
    }
    [HarmonyPatch(typeof(UltimateFume))]
    public static class UltimateFumePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AttackZombie")]
        public static void PreAttackZombie(UltimateFume __instance)
        {
            //Knockback
            foreach (var z in Board.Instance.zombieArray)
            {
                if (z != null && !z.isMindControlled && !TypeMgr.IsAirZombie(z.theZombieType) && z.theZombieRow == __instance.thePlantRow && z.transform.position.x > __instance.transform.position.x)
                {
                    z.KnockBack(math.clamp(0.025f * (z.GetEmberScore() - 5), 0.025f, 0.1f), Zombie.KnockBackReason.ByIronPea);
                    z.AddEmberScore();
                }
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_firePea_purple))]
    public static class Bullet_firePea_purplePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("HitZombie")]
        public static void PostHitZombie(Bullet_firePea_purple __instance, Zombie zombie)
        {
            if (__instance.Damage == 60)
            {
                if (zombie.isJalaed)
                {
                    if ((new System.Random()).Next(1, 3) == 1)
                    {
                        Bullet bullet = CreateBullet.Instance.SetBullet(__instance.transform.position.x, __instance.transform.position.y, __instance.theBulletRow + 1, BulletType.Bullet_firePea_purple, BulletMoveWay.Three_down);
                    }
                    else
                    {
                        Bullet bullet = CreateBullet.Instance.SetBullet(__instance.transform.position.x, __instance.transform.position.y, __instance.theBulletRow - 1, BulletType.Bullet_firePea_purple, BulletMoveWay.Three_up);
                    }
                }
                zombie.AddEmberScore();
                zombie.SetJalaed();
            }
        }
    }
}
