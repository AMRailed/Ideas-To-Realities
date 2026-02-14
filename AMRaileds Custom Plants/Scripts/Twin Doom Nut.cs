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
using Unity.Mathematics;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    public class TwinDoomNut : MonoBehaviour
    {
        public DoomNut plant
        {
            get
            {
                return base.gameObject.GetComponent<DoomNut>();
            }
        }

        public float HealInterval = 3f;
        public float HealCountdown = 0f;
        public void Awake()
        {
            var pTag = new Plant.PlantTag();
            pTag.nutPlant = true;
            pTag.flyingPlant = true;
            plant.plantTag = pTag;
        }
        public void Update()
        {
            HealCountdown -= Time.deltaTime;
            if (HealCountdown <= 0f)
            {
                HealCountdown = HealInterval;
                int embered = 0;
                foreach (Zombie zombie in Board.Instance.zombieArray)
                {
                    if (zombie != null)
                    {
                        var distance = (zombie.transform.position + Vector3.up * 1.5f - this.transform.position).magnitude;
                        if (distance <= 2.5f)
                        {
                            if (zombie.isEmbered) embered++;
                            zombie.TakeDamage(DmgType.NormalAll, 30 * zombie.GetEmberScore());
                            zombie.KnockBack(math.clamp(0.1f * zombie.GetEmberScore(), 0.1f, 1), Zombie.KnockBackReason.ByIronPea);
                            zombie.AddEmberScore();
                            ParticleManager.Instance.SetParticle(ParticleType.DoomSplat, zombie.transform.position+Vector3.up*1.5f);
                        }
                    }
                }
                plant.Recover(400 + (embered * 20));
            }
        }
    }
}
