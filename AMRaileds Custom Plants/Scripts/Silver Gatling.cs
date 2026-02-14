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
    internal class SilverGatling : MonoBehaviour
    {
        public Shooter plant
        {
            get
            {
                return base.gameObject.GetComponent<Shooter>();
            }
        }

        public Bullet AnimShoot()
        {
            Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, BulletType.Bullet_silverCoin, 0);

            bullet.Damage = 40;
            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
