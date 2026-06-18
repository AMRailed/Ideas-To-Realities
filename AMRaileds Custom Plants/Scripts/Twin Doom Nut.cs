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

namespace IdeasCustom
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
            plant.plantTag = pTag;
        }
        public void Update()
        {
            HealCountdown -= Time.deltaTime;
            if (HealCountdown <= 0f)
            {
                HealCountdown = HealInterval;
                int embered = 0;
                var reqDistance = 1.9f;

                if (Lawnf.TravelAdvanced(TwinDoomNut.buff1))
                {
                    reqDistance = 3.6f;
                }

                foreach (Zombie zombie in Board.Instance.zombieArray)
                {
                    if (zombie != null)
                    {
                        var distance = (zombie.transform.position + Vector3.up * 1.5f - this.transform.position).magnitude;
                        if (distance <= reqDistance)
                        {
                            if (zombie.isEmbered) embered++;
                            zombie.TakeDamage(DmgType.NormalAll, 30 * zombie.GetEmberScore());
                            zombie.KnockBack(math.clamp(0.1f * zombie.GetEmberScore(), 0.1f, 1), Zombie.KnockBackReason.ByIronPea);
                            zombie.AddEmberScore();
                            ParticleManager.Instance.SetParticle(ParticleType.DoomSplat, zombie.transform.position+Vector3.up*1.5f);
                        }
                    }
                }
                plant.Recover(200 + (embered * 20));
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }

    [HarmonyPatch(typeof(Plant))]
    public static class TwinDoomNutDamagePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("TakeDamage")]
        public static void PreTakeDamage(Plant __instance, ref int damage)
        {
            if (__instance.thePlantType == (PlantType)838)
            {
                damage = math.min(damage, 50);
            }
        }
    }
}
