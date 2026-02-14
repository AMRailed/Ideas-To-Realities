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
    internal class Calamity_Pea : MonoBehaviour
    {
        public static int Buff1 { get; set; } = -1;
        public Shooter plant
        {
            get
            {
                return base.gameObject.GetComponent<Shooter>();
            }
        }

        public Bullet AnimShoot()
        {
            /*if (Buff1 > 0)
            {
                this.plant.thePlantAttackCountDown = 5;
            }else
            {
                this.plant.thePlantAttackCountDown = 10;
            }*/
            CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, PlantType.SuperHypnoDoom, null, default, true);
            Vector3 position = base.transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y + 0.3f, this.plant.thePlantRow, BulletType.Bullet_doom, 0);

            bullet.Damage = 1800;
            bullet.theBulletRow = this.plant.thePlantRow;

            CreatePlant.Instance.SetPlant(plant.thePlantColumn + 1, plant.thePlantRow, PlantType.SuperHypnoDoom, null, default, true);

            return bullet;
        }
    }
}
