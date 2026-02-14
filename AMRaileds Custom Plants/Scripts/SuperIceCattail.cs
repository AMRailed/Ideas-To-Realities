using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class SuperIceCattail : MonoBehaviour
    {
        public IceCattail plant
        {
            get
            {
                return base.gameObject.GetComponent<IceCattail>();
            }
        }
        public int shots = 0;

        public void AnimShoot()
        {
            shots++;
            if (Lawnf.TravelAdvanced(buff1) && shots%20==0)
            {
                Bullet bullet = CreateBullet.Instance.SetBullet(plant.transform.position.x, plant.transform.position.y + 0.5f, plant.thePlantRow, BulletType.Bullet_iceBlock_big, BulletMoveWay.Track);
                bullet.trackSpeed = 3;
                bullet.Damage = 360;
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }
}
