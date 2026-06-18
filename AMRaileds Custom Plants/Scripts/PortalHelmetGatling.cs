using HarmonyLib;
using IdeasCustom.Scripts;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppSystem;
using MelonLoader;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Il2CppSystem.Collections.Hashtable;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class PortalHelmetGatling : MonoBehaviour
    {
        public GatlingPea plant { get { return base.gameObject.GetComponent<GatlingPea>(); } }

        public int shots = -3;
        public int riftBombQueue = 0;
        public uint energyPoints = 0;
        public float lastPulse = 0;
        public float lastHeal = 0;
        public bool overcharged = false;
        public bool reserveOvercharge = false;

        public static void ChangeSprite(Bullet bullet)
        {
            if (bullet.theBulletType == BulletType.Bullet_helmetPea)
            {
                SpriteRenderer comp;
                if (bullet.TryGetComponent<SpriteRenderer>(out comp))
                {
                    comp.sprite = Plugin.DarkHelmetBullet;
                    comp.color = Color.white;
                }
            }
        }

        public void Start()
        {
            if (Lawnf.TravelAdvanced(buff1))
            {
                reserveOvercharge = true;
                energyPoints = 8;
            }
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
                        if ((this.energyPoints >= 64 || this.reserveOvercharge) && !this.overcharged)
                        {
                            MelonCoroutines.Start(this.Overcharge(Lawnf.TravelAdvanced(buff2) ? 12 : 6));
                            if (this.reserveOvercharge)
                            {
                                this.reserveOvercharge = false;
                            }
                            else
                            {
                                this.energyPoints = 0;
                            }
                        }
                    }
                }
            }

            if ((this.energyPoints >= 64 || this.reserveOvercharge) && !this.overcharged)
            {
                this.lastPulse += Time.deltaTime;
                if (this.lastPulse > 0.5f)
                {
                    this.lastPulse = 0;
                    ParticleManager.Instance.SetParticle(ParticleType.LightningArcExplosion, plant.transform.position + new Vector3(0, 1));
                }
            }

            if (overcharged && Lawnf.TravelAdvanced(buff1))
            {
                this.lastHeal += Time.deltaTime;
                if (this.lastHeal > .5f)
                {
                    this.lastHeal = 0;
                    ParticleManager.Instance.SetParticle(ParticleType.Health, plant.transform.position + new Vector3(0, 1));
                    foreach (var plant in Lawnf.Get3x3Plants(this.plant.thePlantColumn, this.plant.thePlantRow))
                    {
                        if (plant && plant != null && !plant.IsDestroyed())
                        {
                            plant.Recover(750f * .5f, default, false);
                        }
                    }
                }
            }
        }

        [HideFromIl2Cpp]
        public System.Collections.IEnumerator Overcharge(float time)
        {
            overcharged = true;
            this.plant.thePlantAttackInterval = .2f;
            yield return new WaitForSeconds(time);
            this.plant.thePlantAttackInterval = 1;
            overcharged = false;
        }

        public void AnimShoot()
        {
            shots += 1;
            var bulType = shots >= 1 ? BulletType.Bullet_helmetPea : BulletType.Bullet_ironPea;
            Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
            Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, bulType, 0);

            if (!overcharged) { energyPoints += 1; }
            riftBombQueue += 1;

            if (riftBombQueue >= 28)
            {
                riftBombQueue = 0;
                Bullet riftBomb = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, BulletType.Bullet_butter_portal, 0);
                riftBomb.Damage = 8713;
                riftBomb.normalSpeed = 6;
            }
            bullet.Damage = 8715;
            ChangeSprite(bullet);
            if (shots >= 1)
            {
                shots = -3;
                if (overcharged && Lawnf.TravelAdvanced((AdvBuff)buff2))
                {
                    bullet.Damage = 8715;
                    Bullet bullet2 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, bulType, 2);
                    bullet2.Damage = 8715;
                    bullet2.transform.Rotate(0, 0, -10);
                    bullet2.from = this.plant;
                    ChangeSprite(bullet2);
                    Bullet bullet3 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, bulType, 2);
                    bullet3.Damage = 8715;
                    bullet3.transform.Rotate(0, 0, 10);
                    bullet3.from = this.plant;
                    ChangeSprite(bullet3);
                    Bullet bullet4 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, bulType, 2);
                    bullet4.Damage = 8715;
                    bullet4.transform.Rotate(0, 0, -20);
                    bullet4.from = this.plant;
                    ChangeSprite(bullet4);
                    Bullet bullet5 = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y, this.plant.thePlantRow, bulType, 2);
                    bullet5.Damage = 8715;
                    bullet5.transform.Rotate(0, 0, 20);
                    bullet5.from = this.plant;
                    ChangeSprite(bullet5);
                }
                else
                {
                    bullet.Damage = 8713;
                }
            }
            bullet.theBulletRow = this.plant.thePlantRow;
            bullet.from = this.plant;
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }


    [HarmonyPatch(typeof(GatlingPea))]
    public static class ProtalGatlingPeaPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Shoot1")]
        public static bool PreShoot1(GatlingPea __instance)
        {
            if (__instance.thePlantType == (PlantType)840)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(Bullet_ironPea))]
    public static class IronPeaPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_ironPea __instance, Zombie zombie)
        {
            if (__instance.Damage == 8713)
            {
                __instance.Damage = 160;

                if ((new Il2CppSystem.Random()).Next(4) != 0)
                {
                    Bullet bullet2 = Board.Instance.GetComponent<CreateBullet>().SetBullet(__instance.transform.position.x + 0.1F, __instance.transform.position.y, __instance.theBulletRow, BulletType.Bullet_ironPea, 2);
                    bullet2.Damage = 8713;
                    bullet2.transform.Rotate(0, 0, ((new Il2CppSystem.Random()).Next(2) - 0.5f) * 2 * 135);
                    bullet2.transform.position += bullet2.transform.right * .8f;
                }
            }
            if (__instance.Damage == 8715)
            {
                __instance.Damage = 160 * 3;

                if ((new Il2CppSystem.Random()).Next(4) != 0)
                {
                    Bullet bullet2 = Board.Instance.GetComponent<CreateBullet>().SetBullet(__instance.transform.position.x + 0.1F, __instance.transform.position.y, __instance.theBulletRow, BulletType.Bullet_ironPea, 2);
                    bullet2.Damage = 8715;
                    bullet2.transform.Rotate(0, 0, ((new Il2CppSystem.Random()).Next(2) - 0.5f) * 2 * 135);
                    bullet2.transform.position += bullet2.transform.right * .8f;
                }
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_helmetPea))]
    public static class HelmetPeaPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_helmetPea __instance, Zombie zombie)
        {
            if (__instance.Damage == 8713 || __instance.Damage == 8715)
            {
                __instance.Damage = 160 * 3;
                if (__instance.Damage == 8715)
                {
                    __instance.Damage = 160 * 5;
                }

                if ((new Il2CppSystem.Random()).Next(4) != 0)
                {
                    Bullet bullet2 = Board.Instance.GetComponent<CreateBullet>().SetBullet(__instance.transform.position.x + 0.1F, __instance.transform.position.y, __instance.theBulletRow, BulletType.Bullet_helmetPea, 2);
                    bullet2.Damage = 8713;
                    bullet2.transform.Rotate(0, 0, ((new Il2CppSystem.Random()).Next(2) - 0.5f) * 2 * 135);
                    bullet2.transform.position += bullet2.transform.right * .8f;
                    PortalHelmetGatling.ChangeSprite(bullet2);
                }
                zombie.SetPortaled(zombie.protaledTotalTimer + 1);

                if (zombie.theFirstArmorType == Zombie.FirstArmorType.TallNutFootball || zombie.theFirstArmorType == Zombie.FirstArmorType.BucketNut)
                {
                    zombie.FirstArmorTakeDamage(2000);
                }
                if (zombie.theFirstArmorType == Zombie.FirstArmorType.FootballHelmet || zombie.theFirstArmorType == Zombie.FirstArmorType.Bucket)
                {
                    zombie.FirstArmorTakeDamage(1000);
                }
                zombie.FirstArmorTakeDamage(1000);
            }
        }
    }
    [HarmonyPatch(typeof(Bullet_butter_portal))]
    public static class RiftBombPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitZombie")]
        public static void PreHitZombie(Bullet_butter_portal __instance, Zombie zombie)
        {
            if (__instance.Damage == 8713)
            {
                __instance.Damage = 601;
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch("HitZombie")]
        public static void PostHitZombie(Bullet_butter_portal __instance, Zombie zombie)
        {
            if (__instance.Damage == 601)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + .4f), 2.8f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zomb))
                    {
                        zomb.ApplyDamage(DmgType.NormalAll, 400);
                        if (TypeMgr.IsLeaderZombie(zomb.theZombieType))
                        {
                            zomb.RealKnockBack(2);
                        }
                        else
                        {
                            zomb.transform.position = new UnityEngine.Vector3(Board.Instance.boardMaxX-1.75f, zomb.transform.position.y, 0);
                        }
                    }
                }
                zombie.SetPortaled(zombie.protaledTotalTimer + 3);
            }
        }
    }
}
