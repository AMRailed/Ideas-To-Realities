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

namespace AMRaileds_Custom_Plants
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
}
