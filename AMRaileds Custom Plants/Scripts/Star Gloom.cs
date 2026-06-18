using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class StarGloom : MonoBehaviour
    {
        public GloomShroom plant { get { return base.gameObject.GetComponent<GloomShroom>(); } }

        public void Start()
        {
            if (GameAPP.theGameStatus == 0)
            {
                plant.center = gameObject.transform.FindChild("Shoot").gameObject;
            }
        }

        public void AnimShoot()
        {
            var centerPos = plant.center.transform.position;
            ParticleManager.Instance.SetParticle((ParticleType)200, plant.center.transform.position);

            for (int i = 0; i < 8; i++)
            {
                Bullet bullet = CreateBullet.Instance.SetBullet(centerPos.x, centerPos.y, 0, BulletType.Bullet_star, BulletMoveWay.Free);
                bullet.transform.Rotate(0, 0, i * 45);
                bullet.Damage = 20;
            }
        }
    }
}
