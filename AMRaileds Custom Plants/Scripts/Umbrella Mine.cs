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
using IdeasCustom;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class UmbrellaMine : MonoBehaviour
    {
        public CabbageUmbrella plant
        {
            get
            {
                return base.gameObject.GetComponent<CabbageUmbrella>();
            }
        }

        public Animator animator
        {
            get
            {
                return base.gameObject.GetComponent<Animator>();
            }
        }
        public float riseTime = 25f;
        public bool active = false;

        public void Awake()
        {
            var tag = this.plant.plantTag;
            tag.potatoPlant = true;
            this.plant.plantTag = tag;
        }

        public void Update()
        {
            this.riseTime -= Time.deltaTime;
            if (this.riseTime <= 0)
            {
                if (!active) ParticleManager.Instance.SetParticle(ParticleType.PotatoRise, new UnityEngine.Vector2(plant.transform.position.x, plant.transform.position.y));
                this.animator.SetTrigger("armed");
                this.animator.ResetTrigger("mashed");
            }
        }
        public void AnimRiseOver()
        {
            this.active = true;
        }
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (this.active)
            {
                Zombie zombie = other.gameObject.GetComponent<Zombie>();
                if (zombie != null)
                {
                    this.active = false;
                    zombie.TakeDamage(DmgType.Explode, 1800);
                    CreateParticle.SetParticle(15, base.transform.position, this.plant.thePlantRow);
                    var pos = plant.transform.position;
                    var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + .8f), 1f);
                    foreach (var z in array)
                    {
                        if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var otzombie) && otzombie.theZombieRow == this.plant.thePlantRow)
                        {
                            otzombie.TakeDamage(DmgType.Explode, 1800);
                            otzombie.KnockBack(3f + (this.plant.UmbrellaPot != null ? 1.5f : 0f), Zombie.KnockBackReason.ByUmbrella);
                        }
                    }
                    ParticleManager.Instance.SetParticle(ParticleType.PotatoSplat, new Vector2(plant.transform.position.x, plant.transform.position.y));
                    this.animator.SetTrigger("mashed");
                    this.animator.ResetTrigger("armed");
                    this.riseTime = 25f;
                }
            }
        }

        [HideFromIl2Cpp]
        public static bool SBlockEffect(CabbageUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType == (PlantType)819 && !zombie.isMindControlled)
            {
                zombie.TakeDamage(DmgType.NormalAll, 120);
                zombie.KnockBack(1.5f * (__instance.UmbrellaPot != null ? 2 : 1));
                return false;
            }
            return true;
        }
    }
}
