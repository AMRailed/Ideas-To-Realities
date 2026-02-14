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
    internal class HypnoPaperZombie : MonoBehaviour
    {
        public ElitePaperZombie zombie
        {
            get
            {
                return base.gameObject.GetComponent<ElitePaperZombie>();
            }
        }
        public float summonTime = 0.75f;
        public List<ZombieType> spawnPool = new List<ZombieType>
        {
            ZombieType.GatlingBlackFootball,
            ZombieType.BlackFootball,
            ZombieType.CherryPaperZ95,
            ZombieType.SuperKirov,
            ZombieType.JacksonDriver,
        };

        public void Start()
        {
            this.zombie.theSecondArmor = this.transform.Find("Zombie_paper_paper1").gameObject;
            this.zombie.theSecondArmorType = Zombie.SecondArmorType.Paper;
        }

        public void Update()
        {
            if (GameAPP.theGameStatus is 0 && this.zombie.theSecondArmorHealth <= 0 && this.zombie.freezeTimer <= 0)
            {
                this.summonTime -= Time.deltaTime;
                if (this.summonTime <= 0)
                {
                    this.summonTime = 0.75f;
                    ZombieType randomSpawn = spawnPool[(new System.Random()).Next(spawnPool.Count)];
                    if (this.zombie.isMindControlled)
                    {
                        CreateZombie.Instance.SetZombieWithMindControl(this.zombie.theZombieRow, randomSpawn, this.transform.position.x);
                    }else
                    {
                        CreateZombie.Instance.SetZombie(this.zombie.theZombieRow, randomSpawn, this.transform.position.x);
                    }
                }
            }
        }
    }
}
