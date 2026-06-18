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

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    internal class BladeStar : MonoBehaviour
    {
        public SwordStarfruit plant
        {
            get
            {
                return base.gameObject.GetComponent<SwordStarfruit>();
            }
        }

        public void SetBullet(Transform transform, int theMovingWay)
        {
            CreateBullet.Instance.SetBullet(transform.position.x, transform.position.y, 0, BulletType.Bullet_shulkLeaf, (BulletMoveWay)theMovingWay);
        }
    }
}
