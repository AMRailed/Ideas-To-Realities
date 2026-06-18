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
    internal class SunnySniper : MonoBehaviour
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
                zombie.TakeDamage(DmgType.NormalAll, Board.Instance.theSun / 10);
                if (TypeMgr.IsGargantuar(zombie.theZombieType) && !TypeMgr.UltimateZombie(zombie.theZombieType) && !TypeMgr.EliteZombie(zombie.theZombieType))
                {
                    Board.Instance.GetSun(50);
                }
                else if (TypeMgr.EliteZombie(zombie.theZombieType) || TypeMgr.UltimateZombie(zombie.theZombieType))
                {
                    Board.Instance.GetSun((new System.Random()).Next(3, 4)*25);
                }
                else
                {
                    Board.Instance.GetSun(25);
                }
            }
        }
    }
}
