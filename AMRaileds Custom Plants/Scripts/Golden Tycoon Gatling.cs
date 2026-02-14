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
using Il2CppInterop.Runtime.Attributes;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class GoldenTycoonGatling : MonoBehaviour
    {
        public Shooter plant
        {
            get
            {
                return base.gameObject.GetComponent<Shooter>();
            }
        }
        public static int buff1 = -1;
        public static int buff2 = -1;

        [HideFromIl2Cpp]
        public System.Collections.IEnumerator SuperSkill()
        {
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    if (this.plant is not null && Board.Instance is not null)
                    {
                        Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
                        Bullet bullet = CreateBullet.Instance.SetBullet(position.x, position.y, this.plant.thePlantRow, BulletType.Bullet_goldCoin, BulletMoveWay.MoveRight);
                        bullet.Damage = 500 + Mathf.FloorToInt(Board.Instance.theMoney / 100);
                        Bullet bullet2 = CreateBullet.Instance.SetBullet(position.x, position.y, this.plant.thePlantRow+1, BulletType.Bullet_goldCoin, BulletMoveWay.Three_down);
                        bullet2.Damage = 500 + Mathf.FloorToInt(Board.Instance.theMoney / 100);
                        Bullet bullet3 = CreateBullet.Instance.SetBullet(position.x, position.y, this.plant.thePlantRow-1, BulletType.Bullet_goldCoin, BulletMoveWay.Three_up);
                        bullet3.Damage = 500 + Mathf.FloorToInt(Board.Instance.theMoney / 100);
                    }
                    else
                    {
                        break;
                    }
                }
                catch { break; }
                yield return new WaitForSeconds(0.005f);
            }
        }
        public Bullet AnimShoot()
        {
            Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, BulletType.Bullet_goldCoin, 0);

            bullet.Damage = 8713;
            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
