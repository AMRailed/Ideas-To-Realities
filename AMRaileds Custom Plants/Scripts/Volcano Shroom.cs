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
using Il2CppInterop.Runtime.Attributes;
using CustomizeLib.MelonLoader;
using Il2CppInterop.Runtime.Injection;
using Unity.Mathematics;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    public class VolcanoShroom : MonoBehaviour
    {
        public UltimateFume plant
        {
            get
            { 
                return base.gameObject.GetComponent<UltimateFume>();
            }
        }
        public static int buff1 = -1;
        public static int buff2 = -1;
        public VolcanoShroom() : base(ClassInjector.DerivedConstructorPointer<VolcanoShroom>()) => ClassInjector.DerivedConstructorBody(this);

        public VolcanoShroom(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.particle = base.GetComponentInChildren<ParticleSystem>();
            plant.particle.gameObject.SetActive(true);
            plant.emission = plant.particle.emission;
            plant.particle.emission.enabled = false;
            plant.DisableDisMix();
            var tag = plant.plantTag;
            tag.firePlant = true;
            plant.plantTag = tag;
        }

        public void StartShoot()
        {
            plant.particle.Play();
            plant.particle.emission.enabled = true;
        }

        public void EndShoot()
        {
            plant.particle.Play();
            plant.particle.emission.enabled = false;
        }
        public void setBuffedDamage()
        {
            if (Lawnf.TravelAdvanced(VolcanoShroom.buff2))
            {
                plant.attackDamage = 125;
            }
            else
            {
                plant.attackDamage = 50;
            }
        }
        public void AttackZombies()
        {
            plant.zombieList.Clear();
            setBuffedDamage();
            foreach (var z in Board.Instance.zombieArray)
            {
                if (z != null && !z.isMindControlled && !TypeMgr.IsAirZombie(z.theZombieType) && z.theZombieRow == plant.thePlantRow && z.gameObject.transform.position.x > plant.gameObject.transform.position.x)
                {
                    plant.zombieList.Add(z);
                }
            }
            GameAPP.PlaySound(58);
            foreach (var z in plant.zombieList)
            {
                if (z != null && !z.isMindControlled)
                {
                    z.TakeDamage(DmgType.NormalAll, plant.attackDamage);
                    z.KnockBack(math.clamp(0.01f * (z.GetEmberScore()-5), 0.01f, 0.03f), Zombie.KnockBackReason.ByIronPea);
                    z.AddEmberScore();
                    z.SetJalaed();
                    if ((new System.Random()).Next(1, 10) == 1 && z.GetTotalHP() / z.GetTotalMaxHealth() <= 0.5)
                    {
                        z.DestoryZombie();
                        Board.Instance.CreateFireLine(z.theZombieRow);
                        CreateZombie.Instance.SetZombieWithMindControl(z.theZombieRow, (ZombieType)254, z.transform.position.x);
                    }
                }
            }
        }
        public void SuperAttackZombies()
        {
            plant.zombieList.Clear();
            setBuffedDamage();
            foreach (var z in Board.Instance.zombieArray)
            {
                if (z != null && !z.isMindControlled && !TypeMgr.IsAirZombie(z.theZombieType) && z.theZombieRow == plant.thePlantRow && z.gameObject.transform.position.x > plant.gameObject.transform.position.x)
                {
                    plant.zombieList.Add(z);
                }
            }
            GameAPP.PlaySound(58);
            foreach (var z in plant.zombieList)
            {
                if (z != null && !z.isMindControlled)
                {
                    if (Lawnf.TravelAdvanced(VolcanoShroom.buff1) && z.isJalaed)
                    {
                        z.JalaedExplode(true, (int)(plant.attackDamage/5));
                    }
                }
            }
        }
    }
}
