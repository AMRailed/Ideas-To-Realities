using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Attributes;
using Il2CppSystem;
using MelonLoader;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class SaladGatling : MonoBehaviour
    {
        public GatlingPea plant { get { return base.gameObject.GetComponent<GatlingPea>(); } }
        public Animator animator { get { return base.gameObject.GetComponent<Animator>(); } }

        public int Mode = 1; // 1: Normal,  2: Shotgun, 3: Flow, 4: Flower, 5: Spray

        public int SprayCurAngle = 0;
        public bool SprayBounceUp = false;
        public bool Evolved = false;

        public GameObject Indicator { get { return base.transform.Find("Indicator").gameObject; } }
        public GameObject Notch1 { get { return this.Indicator.transform.Find("notch_1").gameObject; } }
        public GameObject Notch2 { get { return this.Indicator.transform.Find("notch_2").gameObject; } }
        public GameObject Notch3 { get { return this.Indicator.transform.Find("notch_3").gameObject; } }
        public GameObject Notch4 { get { return this.Indicator.transform.Find("notch_4").gameObject; } }
        public GameObject Notch5 { get { return this.Indicator.transform.Find("notch_5").gameObject; } }

        public void EnableNotch(GameObject Notch)
        {
            Notch1.transform.Find("filling").gameObject.SetActive(false);
            Notch2.transform.Find("filling").gameObject.SetActive(false);
            Notch3.transform.Find("filling").gameObject.SetActive(false);
            Notch4.transform.Find("filling").gameObject.SetActive(false);
            Notch5.transform.Find("filling").gameObject.SetActive(false);

            Notch.transform.Find("filling").gameObject.SetActive(true);
        }

        public void CycleMode()
        {
            Mode++;
            if (Mode > 5)
            {
                Mode = 1;
            }
            if (Mode > 3 && !Lawnf.TravelAdvanced(buff2))
            {
                Mode = 1;
            }    

            if (Lawnf.TravelAdvanced(buff2))
            {
                this.plant.attackDamage = 320;
            }
            else
            {
                this.plant.attackDamage = 160;
            }

            if (Lawnf.TravelAdvanced(buff1))
            {
                Evolved = true;
            }    

            if (Mode == 1)
            {
                this.animator.SetBool("shoot1", false);
                this.animator.SetBool("shoot2", false);
                plant.thePlantAttackInterval = 1f;
                this.EnableNotch(Notch1);
            }
            else if (Mode == 2)
            {
                this.animator.SetBool("shoot1", true);
                this.animator.SetBool("shoot2", false);
                if (Evolved)
                {
                    this.animator.SetBool("shoot1", false);
                    this.animator.SetBool("shoot2", true);
                }
                plant.thePlantAttackInterval = 1.5f;
                this.EnableNotch(Notch2);
            }
            else if (Mode == 3)
            {
                this.animator.SetBool("shoot1", true);
                this.animator.SetBool("shoot2", false);
                plant.thePlantAttackInterval = 0.25f;
                this.EnableNotch(Notch3);
            }
            else if (Mode == 4)
            {
                this.animator.SetBool("shoot1", true);
                this.animator.SetBool("shoot2", false);
                plant.thePlantAttackInterval = 1f;
                this.EnableNotch(Notch4);
            }
            else if (Mode == 5)
            {
                this.animator.SetBool("shoot1", true);
                this.animator.SetBool("shoot2", false);
                plant.thePlantAttackInterval = 0.25f;
                if (this.Evolved)
                {
                    this.animator.SetBool("shoot1", false);
                    plant.thePlantAttackInterval = 0.05f;
                }
                this.EnableNotch(Notch5);
            }
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0) && this.gameObject != null)
            {
                Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(MousePos, Vector2.zero);
                if (hit.collider is not null && hit.collider.gameObject is not null && hit.collider.gameObject == this.gameObject)
                {
                    if (this.animator.GetBool("shooting")) return;
                    this.CycleMode();
                }
            }
        }

        public void AnimShoot()
        {
            if (this.Mode == 2)
            {
                var end = 5;
                var totalSpread = 35;

                if (this.Evolved)
                {
                    end = 11;
                    totalSpread = 50;
                }

                for (int i = 1; i <= end; i++)
                {
                    Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
                    Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1f, position.y, this.plant.thePlantRow, BulletType.Bullet_superMelon, 2);
                    bullet.theBulletRow = this.plant.thePlantRow;
                    bullet.from = this.plant;
                    bullet.Damage = this.plant.attackDamage;
                    bullet.transform.Rotate(0, 0, (i * (totalSpread / end)) - (totalSpread / 2) );
                }
            }
            else if (this.Mode == 4)
            {
                var end = 8;

                if (this.Evolved)
                {
                    end = 32;
                }

                for (int i = 1; i <= end; i++)
                {
                    Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
                    Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1f, position.y, this.plant.thePlantRow, BulletType.Bullet_superMelon, 2);
                    bullet.theBulletRow = this.plant.thePlantRow;
                    bullet.from = this.plant;
                    bullet.Damage = this.plant.attackDamage;
                    bullet.transform.Rotate(0, 0, i * (360 / end));
                }
            }
            else if (this.Mode == 5)
            {
                if (this.SprayBounceUp)
                {
                    this.SprayCurAngle -= 15;
                    if (this.SprayCurAngle <= -30) this.SprayBounceUp = false;
                }
                else
                {
                    this.SprayCurAngle += 15;
                    if (this.SprayCurAngle >= 30) this.SprayBounceUp = true;
                }
                //Plugin.printString(this.SprayCurAngle.ToString());

                var start = 0;
                var end = 0;

                if (this.Evolved)
                {
                    start = -1;
                    end = 1;
                }

                for (int i = start; i <= end; i++)
                {
                    Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
                    Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1f, position.y, this.plant.thePlantRow, BulletType.Bullet_superMelon, 2);
                    bullet.theBulletRow = this.plant.thePlantRow;
                    bullet.from = this.plant;
                    bullet.Damage = this.plant.attackDamage;
                    bullet.transform.Rotate(0, 0, SprayCurAngle + (i * 15));
                }
            }
            else
            {
                var start = 0;
                var end = 0;

                if (this.Evolved)
                {
                    start = -2;
                    end = 1;
                }

                for (int i = start; i <= end; i++)
                {
                    Vector3 position = base.transform.Find("GatlingPea_head").Find("Shoot").transform.position;
                    Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1f, position.y + (i * 0.3f), this.plant.thePlantRow, BulletType.Bullet_superMelon, 2);
                    bullet.theBulletRow = this.plant.thePlantRow;
                    bullet.from = this.plant;
                    bullet.Damage = this.plant.attackDamage;
                }
            }
        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }


    [HarmonyPatch(typeof(GatlingPea))]
    public static class SaladGatlingPeaPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Shoot1")]
        public static bool PreShoot1(GatlingPea __instance)
        {
            if (__instance.thePlantType == (PlantType)841)
            {
                return false;
            }
            return true;
        }
    }
}
