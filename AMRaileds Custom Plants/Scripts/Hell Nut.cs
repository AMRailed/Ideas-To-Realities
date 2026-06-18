using Il2Cpp;
using Il2CppSystem;
using MelonLoader;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class Hellnut : MonoBehaviour
    {
        public Plant plant { get { return base.gameObject.GetComponent<Plant>(); } }
        public float attackInterval = 0.35f;
        public float attackCooldown = 0;

        public void Start()
        {

        }

        public void Update()
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                attackCooldown = attackInterval;
                var pos = this.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 1f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zomb))
                    {
                        zomb.TakeDamage(DmgType.NormalAll, (new Il2CppSystem.Random()).Next(2, 4));
                        if ((new Il2CppSystem.Random()).Next(1, 701) == 1)
                        {
                            UnityEngine.Object.Destroy(zomb.gameObject);
                        }
                    }
                }
            }
        }
    }
}
