using Il2Cpp;
using Il2CppSystem;
using MelonLoader;
using Unity.Mathematics;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class ShrinePot : MonoBehaviour
    {
        public Plant plant { get { return base.gameObject.GetComponent<Plant>(); } }
        public float attackInterval = 0.5f;
        public float attackCooldown = 0;
        public float eatInterval = 1f;
        public float eatCooldown = 1;
        public int auraDamage = 100;
        public int damageAdder = 0;

        public void Update()
        {
            attackCooldown -= Time.deltaTime;
            eatCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                float d = damageAdder * 0.9f;
                damageAdder = (int)math.floor(d);

                attackCooldown = attackInterval;
                var pos = this.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 5);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zomb) && zomb.theZombieRow == this.plant.thePlantRow)
                    {
                        ParticleManager.Instance.SetParticle((ParticleType)250, zomb.transform.Find("Shadow").position);
                        zomb.TakeDamage(DmgType.NormalAll, this.auraDamage + this.damageAdder);
                    }
                }
            }
            if (eatCooldown <= 0 && this.auraDamage < 500)
            {
                eatCooldown = eatInterval;
                foreach (var p in Lawnf.Get1x1Plants(this.plant.thePlantColumn, this.plant.thePlantRow))
                {
                    if (p is not null && Shrine2.UnsacrificablePlants.Contains(p.thePlantType))
                    {
                        p.TakeDamage(100);
                        this.auraDamage = System.Math.Min(500, this.auraDamage + 20);
                    }
                }
            }

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
                                var health = p.thePlantHealth;
                                var point = (int)(health / 50);
                                p.Die();
                                foreach (Zombie z in Board.Instance.zombieArray)
                                {
                                    z.TakeDamage(DmgType.NormalAll, point * 15);
                                }
                                this.damageAdder += point * 25;
                            }
                        }
                    }
                }
            }
        }
    }
}
