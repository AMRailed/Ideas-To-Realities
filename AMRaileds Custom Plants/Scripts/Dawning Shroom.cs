using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class DawningShroom : MonoBehaviour
    {
        public FumeShroom plant
        {
            get
            {
                return base.gameObject.GetComponent<FumeShroom>();
            }
        }

        public Bullet AnimShoot()
        {
            Vector3 position = base.transform.Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, BulletType.Bullet_smallSun, 0);

            bullet.theBulletRow = this.plant.thePlantRow;

            return bullet;
        }
    }
}
