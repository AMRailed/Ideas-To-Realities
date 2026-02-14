using Il2Cpp;
using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime.Attributes;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    public class SuperFireStar : MonoBehaviour
    {
        public StarFruit plant { get { return base.gameObject.GetComponent<StarFruit>(); } }

        public int shots = 0;
        public int frenzyPoints = 0;
        public bool frenzyMode = false;

        [HideFromIl2Cpp]
        public System.Collections.IEnumerator Frenzy()
        {
            frenzyMode = true;
            yield return new WaitForSeconds(8f);
            frenzyMode = false;
        }

        public void AnimShoot()
        {
            var center = plant.transform.position + Vector3.up * 0.5f;
            if (frenzyMode)
            {
                ParticleManager.Instance.SetParticle(ParticleType.LightningArcExplosion, center);
                plant.attackDamage = 720;
            }else { plant.attackDamage = 360; }

            var bul1 = CreateBullet.Instance.SetBullet(center.x, center.y, plant.thePlantRow, bullet, BulletMoveWay.Free);
            bul1.transform.Rotate(0, 0, 90);
            bul1.Damage = plant.attackDamage;
            var bul2 = CreateBullet.Instance.SetBullet(center.x, center.y, plant.thePlantRow, bullet, BulletMoveWay.Free);
            bul2.transform.Rotate(0, 0, -90);
            bul2.Damage = plant.attackDamage;
            var bul3 = CreateBullet.Instance.SetBullet(center.x, center.y, plant.thePlantRow, bullet, BulletMoveWay.Free);
            bul3.transform.Rotate(0, 0, -180);
            bul3.Damage = plant.attackDamage;
            var bul4 = CreateBullet.Instance.SetBullet(center.x, center.y, plant.thePlantRow, bullet, BulletMoveWay.Free);
            bul4.transform.Rotate(0, 0, 30);
            bul4.Damage = plant.attackDamage;
            var bul5 = CreateBullet.Instance.SetBullet(center.x, center.y, plant.thePlantRow, bullet, BulletMoveWay.Free);
            bul5.transform.Rotate(0, 0, -30);
            bul5.Damage = plant.attackDamage;

            shots++;
            if (Lawnf.TravelAdvanced(SuperFireGloom.buff2) && !frenzyMode)
            {
                frenzyPoints++;
            }
            if (shots%12==0)
            {
                ParticleManager.Instance.SetParticle(ParticleType.BombCloud_vision_doom, center);
                foreach (var zombie in Board.Instance.zombieArray)
                {
                    if (zombie != null)
                    {
                        var distance = (zombie.transform.position - center).magnitude;
                        var damage = (int)(900 - (distance * 50));
                        if (Lawnf.TravelAdvanced(SuperFireGloom.buff1))
                        {
                            damage = (int)(1800 - (distance * 12));
                        }
                        zombie.TakeDamage(DmgType.NormalAll, damage);
                        zombie.AddEmberScore();
                    }
                }
            }
            if (frenzyPoints>=36)
            {
                ParticleManager.Instance.SetParticle(ParticleType.LightningArcExplosion, center);
                frenzyPoints = 0;
                MelonCoroutines.Start(Frenzy());
            }
        }

        public static BulletType bullet = BulletType.Bullet_seaStar;
    }
}
