using Il2Cpp;
using MelonLoader;
using HarmonyLib;
using UnityEngine;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class SuperFireCattail : MonoBehaviour
    {
        public FireCattail plant
        {
            get
            {
                return base.gameObject.GetComponent<FireCattail>();
            }
        }

        public void AnimShoot()
        {

        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }

    [HarmonyPatch(typeof(Bullet_fireTrack))]
    public static class Bullet_fireTrackPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static bool PreHitZombie(Bullet_fireTrack __instance, Zombie zombie)
        {
            if (__instance.Damage == 180)
            {
                if (Lawnf.TravelAdvanced(SuperFireCattail.buff2)) zombie.TakeDamage(DmgType.NormalAll, __instance.Damage * 4);
                zombie.SetJalaed();
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 1.6f);
                var bulletRow = Mouse.Instance.GetRowFromY(pos.x, pos.y);
                ParticleManager.Instance.SetParticle(ParticleType.JalaedCloudSmall, new(pos.x, pos.y));
                if (Lawnf.TravelAdvanced(SuperFireCattail.buff1) && (new System.Random()).Next(1, 20) == 1)
                {
                    MelonCoroutines.Start(Plugin.FireOcean(new(pos.x, pos.y)));
                }
                for (int i = 0; i < 8; i++)
                {
                    Bullet bullet = CreateBullet.Instance.SetBullet(pos.x, pos.y, bulletRow, BulletType.Bullet_fireTrack, BulletMoveWay.Free);
                    bullet.transform.Rotate(0, 0, i * 45);
                    bullet.penetrationTimes = 10;
                    bullet.Damage = 120;
                }
                foreach (var z in array)
                {
                    if (z != null && z.gameObject.TryGetComponent<Zombie>(out var otherZ) && (otherZ.theZombieRow == bulletRow || otherZ.theZombieRow == bulletRow + 1 || otherZ.theZombieRow == bulletRow - 1))
                    {
                        if (otherZ.isJalaed) otherZ.TakeDamage(DmgType.NormalAll, 300);
                        otherZ.SetJalaed();
                    }
                }
            }
            else if (__instance.Damage == 120 && __instance.theMovingWay == (int)BulletMoveWay.Free)
            {
                return true;
            }
            return false;
        }
    }
}
