using Il2Cpp;
using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime.Attributes;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    public class SuperFireGloom : MonoBehaviour
    {
        public FireGloom plant { get { return base.gameObject.GetComponent<FireGloom>(); } }

        public int shots = 0;
        public int frenzyPoints = 0;
        public bool frenzyMode = false;

        public void Start()
        {
            if (GameAPP.theGameStatus == 0)
            {
                plant.range = 4f;
                plant.center = gameObject.transform.FindChild("Shoot").gameObject;
            }
        }

        [HideFromIl2Cpp]
        public System.Collections.IEnumerator Frenzy()
        {
            frenzyMode = true;
            yield return new WaitForSeconds(8f);
            frenzyMode = false;
        }

        public void AnimShoot()
        {
            if (frenzyMode)
            {
                ParticleManager.Instance.SetParticle(ParticleType.LightningArcExplosion, plant.center.transform.position);
                plant.attackDamage = 240;
            }else { plant.attackDamage = 120; }
                ParticleManager.Instance.SetParticle((ParticleType)118, plant.center.transform.position);
            shots++;
            if (Lawnf.TravelAdvanced(buff2) && !frenzyMode)
            {
                frenzyPoints++;
            }
            if (shots%12==0)
            {
                ParticleManager.Instance.SetParticle(ParticleType.BombCloud_vision_doom, plant.center.transform.position);
                foreach (var zombie in Board.Instance.zombieArray)
                {
                    if (zombie != null)
                    {
                        var distance = (zombie.transform.position - plant.center.transform.position).magnitude;
                        var damage = (int)(900 - (distance * 50));
                        if (Lawnf.TravelAdvanced(buff1))
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
                ParticleManager.Instance.SetParticle(ParticleType.LightningArcExplosion, plant.center.transform.position);
                frenzyPoints = 0;
                MelonCoroutines.Start(Frenzy());
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }
}
