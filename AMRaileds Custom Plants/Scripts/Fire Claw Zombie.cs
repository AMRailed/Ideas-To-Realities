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
    public class FireClawZombie : MonoBehaviour
    {
        public Zombie zombie
        {
            get
            {
                return base.gameObject.GetComponent<Zombie>();
            }
        }
        public Animator animator
        {
            get
            {
                return base.gameObject.GetComponent<Animator>();
            }
        }

        public void Start()
        {
            this.zombie.theFirstArmor = this.transform.FindChild("Zombie_head").GetChild(0).gameObject;
            this.zombie.butterHead = this.zombie.theFirstArmor;
            this.zombie.theFirstArmorType = Zombie.FirstArmorType.TallNut;
            this.zombie.revived = true;
        }
    }
}
