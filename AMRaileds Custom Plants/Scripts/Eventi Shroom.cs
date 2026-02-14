using Il2Cpp;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace AMRaileds_Custom_Plants
{
    [HarmonyPatch(typeof(Bullet_doom))]
    public static class DoomBulletPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void HitZombieFix(Bullet_doom __instance, Zombie zombie)
        {
            if (__instance.Damage == 20)
            {
                System.Random rnd = new System.Random();
                if (rnd.Next(1, 10) == 1)
                {
                    if (zombie is not null && !TypeMgr.IsAirZombie(zombie.theZombieType) && !zombie.isMindControlled)
                    {
                        if (zombie.isDoom)
                        {
                            zombie.TakeDamage(DmgType.NormalAll, 300);
                        }
                        zombie.isDoom = true;
                        zombie.doomWithPit = false;
                    }
                }
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    internal class EventiShroom : MonoBehaviour
    {
        public FumeShroom plant
        {
            get
            {
                return base.gameObject.GetComponent<FumeShroom>();
            }
        }

        public Bullet AnimShoot()
        {
            for (int j = Lawnf.GetAllPlants().Count - 1; j >= 0; j--)
            {
                Plant p = Lawnf.GetAllPlants()[j];
                if (p != null)
                {
                    if (p.thePlantColumn == this.plant.thePlantColumn - 1 && p.thePlantRow == this.plant.thePlantRow && p.thePlantType == PlantType.EndoFlameGirl)
                    {
                        Board.Instance.SetDoom(p.thePlantColumn, p.thePlantRow, true, default, default, 300);
                    }
                }
            }
            Vector3 position = base.transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, BulletType.Bullet_doom, 0);

            bullet.Damage = 20;
            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
        public static int BulletId { get; set; } = 23;
    }
}
