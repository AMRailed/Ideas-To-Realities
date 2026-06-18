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
    internal class Frenzerg : MonoBehaviour
    {
        public ScaredyDoom plant
        {
            get
            { 
                return base.gameObject.GetComponent<ScaredyDoom>();
            }
        }

        public Bullet AnimShoot()
        {
            Vector3 position = base.transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, BulletType.Bullet_iceDoom, 0);

            bullet.theBulletRow = this.plant.thePlantRow;
            System.Random rnd = new System.Random();
            if (rnd.Next(1, 10) == 1)
            {
                Bullet bullet2 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y + 0.4f, this.plant.thePlantRow, BulletType.Bullet_iceDoom, 0);
                Bullet bullet3 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y - 0.4f, this.plant.thePlantRow, BulletType.Bullet_iceDoom, 0);
            }

            return bullet;
        }
    }
}
