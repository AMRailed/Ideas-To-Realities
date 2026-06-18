using CustomizeLib;
using CustomizeLib.MelonLoader;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class MelonadeMortar : MonoBehaviour
    {
        public Thrower plant
        {
            get
            {
                return base.gameObject.GetComponent<Thrower>();
            }
        }
        public static int buff1 = -1;
        public static int buff2 = -1;
        public void AnimShoot()
        {
            if (Lawnf.TravelAdvanced(MelonadeMortar.buff1))
            {
                plant.thePlantAttackCountDown = 0.5f;
                plant.thePlantAttackInterval = 0.5f;
            }
            Vector3 position1 = base.transform.Find("Head").Find("Golden_Barrel-1").Find("Shoot").transform.position;
            Vector3 position2 = base.transform.Find("Head").Find("Golden_Barrel-2").Find("Shoot").transform.position;
            Vector3 position3 = base.transform.Find("Cannon").Find("Shoot").transform.position;

            this.plant.attributeCount += 1;

            for (int i=1; i<5; i++)
            {
                var pos = position1;
                if (i > 2)
                {
                    pos = position2;
                }
                Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(pos.x + 0.1F, pos.y + (float)((((float)i+1)%2)/10), this.plant.thePlantRow, BulletType.Bullet_extremeSnowPea, 0);

                bullet.Damage = 40;
                bullet.theBulletRow = this.plant.thePlantRow;

                if (this.plant.attributeCount >= 3 || Lawnf.TravelAdvanced(MelonadeMortar.buff1))
                {
                    Zombie zombie = this.plant.ThrowerSearchZombie();
                    Bullet bullet2 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position3.x + 0.1F, position3.y, this.plant.thePlantRow, BulletType.Bullet_goldMelon, 13);

                    bullet2.detaVy = (-(float)((float)i)) + 1.2f;
                    bullet2.Vx = 12;
                    bullet2.Damage = 140;
                    bullet2.theBulletRow = this.plant.thePlantRow;
                }
            }
            if (this.plant.attributeCount >= 3)
            {
                this.plant.attributeCount = 0;
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_cannon))]
    public static class Bullet_cannonPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitLand")]
        public static bool PreHitLand(Bullet_cannon __instance)
        {
            if (__instance.theBulletType == BulletType.Bullet_goldMelonCannon && __instance.Damage == 240)
            {
                CreateParticle.SetParticle(71, new(__instance.cannonPos.x, __instance.cannonPos.y), __instance.theBulletRow);
                var pos = __instance.transform.position;
                LayerMask layermask = __instance.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 1f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled)
                    {
                        if (Lawnf.TravelAdvanced(MelonadeMortar.buff2) && zombie.GetTotalHealth() <= 1200)
                        {
                            zombie.TakeDamage(DmgType.IceShieldless, 5000);
                        }
                        zombie.TakeDamage(DmgType.IceAll, 240);
                        zombie.AddfreezeLevel(10);
                        zombie.SetCold(8);
                        if ((new System.Random()).Next(1, 4) <= 1)
                        {
                            CreateItem.Instance.SetCoin(Mouse.Instance.GetColumnFromX(zombie.transform.position.x), zombie.theZombieRow, 39, 0);

                        }
                        else
                        {
                            CreateItem.Instance.SetCoin(Mouse.Instance.GetColumnFromX(zombie.transform.position.x), zombie.theZombieRow, 38, 0);
                        }
                    }
                }
                GameAPP.PlaySound(UnityEngine.Random.RandomRangeInt(104, 106));
                __instance.Die();
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Bullet_silverMelon))]
    public static class Bullet_silverMelonPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_silverMelon __instance, Zombie zombie)
        {
            if (__instance is not null && __instance.theBulletType == BulletType.Bullet_goldMelon && __instance.Damage == 140)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 1f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var tzombie))
                    {
                        if (tzombie.theZombieRow == __instance.theBulletRow || tzombie.theZombieRow == __instance.theBulletRow + 1 || tzombie.theZombieRow == __instance.theBulletRow - 1)
                        {
                            tzombie.AddfreezeLevel(10);
                            tzombie.SetCold(8);
                            CreateItem.Instance.SetCoin(Mouse.Instance.GetColumnFromX(tzombie.transform.position.x), tzombie.theZombieRow, 38, 0);
                        }
                    }
                }
            }
        }
    }
}
