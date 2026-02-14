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
using static MelonLoader.MelonLogger;
using JetBrains.Annotations;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    public class FireFlower : MonoBehaviour
    {
        public Producer plant
        {
            get
            {
                return base.gameObject.GetComponent<Producer>();
            }
        }
        public void Awake()
        {
            plant.attributeCount = 0;
        }
        public void Update()
        {
            var cPlant = base.GetComponent<Producer>();
            var pos = plant.transform.position;
            var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + .8f), 1f);
            foreach (var b in array)
            {
                if (b is not null && b.gameObject.TryGetComponent<Bullet_smallSun>(out var sun) && sun.theBulletRow == plant.thePlantRow)
                {
                    sun.Die();
                    cPlant.attributeCount++;
                    if (cPlant.attributeCount >= 20)
                    {
                        Lawnf.SetDroppedCard(this.transform.position, PlantType.Jalapeno);
                        cPlant.attributeCount = 0;
                        /*GameObject obj = null;
                        for (int i = 0; i < 9; i++)
                        {
                            obj = CreatePlant.Instance.SetPlant(plant.thePlantColumn + i, plant.thePlantRow, PlantType.Jalapeno);
                            if (obj is not null)
                            {
                                break;
                            }
                        }
                        cPlant.attributeCount = 0;*/
                    }
                }
            }
        }
    }
}
