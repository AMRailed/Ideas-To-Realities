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
    internal class FireClawZombie : MonoBehaviour
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
            Plugin.printString(this.animator.GetBool("isAttacking").ToString());
            this.zombie.theFirstArmor = this.transform.FindChild("Zombie_head").GetChild(0).gameObject;
            this.zombie.butterHead = this.zombie.theFirstArmor;
        }
        public void Awake()
        {
            this.zombie.theFirstArmorType = Zombie.FirstArmorType.TallNut;
        }
    }
}
