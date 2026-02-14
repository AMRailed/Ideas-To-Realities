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
    internal class DecaySniper : MonoBehaviour
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
                zombie.TakeDamage(DmgType.NormalAll, plant.attackDamage+(80*zombie.poisonLevel));
                System.Random rnd = new System.Random();
                zombie.SetPoison(rnd.Next(4, 6));
                zombie.Garliced();
            }
        }
    }
}
