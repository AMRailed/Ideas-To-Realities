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
    internal class ObsidianChomper : MonoBehaviour
    {
        public Chomper plant
        {
            get
            {
                return base.gameObject.GetComponent<Chomper>();
            }
        }
        public Animator animator
        {
            get
            {
                return base.gameObject.GetComponent<Animator>();
            }
        }

    }
}
