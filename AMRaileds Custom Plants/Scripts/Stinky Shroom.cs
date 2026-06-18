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
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class StinkyShroom : MonoBehaviour
    {
        public SmallPuff plant
        {
            get
            {
                return base.gameObject.GetComponent<SmallPuff>();
            }
        }
        public Bullet AnimShoot()
        {
            Vector3 position = base.transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y + 0.1f, this.plant.thePlantRow, BulletType.Bullet_garlicKernal, 0);

            bullet.Damage = 20;
            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
