using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace AMRaileds_Custom_Plants
{
    [RegisterTypeInIl2Cpp]
    internal class SuperFireCattail : MonoBehaviour
    {
        public FireCattail plant
        {
            get
            {
                return base.gameObject.GetComponent<FireCattail>();
            }
        }

        public void AnimShoot()
        {

        }

        public static int buff1 = -1;
        public static int buff2 = -1;
    }
}
