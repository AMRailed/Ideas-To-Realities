using Il2Cpp;
using MelonLoader;
using HarmonyLib;
using UnityEngine;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class SuperIceCattail : MonoBehaviour
    {
        public IceCattail plant
        {
            get
            {
                return base.gameObject.GetComponent<IceCattail>();
            }
        }
        public int shots = 0;

        public void AnimShoot()
        {
            shots++;
            if (Lawnf.TravelAdvanced(buff1) && shots%20==0)
            {
                Bullet bullet = CreateBullet.Instance.SetBullet(plant.transform.position.x, plant.transform.position.y + 0.5f, plant.thePlantRow, BulletType.Bullet_iceBlock_big, BulletMoveWay.Track);
                bullet.trackSpeed = 3;
                bullet.Damage = 360;
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }

    [HarmonyPatch(typeof(Bullet_iceSpark))]
    public static class Bullet_iceSparkPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static bool PreHitZombie(Bullet_iceSpark __instance, Zombie zombie)
        {
            if (__instance.Damage == 80 && __instance.theMovingWay == (int)BulletMoveWay.Free)
            {
                var pos = __instance.transform.position;
                var bulletRow = Mouse.Instance.GetRowFromY(pos.x, pos.y);
                if (Lawnf.TravelAdvanced(SuperIceCattail.buff2) && (new System.Random()).Next(1, 100) <= 37)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Bullet bullet = CreateBullet.Instance.SetBullet(pos.x, pos.y, bulletRow, BulletType.Bullet_iceSpark, BulletMoveWay.Free);
                        bullet.transform.Rotate(0, 0, i * 90);
                        bullet.penetrationTimes = 3;
                        bullet.Damage = 30;
                    }
                }
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Bullet_iceTrack))]
    public static class Bullet_iceTrackPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static bool PreHitZombie(Bullet_iceTrack __instance, Zombie zombie)
        {
            if (__instance.Damage == 120)
            {
                zombie.SetCold(8, 1);
                zombie.AddfreezeLevel(15);
                var pos = __instance.transform.position;
                var bulletRow = Mouse.Instance.GetRowFromY(pos.x, pos.y);
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 1.6f);
                ParticleManager.Instance.SetParticle(ParticleType.IceBallExplode, new(pos.x, pos.y));
                for (int i = 0; i < 8; i++)
                {
                    Bullet bullet = CreateBullet.Instance.SetBullet(pos.x, pos.y, bulletRow, BulletType.Bullet_iceSpark, BulletMoveWay.Free);
                    bullet.transform.Rotate(0, 0, i * 45);
                    bullet.penetrationTimes = 3;
                    bullet.Damage = 80;
                }
                foreach (var z in array)
                {
                    if (z != null && z.gameObject.TryGetComponent<Zombie>(out var otherZ) && (otherZ.theZombieRow == bulletRow || otherZ.theZombieRow == bulletRow + 1 || otherZ.theZombieRow == bulletRow - 1))
                    {
                        otherZ.TakeDamage(DmgType.IceAll, 200);
                        otherZ.SetCold(8, 1);
                        otherZ.AddfreezeLevel(5);
                    }
                }
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Bullet_iceBlock_big))]
    public static class Bullet_iceBlock_bigPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("HitZombie")]
        public static void PostHitZombie(Bullet_iceBlock_big __instance, Zombie zombie)
        {
            if (__instance.Damage == 360)
            {
                zombie.SetFreeze(12, 1);
                var pos = __instance.transform.position;
                var bulletRow = Mouse.Instance.GetRowFromY(pos.x, pos.y);
                Board.Instance.CreateCherryExplode(new(pos.x, pos.y), bulletRow, CherryBombType.IceCharry, 900);
            }
        }
    }
}
