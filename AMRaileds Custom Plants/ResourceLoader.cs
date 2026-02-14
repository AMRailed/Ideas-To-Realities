using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AMRaileds_Custom_Plants.Scripts
{
    public class ResourceLoader
    {
        public static AssetBundle LoadBundleFromEmbedded(string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(buffer);
            AssetBundle bundle = request.assetBundle;

            if (bundle != null)
            {
                return bundle;
            }

            return null;
        }

        public static GameObject GetResourceFromBundle(AssetBundle bundle, string name)
        {
            foreach (UnityEngine.Object obj in bundle.LoadAllAssets())
            {
                GameObject testObj = obj.TryCast<GameObject>();
                if (testObj != null)
                {
                    if (testObj.name == name)
                    {
                        return testObj;
                    }
                }
            }
            return null;
        }
    }
}
