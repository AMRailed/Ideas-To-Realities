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
using Unity.Profiling;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class TwinTycoonShooter : MonoBehaviour
    {
        public Shooter plant
        {
            get
            {
                return base.gameObject.GetComponent<Shooter>();
            }
        }

        public void Update()
        {
            this.plant.thePlantProduceInterval -= Time.deltaTime;
            if (this.plant.thePlantProduceInterval <= 0 )
            {
                int rand = (new System.Random()).Next(1, 20);
                if (rand <= 16)
                {
                    CreateItem.Instance.SetCoin(this.plant.thePlantColumn, this.plant.thePlantRow, 34, 0);
                }else if (rand <= 19)
                {
                    CreateItem.Instance.SetCoin(this.plant.thePlantColumn, this.plant.thePlantRow, 35, 0);
                }
                else
                {
                    CreateItem.Instance.SetCoin(this.plant.thePlantColumn, this.plant.thePlantRow, 36, 0);
                }
                this.plant.thePlantProduceInterval = 25;
            }
        }

        public void AnimShoot()
        {
            Vector3 position = base.transform.Find("Shoot").transform.position;
            Bullet bullet;
            if (Board.Instance.theMoney < 8000)
            {
                bullet = CreateBullet.Instance.SetBullet(position.x, position.y, plant.thePlantRow, BulletType.Bullet_silverCoin, (BulletMoveWay)0);
            }else
            {
                bullet = CreateBullet.Instance.SetBullet(position.x, position.y, plant.thePlantRow, BulletType.Bullet_goldCoin, (BulletMoveWay)0);
            }
            bullet.Damage = 20 + Mathf.FloorToInt(Board.Instance.theMoney/100);
        }
    }
}
