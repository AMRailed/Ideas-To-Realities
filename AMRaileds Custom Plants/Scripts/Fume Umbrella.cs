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

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class FumeUmbrella : MonoBehaviour
    {
        public CabbageUmbrella plant
        {
            get
            {
                return base.gameObject.GetComponent<CabbageUmbrella>();
            }
        }
    }
}
