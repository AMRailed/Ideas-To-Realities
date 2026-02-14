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
using Il2CppInterop.Runtime.Attributes;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class MagmaShroom : MonoBehaviour
    {
        public Shooter plant
        {
            get
            { 
                return base.gameObject.GetComponent<Shooter>();
            }
        }
        [HideFromIl2Cpp]
        public System.Collections.IEnumerator Shoot()
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    if (this.plant is not null && Board.Instance is not null)
                    {
                        Vector3 position = base.transform.Find("Shoot").transform.position;
                        Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, BulletType.Bullet_firePea_purple, 0);

                        bullet.penetrationTimes = -1000;
                        bullet.Damage = 60;
                    }
                    else
                    {
                        break;
                    }
                }
                catch { break; }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void AnimShoot()
        {
            MelonCoroutines.Start(this.Shoot());
        }
    }
}
