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
    internal class Apocastar : MonoBehaviour
    {
        public UltimateStar plant
        {
            get
            {
                return base.gameObject.GetComponent<UltimateStar>();
            }
        }

        public void AnimShoot()
        {
            Board.Instance.CreateUltimateMateorite();
            
            Board.Instance.SetDoom((int)Math.Floor(Board.Instance.boardMaxX - 2), (int)Math.Floor(Board.Instance.boardMaxY / 2), false, true);
        }
    }
}
