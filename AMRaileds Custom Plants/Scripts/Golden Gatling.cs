using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using MelonLoader;
using UnityEngine;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class GoldenGatling : MonoBehaviour
    {
        public Shooter plant
        {
            get
            {
                return base.gameObject.GetComponent<Shooter>();
            }
        }

        [HideFromIl2Cpp]
        public System.Collections.IEnumerator SuperSkill()
        {
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    if (this.plant is not null && Board.Instance is not null)
                    {
                        Vector3 position = this.plant.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
                        Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y + ((float)(new System.Random()).Next(-3, 3)) / 10, this.plant.thePlantRow, BulletType.Bullet_goldCoin, 0);

                        bullet.Damage = 200;
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

            bullet.Damage = 120;
            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
