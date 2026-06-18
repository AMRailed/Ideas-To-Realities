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
    internal class Obsidian_Pea : MonoBehaviour
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
            Vector3 position = base.transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y + 0.3f, this.plant.thePlantRow, BulletType.Bullet_pea, 0);

            bullet.Damage = 600;
            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
