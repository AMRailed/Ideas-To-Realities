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
using Il2CppSystem.Linq.Expressions.Interpreter;
using Il2CppSystem.IO;
using Il2CppSystem.Collections;
using UnityEngine.UIElements;

namespace IdeasCustom
{
    public enum ShrineType {
        Slice,
        Execute,
        Heal,
        Illuminate,
    };
    [RegisterTypeInIl2Cpp]
    internal class Shrine : MonoBehaviour
    {
        public Shooter plant { get { return base.gameObject.GetComponent<Shooter>(); } }
        public Animator animator { get { return base.gameObject.GetComponent<Animator>(); } }
        public ParticleSystem AcceptParticle { get { return base.transform.Find("Accept").GetComponent<ParticleSystem>(); } }
        public ParticleSystem DeclineParticle { get { return base.transform.Find("Decline").GetComponent<ParticleSystem>(); } }
        public ParticleSystem domainFloor { get { return base.transform.Find("DomainFloor").GetComponent<ParticleSystem>(); } }
        public ParticleSystem slashes { get { return base.transform.Find("Slashes").GetComponent<ParticleSystem>(); } }
        public ParticleSystem scream { get { return base.transform.Find("Scream").GetComponent<ParticleSystem>(); } }

        public float healCD = 5;
        public SpriteRenderer HealSprite
        {
            get
            {
                return base.transform.Find("Options").Find("Heal").GetComponent<SpriteRenderer>();
            }
        }
        public float sliceCD = 5;
        public SpriteRenderer SliceSprite
        {
            get
            {
                return base.transform.Find("Options").Find("Slice").GetComponent<SpriteRenderer>();
            }
        }
        public float executeCD = 5;
        public SpriteRenderer ExecuteSprite
        {
            get
            {
                return base.transform.Find("Options").Find("Execute").GetComponent<SpriteRenderer>();
            }
        }
        public float illuminateCD = 5;
        public SpriteRenderer IlluminateSprite
        {
            get
            {
                return base.transform.Find("Options").Find("Illuminate").GetComponent<SpriteRenderer>();
            }
        }
        public float auraDamageCD = 0.5f;
        public ShrineType theShrineType = ShrineType.Heal;
        public float domainDamageCD = 0.1f;
        public int domainAmplifier = 0;
        public float domainLength = 0f;
        public float screamLength = 0f;
        public float screamCountdown = 0.1f;

        public void Execute(int amplifier)
        {
            for (int i = 0; i < amplifier; i++)
            {
                float strongestHP = 0;
                Zombie strongest = null;
                foreach (var zombie in Board.Instance.zombieArray)
                {
                    if (zombie != null && zombie.GetTotalHP() > strongestHP)
                    {
                        strongestHP = zombie.GetTotalHP();
                        strongest = zombie;
                    }
                }

                if (strongest != null)
                {
                    strongest.TakeDamage(DmgType.NormalAll, int.MaxValue);
                }
            }
        }

        public void Slice(int amplifier)
        {
            if (amplifier >= 48)
            {
                if (domainLength > 0)
                {
                    if (domainAmplifier < amplifier)
                    {
                        domainAmplifier = amplifier;
                    }
                    domainLength += amplifier;
                }else
                {
                    domainLength = amplifier;
                    domainAmplifier = amplifier;
                }
            }
            foreach (var zombie in Board.Instance.zombieArray)
            {
                if (zombie != null)
                {
                    zombie.TakeDamage(DmgType.NormalAll, 540 * amplifier);
                }
            }
        }

        public void Illuminate(int amplifier)
        {
            foreach (var zombie in Board.Instance.zombieArray)
            {
                if (zombie != null)
                {
                    for (int i = 0; i < amplifier; i++)
                    {
                        CreateItem.Instance.SetCoin(Mouse.Instance.GetColumnFromX(zombie.transform.position.x), zombie.theZombieRow, 0, 0);
                    }
                }
            }
        }

        public void Heal(int amplifier)
        {
            foreach (var p in Lawnf.GetAllPlants())
            {
                if (p != null && p.gameObject.TryGetComponent<Plant>(out var plt))
                {
                    plt.Recover(((300*amplifier) + (int)(plt.thePlantMaxHealth * (0.05f*amplifier))));
                }
            }
        }

