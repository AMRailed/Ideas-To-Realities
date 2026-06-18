using CustomizeLib;
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections;
using Il2CppSystem.IO;
using Il2CppSystem.Linq.Expressions.Interpreter;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class Shrine2 : MonoBehaviour
    {

        public static PlantType[] UnsacrificablePlants = {
            (PlantType)845,
            (PlantType)820,
        };

        public Shooter plant { get { return base.gameObject.GetComponent<Shooter>(); } }
        public Animator animator { get { return base.gameObject.GetComponent<Animator>(); } }
        public ParticleSystem AcceptParticle { get { return base.transform.Find("Accept").GetComponent<ParticleSystem>(); } }
        public ParticleSystem DeclineParticle { get { return base.transform.Find("Decline").GetComponent<ParticleSystem>(); } }
        public ParticleSystem domainFloor { get { return base.transform.Find("DomainFloor").GetComponent<ParticleSystem>(); } }
        public ParticleSystem slashes { get { return base.transform.Find("Slashes").GetComponent<ParticleSystem>(); } }
        public ParticleSystem scream { get { return base.transform.Find("Scream").GetComponent<ParticleSystem>(); } }

        public GameObject Options
        {
            get
            {
                return base.transform.Find("Options").gameObject;
            }
        }
        public float auraDamageCD = 0.5f;
        public int auraDamage = 200;
        public int damageAdder = 0;

        public float domainDamageCD = 0.1f;
        public int domainAmplifier = 0;
        public float domainLength = 0f;

        public float screamLength = 0f;
        public float screamCountdown = 0.1f;

        public void Start()
        {
            domainFloor.gameObject.SetActive(true);
            slashes.gameObject.SetActive(true);
            scream.gameObject.SetActive(true);
            Options.gameObject.SetActive(false);

            domainFloor.emission.enabled = false;
            slashes.emission.enabled = false;
            scream.emission.enabled = false;
        }

        public int Sacrifice()
        {
            int Score = 0;
            foreach (var plt in Lawnf.Get3x3Plants(plant.thePlantColumn, plant.thePlantRow))
            {
                if (plt != null && Shrine2.UnsacrificablePlants.Contains(plt.thePlantType))
                {
                    Score += (int)plt.thePlantHealth / 320;
                    if (Score > 320) break;
                    plt.Die(Plant.DieReason.BySelf);
                }
            }
            return System.Math.Min(200, Score);
        }

        public void Update()
        {

            if (Input.GetMouseButtonDown(0) && this.gameObject != null)
            {
                Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(MousePos, Vector2.zero);
                if (hit.collider is not null && hit.collider.gameObject is not null)
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        foreach (var p in Lawnf.Get1x1Plants(this.plant.thePlantColumn, this.plant.thePlantRow))
                        {
                            if (p is not null && Shrine2.UnsacrificablePlants.Contains(p.thePlantType))
                            {
                                var score = this.Sacrifice();
                                foreach (var otPlant in Lawnf.GetAllPlants())
                                {
                                    if (otPlant != null)
                                    {
                                        otPlant.thePlantHealth += score * 15;
                                    }
                                }
                                foreach (var otZombie in Board.Instance.zombieArray)
                                {
                                    if (otZombie != null)
                                    {
                                        otZombie.TakeDamage(DmgType.NormalAll, score * 5, (PlantType)820);
                                    }
                                }

                                auraDamage = System.Math.Min(900, auraDamage + (score * 2));
                                damageAdder += score * 10;
                            }
                        }
                    }
                }
            }

            auraDamageCD -= Time.deltaTime;
            if (auraDamageCD <= 0)
            {
                auraDamageCD = 0.5f;
                var pos = plant.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y+0.8f), 1.3f);
                Plant[] db = { };
                foreach (var z in array)
                {
                    if (z != null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && (zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow + 1 || zombie.theZombieRow == plant.thePlantRow - 1))
                    {
                        ParticleManager.Instance.SetParticle((ParticleType)250, zombie.transform.Find("Shadow").position);
                        zombie.TakeDamage(DmgType.NormalAll, this.auraDamage + this.damageAdder);
                    }
                }
                float d = this.damageAdder * 0.9f;
                this.damageAdder = (int)math.floor(d);
            }

            scream.emission.enabled = screamLength > 0;
            screamLength -= Time.deltaTime;
            if (screamLength > 0)
            {
                screamCountdown -= Time.deltaTime;
                if (screamCountdown < 0)
                {
                    screamCountdown = 0.1f;
                    foreach (var zombie in Board.Instance.zombieArray)
                    {
                        zombie.KnockBack(0.4f, Zombie.KnockBackReason.ByIronPea);
                        zombie.RealKnockBack(0.1f);
                        zombie.SetCold(4f, 1);
                        zombie.TakeDamage(DmgType.NormalAll, 20);
                    }
                }
            }

            domainLength -= Time.deltaTime;
            slashes.emission.enabled = domainLength > 0;

            domainDamageCD -= Time.deltaTime;
            if (domainDamageCD <= 0 && domainLength > 0)
            {
                domainDamageCD = 0.1f;
                foreach (var z in Board.Instance.zombieArray)
                {
                    if (z != null)
                    {
                        z.TakeDamage(DmgType.NormalAll, 10*domainAmplifier);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Plant))]
    public static class Shrine2DamagePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("TakeDamage")]
        public static void PostTakeDamage(Plant __instance)
        {
            if (__instance.thePlantType == (PlantType)820)
            {
                var scream = __instance.gameObject.GetComponent<Shrine2>();
                if (scream != null)
                {
                    scream.screamLength = 1f;
                }
            }
        }
    }
}
