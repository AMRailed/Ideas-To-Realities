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

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class Hybrid_Melon : MonoBehaviour
    {
        public WinterMelon plant
        {
            get
            {
                return base.gameObject.GetComponent<WinterMelon>();
            }
        }
        
        public void AnimShoot()
        {
            if (Lawnf.TravelAdvanced(Hybrid_Melon.buff1))
            {
                plant.thePlantAttackCountDown = 0.5f;
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }
}