        public void Start()
        {
            domainFloor.gameObject.SetActive(true);
            slashes.gameObject.SetActive(true);
            scream.gameObject.SetActive(true);

            domainFloor.emission.enabled = false;
            slashes.emission.enabled = false;
            scream.emission.enabled = false;
        }

        public int Sacrifice()
        {
            int Score = 0;
            foreach (var plt in Lawnf.Get3x3Plants(plant.thePlantColumn, plant.thePlantRow))
            {
                if (plt != null)
                {
                    if (Lawnf.IsSuperPlant(plt.thePlantType) || Lawnf.IsUltiPlant(plt.thePlantType) || plt.thePlantType == PlantType.ScaredyPotato)
                    {
                        plt.Die(Plant.DieReason.BySelf);
                        if (Lawnf.IsUltiPlant(plt.thePlantType))
                        {
                            Score += 2;
                        }
                        if (plt.thePlantType == PlantType.ScaredyPotato)
                        {
                            if (domainLength > 0) domainLength = 100000;
                        }
                        Score += 1;
                    }
                }
            }
            return Score;
        }

        public void Update()
        {
            healCD -= Time.deltaTime;
            HealSprite.color = Color.white * (healCD <= 0 ? 1f : .5f);
            HealSprite.gameObject.SetActive(theShrineType == ShrineType.Heal ? true : false);
            executeCD -= Time.deltaTime;
            ExecuteSprite.color = Color.white * (executeCD <= 0 ? 1f : .5f);
            ExecuteSprite.gameObject.SetActive(theShrineType == ShrineType.Execute ? true : false);
            illuminateCD -= Time.deltaTime;
            IlluminateSprite.color = Color.white * (illuminateCD <= 0 ? 1f : .5f);
            IlluminateSprite.gameObject.SetActive(theShrineType == ShrineType.Illuminate ? true : false);
            sliceCD -= Time.deltaTime;
            SliceSprite.color = Color.white * (sliceCD <= 0 ? 1f : .5f);
            SliceSprite.gameObject.SetActive(theShrineType == ShrineType.Slice ? true : false);

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && this.gameObject != null)
            {
                Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(MousePos, Vector2.zero);
                if (hit.collider is not null && hit.collider.gameObject is not null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            theShrineType += 1;
                            if (theShrineType == (ShrineType)4) theShrineType = 0;
                            return;
                        }
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (hit.collider.transform.gameObject != this.gameObject) return;
                        if (executeCD <= 0 && theShrineType == ShrineType.Execute)
                        {
                            int accepted = Sacrifice();
                            if (accepted>0) AcceptParticle.Emit(100); else DeclineParticle.Emit(100);
                            if (accepted<=0) return;
                            executeCD = 5;
                            Execute(accepted);
                        }
                        if (illuminateCD <= 0 && theShrineType == ShrineType.Illuminate)
                        {
                            int accepted = Sacrifice();
                            if (accepted > 0) AcceptParticle.Emit(100); else DeclineParticle.Emit(100);
                            if (accepted <= 0) return;
                            illuminateCD = 5;
                            Illuminate(accepted);
                        }
                        if (healCD <= 0 && theShrineType == ShrineType.Heal)
                        {
                            int accepted = Sacrifice();
                            if (accepted > 0) AcceptParticle.Emit(100); else DeclineParticle.Emit(100);
                            if (accepted <= 0) return;
                            healCD = 5;
                            Heal(accepted);
                        }
                        if (sliceCD <= 0 && theShrineType == ShrineType.Slice)
                        {
                            int accepted = Sacrifice();
                            if (accepted > 0) AcceptParticle.Emit(100); else DeclineParticle.Emit(100);
                            if (accepted <= 0) return;
                            sliceCD = 5;
                            Slice(accepted);
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
                        zombie.TakeDamage(DmgType.NormalAll, 200);
                    }
                }
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
    public static class ShrineDamagePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("TakeDamage")]
        public static void PostTakeDamage(Plant __instance)
        {
            if (__instance.thePlantType == (PlantType)820)
            {
                var scream = __instance.gameObject.GetComponent<Shrine>();
                if (scream != null)
                {
                    scream.screamLength = 1f;
                }
            }
        }
    }
}
