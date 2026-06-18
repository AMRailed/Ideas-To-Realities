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
    internal class Pea_Turret : MonoBehaviour
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
            this.plant.thePlantAttackInterval = Math.Clamp(this.plant.thePlantAttackInterval-0.1f, 0.01f, 1.5f);
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y+0.3f, this.plant.thePlantRow, 0, 0);
            Bullet bullet2 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, 0, 0);
            Bullet bullet3 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y-0.3f, this.plant.thePlantRow, 0, 0);

            bullet.theBulletRow = this.plant.thePlantRow;
            bullet2.theBulletRow = this.plant.thePlantRow;
            bullet3.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
