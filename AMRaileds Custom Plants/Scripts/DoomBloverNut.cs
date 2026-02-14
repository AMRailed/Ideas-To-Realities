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
    public class DoomBloverNut : MonoBehaviour
    {
        public NutBlover plant
        {
            get
            {
                return base.gameObject.GetComponent<NutBlover>();
            }
        }

        public float BlowInterval = 4.5f;
        public float BlowCountdown = 0f;
        public void Awake()
        {
            var pTag = new Plant.PlantTag();
            pTag.nutPlant = true;
            pTag.flyingPlant = true;
            plant.plantTag = pTag;
        }
        public void Update()
        {
            BlowCountdown -= Time.deltaTime;
            if (BlowCountdown <= 0f)
            {
                BlowCountdown = BlowInterval;
                int embered = 0;
                foreach (Zombie zombie in Board.Instance.zombieArray)
                {
                    if (zombie != null)
                    {
                        var distance = (zombie.transform.position + Vector3.up * 1.5f - this.transform.position).magnitude;
                        if (distance <= 1.9f)
                        {
                            if (zombie.isEmbered) embered++;
                            zombie.AddEmberScore();
                            zombie.TakeDamage(DmgType.NormalAll, 15 * zombie.GetEmberScore());
                            zombie.KnockBack(math.clamp(0.2f * zombie.GetEmberScore(), 0.2f, 2), Zombie.KnockBackReason.ByIronPea);
                            ParticleManager.Instance.SetParticle(ParticleType.DoomSplat, zombie.transform.position+Vector3.up*1.5f);
                        }
                    }
                }
                foreach (var otherPlant in Lawnf.GetAllPlants())
                {
                    if (otherPlant != null && otherPlant.thePlantColumn == plant.thePlantColumn && otherPlant.thePlantRow == plant.thePlantRow)
                    {
                        otherPlant.Recover(200 + 25 * embered);
                    }
                }
            }
        }
    }
}
