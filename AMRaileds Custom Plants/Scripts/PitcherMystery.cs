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
using IdeasCustom;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class PitcherMystery : MonoBehaviour
    {
        public Plant plant { get { return base.gameObject.GetComponent<Plant>(); } }
        public Animator animator { get { return base.gameObject.GetComponent<Animator>(); } }

        public void Start()
        {
            if (GameAPP.theGameStatus == 0)
            {
                animator.SetTrigger("Pop");
            }
        }
        public void pop()
        {
            plant.Die(Plant.DieReason.BySelf);
            ParticleManager.Instance.SetParticle(ParticleType.RandomCloud, plant.transform.position);
            CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, PlantType.Tower_waterCan);
        }
    }
}
