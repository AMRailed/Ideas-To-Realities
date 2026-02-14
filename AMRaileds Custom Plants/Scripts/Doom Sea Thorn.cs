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

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class DoomSeaThorn : MonoBehaviour
    {
        public Shooter plant
        {
            get
            {
                return base.gameObject.GetComponent<Shooter>();
            }
        }
        public int growLevel = 0;
        public float growInterval = 120;

        public void Start()
        {
            this.growLevel = 0;
            this.growInterval = 10;
            this.plant.transform.localScale = Vector3.one / 2;
        }

        public void Update()
        {
            this.growInterval -= Time.deltaTime;
            if (this.growInterval <= 0 && this.growLevel < 1)
            {
                this.growLevel += 1;
                this.plant.transform.localScale = Vector3.one * Time.deltaTime;
                if (this.plant.transform.localScale.x >= 1)
                {
                    this.plant.transform.localScale = Vector3.one;
                }
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
