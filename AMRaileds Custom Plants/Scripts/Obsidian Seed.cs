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
using UnityEngine.PlayerLoop;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class ObsidianSeed : MonoBehaviour
    {
        public WallNut plant
        {
            get
            {
                return base.gameObject.GetComponent<WallNut>();
            }
        }
        public float growTime = 90f;
        public void Start()
        {
            if (Lawnf.TravelAdvanced(Hybrid_Melon.buff2))
            {
                growTime = 30f;
            }
        }
        public void Update()
        {
            this.growTime -= Time.deltaTime;
            if (this.growTime <= 0)
            {
                if (Lawnf.TravelAdvanced(Hybrid_Melon.buff2))
                {
                    Board.Instance.CreateFireLine(this.plant.thePlantRow);
                    Board.Instance.CreateFreeze(this.plant.transform.position);
                }
                this.plant.Die();
                CreatePlant.Instance.SetPlant(this.plant.thePlantColumn, this.plant.thePlantRow, (PlantType)260);
            }
        }
    }
}
