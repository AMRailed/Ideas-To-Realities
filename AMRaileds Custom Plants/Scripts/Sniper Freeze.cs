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

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class SniperFreeze : MonoBehaviour
    {
        public SniperPea plant
        {
            get
            {
                return base.gameObject.GetComponent<SniperPea>();
            }
        }

        public void AnimShoot()
        {
            GameObject z = this.plant.SearchZombie();
            if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie))
            {
                zombie.AddfreezeLevel(400);
                zombie.SetFreeze(4);
            }
        }
    }
}
