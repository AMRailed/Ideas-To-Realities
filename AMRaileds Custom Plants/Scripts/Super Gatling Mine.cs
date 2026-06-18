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
using System.Runtime.CompilerServices;
using System.Numerics;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class SuperGatlingMine : MonoBehaviour
    {
        public SuperSnowGatling plant
        {
            get
            {
                return base.gameObject.GetComponent<SuperSnowGatling>();
            }
        }
        public Animator animator
        {
            get
            {
                return base.gameObject.GetComponent<Animator>();
            }
        }
        public float riseTime = 5f;
        public bool active = false;
        public void Awake()
        {
            this.plant.shoot = base.transform.Find("GatlingPea_head").Find("Shoot").transform;
        }

        public float getDistClosestZombie()
        {
            float bestDistance = int.MaxValue;
            foreach (Zombie zombie in Board.Instance.zombieArray)
            {
                if (zombie != null && zombie.theZombieRow == this.plant.thePlantRow)
                {
                    if (UnityEngine.Vector3.Distance(base.transform.position, zombie.transform.position) < bestDistance)
                    {
                        bestDistance = UnityEngine.Vector3.Distance(base.transform.position, zombie.transform.position);
                    }
                }
            }

            return bestDistance;
        }

        public void Update()
        {
            this.riseTime -= Time.deltaTime;
            if (this.riseTime <= 0)
            {
                if (!active) ParticleManager.Instance.SetParticle(ParticleType.PotatoRise, new UnityEngine.Vector2(plant.transform.position.x, plant.transform.position.y));
                this.animator.SetTrigger("rise");
                this.animator.ResetTrigger("mashed");
            }
        }
        public void AnimShoot()
        {
            float dist = getDistClosestZombie();
            this.plant.thePlantAttackInterval = 1.5f - (1.4f - Math.Clamp(dist / 10, 0f, 1f)*1.4f);
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
                        }
                    }
                    ParticleManager.Instance.SetParticle(ParticleType.PotatoSplat, new UnityEngine.Vector2(plant.transform.position.x, plant.transform.position.y));
                    this.animator.SetTrigger("mashed");
                    this.animator.ResetTrigger("rise");
                    this.riseTime = 5f;
                }
            }
        }
    }
}
