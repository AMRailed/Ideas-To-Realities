using Il2Cpp;
using Il2CppSystem;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace IdeasCustom
{
    [RegisterTypeInIl2Cpp]
    public class PickledPepper : MonoBehaviour
    {
        public Plant plant { get { return base.gameObject.GetComponent<Plant>(); } }

        public void Start()
        {

        }

        public void Update()
        {

        }

        public void AnimExplode()
        {

            for (int i = 0; i < Board.Instance.rowNum; i++)
            {
                ParticleManager.Instance.SetParticle(ParticleType.Fire, new(this.plant.thePlantColumn, i));
            }
            GameAPP.PlaySound(6);

            plant.Die(Plant.DieReason.BySelf);
        }
    }

    [HarmonyPatch(typeof(Jalapeno))]
    public static class JalapenoPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AnimExplode")]
        public static bool PreAnimExplode(Jalapeno __instance)
        {
            return __instance.thePlantType != (PlantType)843;
        }
    }
}
