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
    internal class SunnyCommando : MonoBehaviour
    {
        public SuperSnowGatling plant
        {
            get
            {
                return base.gameObject.GetComponent<SuperSnowGatling>();
            }
        }
        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(0);
        }

        public Bullet AnimShoot()
        {
            Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y + 0.1f, this.plant.thePlantRow, BulletType.Bullet_smallSun, 0);

            bullet.Damage = 100;
            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
