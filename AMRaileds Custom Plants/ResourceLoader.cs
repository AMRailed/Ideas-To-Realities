using Il2Cpp;
using Il2CppSystem.IO;
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

namespace IdeasCustom.Scripts
{
    public class ResourceLoader
    {
        public static AssetBundle LoadBundleFromEmbedded(string path)
        {
            Plugin.printString("Loading "+path);

            Assembly assembly = Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Dispose();

            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(buffer);
            AssetBundle bundle = request.assetBundle;

            if (bundle != null)
            {
                Plugin.printString("Successfully loaded " + path);
                return bundle;
            }

            return null;
        }
        public static Sprite LoadSpriteFromEmbedded(string path, int pixelPerUnit = 100)
        {
            Plugin.printString("Loading " + path);

            Assembly assembly = Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Dispose();

            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            UnityEngine.ImageConversion.LoadImage(tex, buffer);

            if (tex)
            {
                Plugin.printString("Successfully loaded " + path);
                return Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    pixelPerUnit
                );
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
                        Plugin.printString("Successfully loaded asset from bundle with the name of " + name);
                        return testObj;
                    }
                }
            }
            return null;
        }
    }
}
