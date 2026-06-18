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

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class LaserIceUmbrella : MonoBehaviour
    {
        public LaserUmbrella plant
        {
            get
            {
                return base.gameObject.GetComponent<LaserUmbrella>();
            }
        }

        public void Awake()
        {
            this.transform.Find("sheild").gameObject.AddComponent<LanternUmbrellaEffect>().plant = this.plant;
        }
        
        public void Start()
        {
            //this.plant.ballPrefab = Plugin.LaserIceUmbrellalightBall;
            //this.plant.theLightPrefab = Plugin.LaserIceUmbrellatheLight;
        }
    }
}
