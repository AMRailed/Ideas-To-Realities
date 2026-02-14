using CustomizeLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class UltimateHypnoMagnet : MonoBehaviour
    {
        public Shooter plant
        {
            get
            {
                return base.gameObject.GetComponent<Shooter>();
            }
        }
    }
}
