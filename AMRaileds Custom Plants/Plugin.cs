using IdeasCustom.Scripts;
using CustomizeLib;
using CustomizeLib.MelonLoader;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppTMPro;
using JetBrains.Annotations;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using static Il2Cpp.Board;
using static MelonLoader.MelonLogger;
using UnityEngine.UI;
using System.Runtime.Intrinsics.X86;


[assembly: MelonInfo(typeof(IdeasCustom.Plugin), "Ideas to Realities", "1.4.0", "AMRailed and Others")]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]

namespace IdeasCustom
{
    public static class Extensions
    {
        public static float GetTotalHP(this Zombie zombie)
        {
            return zombie.theHealth + zombie.theFirstArmorHealth + zombie.theSecondArmorHealth;
        }
        public static float GetTotalMaxHealth(this Zombie zombie)
        {
            return zombie.theMaxHealth + zombie.theFirstArmorMaxHealth + zombie.theSecondArmorMaxHealth;
        }

        public static int GetEmberScore(this Zombie zombie)
        {
            if (Plugin.emberScores.ContainsKey(zombie))
            {
                return Plugin.emberScores[zombie];
            }
            return 0;
        }
        public static void AddEmberScore(this Zombie zombie, int amount = 1)
        {
            Plugin.ApplyEmberScore(zombie, amount);
        }
    }

    public enum BossRushStage
    {
        Normal,
        Odyssey,
        Ascended
    }

    public class Plugin : MelonMod
    {
        //public static GameObject LaserIceUmbrellalightBall = null;
        //public static GameObject LaserIceUmbrellatheLight = null;
        public static Dictionary<Zombie, int> emberScores = new Dictionary<Zombie, int>();
        public static Plugin instance;

        public static Sprite DarkHelmetBullet;
        public static Sprite BuckportalBullet;
        public static Sprite DoomCabbageBullet;

        public static int SuperFireGloomLevelID = 0;
        public static int PortalHelmetPeaLevelID = 0;
        public static int SaladGatlingLevelID = 0;
        public static int DoomsdayLevelID = 0;
        public static int DoomsdayOdysseyLevelID = 0;
        public static int BossRushLevelID = 0;

        public static List<PlantType> BossRushSet1 = new List<PlantType>()
        {
            (PlantType)805,
            PlantType.Melonpult,
            PlantType.CornCabbage,
            PlantType.WallNut,
            PlantType.LotusBamboo,
            PlantType.SpruceShulk,
            PlantType.WaterAloes,
            PlantType.CherryUmbrella,
            PlantType.ObsidianJalapeno,
        };

        public static List<PlantType> BossRushSet2 = new List<PlantType>()
        {
            (PlantType)806,
            PlantType.SuperMelon,
            PlantType.WallNut,
            PlantType.LotusBamboo,
            PlantType.SuperSpruce,
            PlantType.IronPumpkin,
            PlantType.CherryUmbrella,
            PlantType.ObsidianJalapeno,
            PlantType.TallIceNut,
            PlantType.IceDoom,
        };

        public BossRushStage CurrentBossRushStage = BossRushStage.Normal;
        public bool BossRushHelp = false;

        public static void ApplyEmberScore(Zombie zombie, int amount)
        {
            if (emberScores.ContainsKey(zombie))
            {
                emberScores[zombie] += amount;

                var generalEmberBuff = Lawnf.TravelAdvanced(TwinDoomNut.buff2);
                var threshold = 5;
                if (generalEmberBuff)
                {
                    threshold = 2;
                }

                if (emberScores[zombie] >= threshold && !zombie.isEmbered)
                {
                    zombie.SetEmbered();
                    if (generalEmberBuff)
                    {
                        ParticleManager.Instance.SetParticle(ParticleType.DoomSplat, zombie.transform.position + UnityEngine.Vector3.up * 2f);
                        var pos = zombie.transform.position;
                        var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + 2), 2.5f);
                        foreach (var z in array)
                        {
                            if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zomb))
                            {
                                zomb.TakeDamage(DmgType.NormalAll, 10);
                                zomb.AddEmberScore(2);
                            }
                        }
                    }
                }
            }
            else
            {
                emberScores.Add(zombie, 1);
            }
        }

        public static void summonJack(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateJackboxZombie, plant.transform.position.x);
        }
        public static void summonStriker(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateFootballZombie, plant.transform.position.x);
        }
        public static void summonBowling(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateMachineNutZombie, plant.transform.position.x);
        }
        public static void summonSpider(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateKirovZombie, plant.transform.position.x);
        }
        public static void summonGramps(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimatePaperZombie, plant.transform.position.x);
        }
        public static void summonHypnodancer(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateJacksonDriver, plant.transform.position.x);
        }
        public static void summonTrident(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateFootballDrown, plant.transform.position.x);
        }
        public static void summonAbyssal(Plant plant)
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.UltimateGargantuar, plant.transform.position.x);
        }
        public static void newCard(int pType)
        {
            var template = GameObject.Find("WaterAloes").transform.parent.parent.parent.GetChild(1).GetChild(0).Find("Hamburger");
            var card = UnityEngine.Object.Instantiate(template, template.transform.parent.parent.parent.GetChild(1).GetChild(0));
            card.name = "Tower_waterCan";
            var mkbBg = card.transform.GetChild(0).gameObject;
            Lawnf.ChangeCardSprite((PlantType)pType, mkbBg);
            mkbBg.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = PlantDataLoader.plantData[pType].field_Public_Int32_1.ToString();
            if (Board.Instance is not null)
            {
                var mkb1 = card.transform.GetChild(1).gameObject;
                Lawnf.ChangeCardSprite((PlantType)pType, mkb1);
                mkb1.GetComponent<CardUI>().CD = PlantDataLoader.plantData[pType].field_Public_Single_2;
                mkb1.GetComponent<CardUI>().theSeedCost = PlantDataLoader.plantData[pType].field_Public_Int32_1;
                mkb1.GetComponent<CardUI>().thePlantType = (PlantType)pType;
                mkb1.GetComponent<CardUI>().theSeedType = pType;
                InGameUI.Instance.cards.Add(mkb1.GetComponent<CardUI>());
            }
        }

        public static void printString(string str)
        {
            MelonLogger.Msg(str);
        }

        public static System.Collections.IEnumerator FireOcean(UnityEngine.Vector2 location)
        {
            var ticksParticle = 0;
            for (int i = 0; i < 400; i++)
            {
                ticksParticle++;
                var array = Physics2D.OverlapCircleAll(new(location.x, location.y), 1f);
                var bulletRow = Mouse.Instance.GetRowFromY(location.x, location.y);
                if (ticksParticle % 10 == 0) ParticleManager.Instance.SetParticle(ParticleType.FireOcean, location - new UnityEngine.Vector2(0, 0.4f), bulletRow);
                foreach (var z in array)
                {
                    if (z != null && z.gameObject.TryGetComponent<Zombie>(out var otherZ) && (otherZ.theZombieRow == bulletRow))
                    {
                        otherZ.TakeDamage(DmgType.NormalAll, 30);
                        otherZ.SetJalaed();
                    }
                }
                yield return new WaitForSeconds(0.02f);
            }
        }

        public IEnumerator InitializeBossRush()
        {
            CustomLevelData customLevelData;
            if (!Utils.IsCustomLevel(out customLevelData)) Plugin.printString("Failed to run Boss Rush"); yield return "f";

            Board board = Board.Instance;

            yield return new WaitForSeconds(20f);
            if (!board) yield return "f";

            CurrentBossRushStage = BossRushStage.Normal;

            //Zomboss
            Plugin.printString("Zomboss Stage");
            GameObject zomboss = CreateZombie.Instance.SetZombie(0, ZombieType.ZombieBoss);

            while (zomboss && (zomboss.GetComponent<Zombie>().theHealth) > 0f)
            {
                if (board == null) break;
                board.theWave = 1;
                yield return null;
            }
            ;
            board.theWave = 10;

            yield return new WaitForSeconds(3);

            GameObject.Destroy(zomboss);
            board.CreateFreeze(new(0, 0));

            //Golden Zomboss
            Plugin.printString("Golden Zomboss Stage");
            GameObject zomboss2 = CreateZombie.Instance.SetZombie(0, ZombieType.ZombieBoss2);

            while (zomboss2 && (zomboss2.GetComponent<Zombie>().theHealth) > 0f)
            {
                if (board == null) break;
                board.theWave = 11;
                yield return null;
            }
            ;
            board.theWave = 20;

            yield return new WaitForSeconds(3);

            GameObject.Destroy(zomboss2);
            board.CreateFreeze(new(0, 0));

            //Snow Queen
            Plugin.printString("Snow Queen Stage");
            CurrentBossRushStage = BossRushStage.Odyssey;

            for (int i = 0; i < 5; i++)
            {
                CreatePlant.Instance.SetPlant(0, i, PlantType.IronPumpkin).GetComponent<Plant>();
                CreatePlant.Instance.SetPlant(1, i, PlantType.IronPumpkin).GetComponent<Plant>();
                CreatePlant.Instance.SetPlant(2, i, PlantType.IronPumpkin).GetComponent<Plant>();
                CreatePlant.Instance.SetPlant(3, i, PlantType.IronPumpkin).GetComponent<Plant>();
                CreatePlant.Instance.SetPlant(4, i, PlantType.IronPumpkin).GetComponent<Plant>();
            }

            GameObject snowQueen = CreateZombie.Instance.SetZombie(2, ZombieType.UltimateSnowZombie);
            snowQueen.GetComponent<UltimateSnowZombie>().boss = true;

            yield return new WaitForSeconds(3.2f);
            //Reset Board and Conveyor
            board.CreateFreeze(new(0, 0));
            ConveyManager.Instance.ClearCards();
            board.ClearTheBoard();
            if (BossRushHelp)
            {
                for (int i = 0; i < 5; i++)
                {
                    CreatePlant.Instance.SetPlant(6, i, PlantType.UltimateTallNut);
                    CreatePlant.Instance.SetPlant(7, i, PlantType.UltimateTallNut);

                    CreatePlant.Instance.SetPlant(0, i, PlantType.UltimatePumpkin);
                    CreatePlant.Instance.SetPlant(1, i, PlantType.UltimatePumpkin);
                }
            }

            while (snowQueen && (snowQueen.GetComponent<Zombie>().theHealth) > 0f)
            {
                if (board == null) break;
                board.theWave = 21;
                yield return null;
            }
            ;
            board.theWave = 30;

            yield return new WaitForSeconds(3);

            GameObject.Destroy(snowQueen);
            board.CreateFreeze(new(0, 0));

            //Horse Boss
            Plugin.printString("Horse Boss Stage");

            GameObject horseBoss = CreateZombie.Instance.SetZombie(2, ZombieType.HorseBoss);
            horseBoss.GetComponent<UltimateSnowZombie>().boss = true;

            while (horseBoss && (horseBoss.GetComponent<Zombie>().theHealth) > 0f)
            {
                if (board == null) break;
                board.theWave = 31;
                yield return null;
            }

            yield return new WaitForSeconds(3);

            GameObject.Destroy(horseBoss);
            board.CreateFreeze(new(0, 0));

            board.theWave = 40;
        }

        public override void OnInitializeMelon()
        {
            Plugin.instance = this;

            MelonLogger.Msg("The Ideas to Realities mod has loaded!");
            //AssetBundleCreateRequest firePeashooterBundleRequest = AssetBundle.LoadFromFileAsync("Mods/Custom_Plant_Bundles/firepeashooter");
            //AssetBundleCreateRequest obsidianPeaBundleRequest = AssetBundle.LoadFromFileAsync("Mods/Custom_Plant_Bundles/obsidian_peashooter");

            AssetBundle HybridMelonAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.hybridmelon");
            AssetBundle DawningShroomAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.dawning_shroom");
            AssetBundle SniperHypnoAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.sniperhypno");
            AssetBundle EventiShroomAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.eventishroom");
            AssetBundle SniperFreezeAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.sniperfreeze");
            AssetBundle DecaySniperAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.stinkper");
            AssetBundle FrenzergAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.frenzerg");
            AssetBundle UltimateHypnoMagnetAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.ulthypnet");
            AssetBundle ClusterDoomAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.clusterdoom");
            AssetBundle SunnySniperAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.sunnysniper");
            AssetBundle GarlicPuffAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.garpuff");
            AssetBundle IceChomperAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.icechomper");
            AssetBundle IceDoomUltimateStarAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.icedoomultistar");
            AssetBundle IceFlowerAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.iceflower");
            AssetBundle RichSunflowerAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.richsunflower");
            AssetBundle FireFlowerAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.fireflower");
            AssetBundle JalaChomperAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.jalachomper");
            AssetBundle DoomFlowerAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.doomflower");
            AssetBundle ObsidianChomperAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.obisidianchomper");
            AssetBundle SunnyGatlingAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.sunnygatling");
            AssetBundle FumeUmbrellaAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.fumeumbrella");
            AssetBundle FrostUmbrellaAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.frostumbrella");
            AssetBundle SunnyCommandoAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.sunnycommando");
            AssetBundle MelonadeMortarAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.melonademortar");
            AssetBundle ExplodoNutAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.explodonut");
            AssetBundle MarigoldBombAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.marigoldbomb");
            AssetBundle TwinTycoonShooterAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.twintycoonshooter");
            AssetBundle GarlicBoomAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.garlicboom");
            AssetBundle SilverGatlingAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.silvergatling");
            AssetBundle GoldenGatlingAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.goldengatling");
            AssetBundle GoldenTycoonGatlingAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.goldentycoongatling");
            AssetBundle ObsidianSeedAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.obsidianseed");
            AssetBundle LaserIceUmbrellaAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.lasericeumbrella");
            AssetBundle SuperGatlingMineAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.supergatlingmine");
            AssetBundle SummerCabbageAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.summercabbage");
            AssetBundle BladeStarAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.bladestar");
            AssetBundle MagmaShroomAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.magmashroom");
            AssetBundle JalaHypnoAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.jalahypno");
            AssetBundle UltimateJalaDoomFumeAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.ultimatejaladoomfume");
            AssetBundle UmbrellaMineAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.umbrellamine");
            AssetBundle ShrineAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.shrine");
            AssetBundle SuperIceCattailAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.supericecattail");
            AssetBundle SuperFireCattailAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.superfirecattail");
            AssetBundle SuperFireGloomAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.superfiregloom");
            AssetBundle PitcherMysteryAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.pitchermystery");
            AssetBundle StargloomAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.stargloom");
            AssetBundle SuperFireStarAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.superfirestar");
            AssetBundle TwinDoomNutAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.twindoomnut");
            AssetBundle DoomNutBloverAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.doomnutblover");
            AssetBundle ProtalHelmetGatlingAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.protalhelmetgatling");
            AssetBundle SaladGatlingAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.saladgatling");
            AssetBundle BushPlantAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.bushplant");
            AssetBundle HellsNutAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.hellnut");
            AssetBundle PickeledPepperAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.pickeledpepper");
            AssetBundle MegaDoomCabbageAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.megadoomcabbage");
            AssetBundle ShrinePotAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.shrinepot");

            //Zombies\\
            AssetBundle HypnoPaperZombieAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.hypnopaperzombie");
            AssetBundle FireClawZombieAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.fireclawzombie");
            AssetBundle DeathcatcherAssetBundle = ResourceLoader.LoadBundleFromEmbedded("Ideas_Custom.Resources.AssetBundles.deathcatcher");

            //Misc.\\
            Sprite BossRushLogo = ResourceLoader.LoadSpriteFromEmbedded("Ideas_Custom.Resources.Images.bossrush.jpg", 50);
            DarkHelmetBullet = ResourceLoader.LoadSpriteFromEmbedded("Ideas_Custom.Resources.Images.Bullets.DarkHelmetBullet.png", 100);
            BuckportalBullet = ResourceLoader.LoadSpriteFromEmbedded("Ideas_Custom.Resources.Images.Bullets.BuckportalBullet.png", 100);
            DoomCabbageBullet = ResourceLoader.LoadSpriteFromEmbedded("Ideas_Custom.Resources.Images.Bullets.DoomCabbageBullet.png", 100);
            /*if (PickeledPepperAssetBundle == null)
            {
                MelonLogger.Msg("Missing Pickled Pepper's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(PickeledPepperAssetBundle, "JalapenoPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(PickeledPepperAssetBundle, "JalapenoPreview");
                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Pickled Pepper's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Jalapeno, PickledPepper>(843, prefabObj, previewObj, new(), 1.9f, 0f, 20, 300, 5f, 125);
                    CustomCore.TypeMgrExtra.IsFirePlant.Add((PlantType)843);
                    CustomCore.AddPlantAlmanacStrings(843, "Pickled Pepper", "(@theoneandonly30) Burns an entire... column?\n\n<color=#3D1400>Damage: </color><color=#8B0000> 1800 (Cremator) </color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Releases a column of flames on the column it is planted in that deals 1800 cremator damage.\n\n</color>Cost : <color=#8B0000>125 Sun</color>\n\n</color>Recharge : <color=#8B0000>25 Seconds</color>");
                }
            }
            if (HellsNutAssetBundle == null)
            {
                MelonLogger.Msg("Missing Hells Nut's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(HellsNutAssetBundle, "HellNutPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(HellsNutAssetBundle, "HellNutPreview");
                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Hells Nut's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Plant, Hellnut>(844, prefabObj, previewObj, new(), 1.9f, 0f, 20, 8000, 5f, 325);
                    CustomCore.TypeMgrExtra.UncrashablePlants.Add((PlantType)844);
                    CustomCore.TypeMgrExtra.IsNut.Add((PlantType)844);
                    CustomCore.AddPlantAlmanacStrings(844, "Hell's Nut", "(@amrailed) A nut formed within hell's depth.\n\n<color=#3D1400>Toughness: </color><color=#8B0000> 8000 </color>\n\n<color=#3D1400>Damage: </color><color=#8B0000> 35 / Bite</color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> When eaten, inflicts, enflamed, and a single karma point to all zombies near it.\n<color=black>•</color> \n\n</color>Fusion Formula : <color=#8B0000> (Wall-Nut + Jalapeno) + (Tangle-Kelp + Jalapeno) </color>");
                }
            }*/
            if (MegaDoomCabbageAssetBundle == null)
            {
                MelonLogger.Msg("Missing Mega Doom Cabbage's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(MegaDoomCabbageAssetBundle, "DoomCabbagePrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(MegaDoomCabbageAssetBundle, "DoomCabbagePreview");
                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Mega Doom Cabbage's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Cabbage, MegaDoomCabbage>(846, prefabObj, previewObj, new()
                    {
                        (1111, 11), (11, 1111)
                    }, 1.9f, 0f, 600, 300, 5f, 675);
                    CustomCore.AddFusion(1111, 846, 1);
                    CustomCore.AddFusion(1111, 1, 864);
                    CustomCore.AddPlantAlmanacStrings(846, "Nemesis Cabbage", "(@theoneandonly30) Immerse oneself in pure death.\n\n<color=#3D1400>Damage: </color><color=#8B0000> 600 * 5 / 2 seconds</color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Shoots 5 doom cabbages that explode in 3x3 area, dealing 600 damage initial damage and then another 600 area damage which also applies 1 Ember Score.\n<color=black>•</color> Modifier Affilation 1 : Each projectile has a 5% chance to release a doom detonation that deals 12800 damage, chances increase with each ember score.\n<color=black>•</color> Modifier Affilation 2 : Deals 2.5x explosion damage and 10x direct hit damage.\n\n</color>Fusion Formula : <color=#8B0000>Helios Cabbage + Doom-Shroom</color>");
                }
            }
            if (ShrinePotAssetBundle == null)
            {
                MelonLogger.Msg("Missing Shrine Pot's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(ShrinePotAssetBundle, "PotPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(ShrinePotAssetBundle, "PotPreview");
                GameObject particleObj = ResourceLoader.GetResourceFromBundle(ShrinePotAssetBundle, "Tentacle");
                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Shrine Pot's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Pot, ShrinePot>(845, prefabObj, previewObj, new()
                    {
                        (27, 820)
                    }, 1.9f, 0f, 100, 300, 5f, 25);
                    CustomCore.TypeMgrExtra.IsPot.Add((PlantType)845);
                    CustomCore.RegisterCustomParticle((ParticleType)250, particleObj);
                    CustomCore.AddPlantAlmanacStrings(845, "Basin", "(@amrailed) A souless pot.\n\n<color=#3D1400>Damage: </color><color=#8B0000> 100 / 0.5 Seconds </color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Deals a constant 100 damage every 0.5 seconds in a 5x1 area (5 column, 1 row).\n<color=black>•</color> Eats 100 health away on the plant planted on the pot every second to increase its base damage by 20, upon reaching 500 damage, right click on it to sacrifice the plant ontop of it and for every 100 health the plant ontop of it has, deals 15 damage to all zombies on the lawn also temporarily increase damage by 25 for every 100 health.\n<color=black>•</color> If a Shrine is placed ontop of this pot, it will naturally give an extra 5 score if a ritual is successful, and increases the pot's base damage by 25 for every successful ritual.\n\n</color>Fusion Formula : <color=#8B0000>Shrine > Flower Pot</color>");
                }
            }
            if (BushPlantAssetBundle == null)
            {
                MelonLogger.Msg("Missing Bush's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(BushPlantAssetBundle, "BushPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(BushPlantAssetBundle, "BushPreview");
                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Bush's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Plant, BushPlant>(842, prefabObj, previewObj, new(), 1.9f, 0f, 20, 300, 5f, 25);
                    CustomCore.TypeMgrExtra.IsCaltrop.Add((PlantType)842);
                    CustomCore.AddPlantAlmanacStrings(842, "Bush", "(@amrailed) Its a joke... Right?\n\n<color=#3D1400>Damage: </color><color=#8B0000> 2-4 / 0.35 seconds</color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Ignored by zombies, constantly deals 20-80 damage every second to all zombies in the bush. With a 1 in 700 chance of completely annhilating the zombie (no restrictions).\n\n</color>Cost : <color=#8B0000>25 Sun</color>\n</color>Recharge : <color=#8B0000>5 Seconds</color>");
                }
            }
            if (SaladGatlingAssetBundle == null)
            {
                MelonLogger.Msg("Missing Salad Gatling's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SaladGatlingAssetBundle, "GatlingPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SaladGatlingAssetBundle, "GatlingPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Salad Gatling's Asset Bundle");
                    CustomCore.RegisterCustomPlant<GatlingPea, SaladGatling>(841, prefabObj, previewObj, new List<(int, int)>
                    { (1032, 1126), (1126, 1032) }, 1.5f, 0f, 160, 4000, 18f, 900);
                    CustomCore.AddUltimatePlant((PlantType)841);
                    SaladGatling.buff1 = CustomCore.RegisterCustomBuff("Salad Gatling : All modes has been enhanced to their supreme version.", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)841);
                    SaladGatling.buff2 = CustomCore.RegisterCustomBuff("Salad Gatling : Adds 2 new modes and doubles the base damage.", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)841);
                    CustomCore.AddPlantAlmanacStrings(841, "Salad Gatling", "(@wladimirkisl) Provides extreme fire power.\n\n<color=#3D1400>Damage: </color><color=#8B0000> Normal/Flower : 140*4/1 Seconds | Shotgun : 140*5/1.5 Seconds | Flow/Spray : 140/0.25 Second </color>\n\n<color=black>Modes (Click to cycle between them): </color><color=#8B0000>\n<color=black>•</color> Regular Mode : Shoot 4 Salads in the same lane every second, (Supreme: Enhanced to 4 salads per shot, 4 shots per burst).\n<color=black>•</color> Shotgun Mode : Shoot 5 Salads in 5 different directions every 1.5 seconds, (Supreme: Enhanced to 11 salads and 2 bursts).\n<color=black>•</color> Flow Mode : Shoots 1 salad every 0.25 seconds (Supreme: Enhanced to 4 salads).\n<color=black>•</color> (Requires Blooming Crest buff) Flower Mode : Shoots 8 salads in a circular pattern every second (Supreme: Enhanced to 32 salads).\n<color=black>•</color> (Requires Blooming Crest buff) Spray Mode : Shoots 1 salad in different directions every 0.25 seconds (Supreme: Adds 2 more salads alongside the first).\n\n<color=black>Odyssey Modifiers: </color><color=#8B0000>\n<color=black>•</color> Natural Evolution : All modes has been enhanced to their supreme version. \n<color=black>•</color> Blooming Crest : Adds 2 new modes and doubles the base damage. \n\n</color>Fusion Formula : <color=#8B0000> (Cabbage-Pult + Kernel-Pult + Melon-Pult) + Gatling-Pea </color>");


                    CustomLevelData SaladGatlingLevelData = new CustomLevelData();
                    SaladGatlingLevelData.BgmType = MusicType.Day;
                    SaladGatlingLevelData.SceneType = SceneType.Day_6;
                    SaladGatlingLevelData.AdvBuffs = () =>
                    {
                        var list = new List<int>();
                        list.Add(PortalHelmetGatling.buff1);
                        list.Add(PortalHelmetGatling.buff2);
                        list.Add(16);
                        list.Add(13);
                        list.Add(9);
                        list.Add(27);
                        list.Add(32);
                        return list;
                    };
                    SaladGatlingLevelData.PreSelectCards = () =>
                    {
                        var list = new List<PlantType>();
                        list.Add((PlantType)841);
                        list.Add((PlantType)1032);
                        list.Add((PlantType)1126);
                        return list;
                    };
                    SaladGatlingLevelData.ZombieList = () =>
                    {
                        var list = new List<ZombieType>();
                        list.Add(ZombieType.NormalZombie);
                        list.Add(ZombieType.ConeZombie);
                        list.Add(ZombieType.BucketZombie);
                        list.Add(ZombieType.FootballZombie);
                        list.Add(ZombieType.TallNutFootballZombie);
                        list.Add(ZombieType.FootballDrown);
                        list.Add(ZombieType.BlackFootball_a);
                        list.Add(ZombieType.BlackFootball_b);
                        list.Add(ZombieType.BlackFootball_c);
                        list.Add(ZombieType.UltimateFootballZombie);
                        list.Add(ZombieType.FlagFootball);
                        return list;
                    };
                    SaladGatlingLevelData.WaveCount = () => (30);
                    SaladGatlingLevelData.Logo = previewObj.GetComponent<SpriteRenderer>().sprite;
                    SaladGatlingLevelData.Name = () => ("Salad Gatling \nShowcase");
                    SaladGatlingLevelData.RowCount = 6;

                    var bTag = default(Board.BoardTag);
                    bTag.enableAllTravelPlant = true;
                    bTag.enableTravelPlant = true;
                    SaladGatlingLevelData.BoardTag = bTag;
                    CustomCore.RegisterCustomLevel(SaladGatlingLevelData);
                }
            }
            if (ProtalHelmetGatlingAssetBundle == null)
            {
                MelonLogger.Msg("Missing Protal Helmet Gatling's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(ProtalHelmetGatlingAssetBundle, "ProtalPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(ProtalHelmetGatlingAssetBundle, "ProtalPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Protal Helmet Gatling's Asset Bundle");
                    CustomCore.RegisterCustomPlant<GatlingPea, PortalHelmetGatling>(840, prefabObj, previewObj, new List<(int, int)>
                    { (1208, 1306), (1306, 1208) }, 1.5f, 0f, 60, 4000, 18f, 700);
                    CustomCore.TypeMgrExtra.IsMagnetPlants.Add((PlantType)840);
                    CustomCore.AddUltimatePlant((PlantType)840);
                    PortalHelmetGatling.buff1 = CustomCore.RegisterCustomBuff("Cosmic Quarterback Gatling : Start every round with a single overcharge that can be used at any point. Overcharging also slowly restores health of all plants near it.", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)840);
                    PortalHelmetGatling.buff2 = CustomCore.RegisterCustomBuff("Cosmic Quarterback Gatling : Increased damage during overcharge to 5x more, Increased overcharge duration, During overcharge the first shot of a burst will shoot blackportal footpeas going in different directions.", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)840);
                    CustomCore.AddPlantAlmanacStrings(840, "Cosmic Quarterback Gatling", "(@theoneandonly30) Only few could produce such like these.\n\n<color=#3D1400>Damage: </color><color=#8B0000> 160*4/1 second | 160*3 + 600/1 second every fourth burst (Average : 160 * 15 + 600 / 4 seconds) </color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Each burst shoots 3 buck peas and 1 blackportal footpea that knockback zombies. Blackportal footpeas applies 1 second of chornoshift, deals 1000 armor damage alongside 480 damage, deals 2000 extra armor damage to Tall-nut Footballs, and 1000 extra armor damage to Footballs. Iron peas and Blackportal Footpeas and has a 75% chance of bouncing, on the nineth burst shoot 3 buck peas and a rift bomb which teleport all zombies hit to the beginning, aswell as inflicting 3 seconds of chornoshift upon the first zombie hit, (or deals 2 tile knockback to mini-bosses (ignores immunities)).\n<color=black>•</color> Each shot stores an energy point, once 64 points are collected, click on it to boost its attack speed by 5 times for 6 seconds. Points aren't gained during this period. It will start pulsing with energy when its ready to overcharge. \n\n<color=black>Odyssey Modifiers: </color><color=#8B0000>\n<color=black>•</color> Reserve Energy : Start every round with a single overcharge that can be used at any point. Overcharging also slowly restores health of all plants near it (750 every second). \n<color=black>•</color> Relentless Attack : Increased damage during overcharge to 3x more (5x for football peas), Increased overcharge duration, During overcharge the first shot of a burst will shoot blackportal footpeas going in different directions. \n\n</color>Fusion Formula : <color=#8B0000> (Peashooter + Chrono-Device) + (Gatling-Pea + Football Helmet) </color>");


                    CustomLevelData PortalHelmetPeaLevelData = new CustomLevelData();
                    PortalHelmetPeaLevelData.BgmType = MusicType.Day;
                    PortalHelmetPeaLevelData.SceneType = SceneType.Day_6;
                    PortalHelmetPeaLevelData.AdvBuffs = () =>
                    {
                        var list = new List<int>();
                        list.Add(PortalHelmetGatling.buff1);
                        list.Add(PortalHelmetGatling.buff2);
                        list.Add(9);
                        list.Add(16);
                        list.Add(26);
                        list.Add(27);
                        list.Add(32);
                        return list;
                    };
                    PortalHelmetPeaLevelData.PreSelectCards = () =>
                    {
                        var list = new List<PlantType>();
                        list.Add((PlantType)840);
                        list.Add((PlantType)1208);
                        list.Add((PlantType)1306);
                        return list;
                    };
                    PortalHelmetPeaLevelData.ZombieList = () =>
                    {
                        var list = new List<ZombieType>();
                        list.Add(ZombieType.NormalZombie);
                        list.Add(ZombieType.ConeZombie);
                        list.Add(ZombieType.BucketZombie);
                        list.Add(ZombieType.FootballZombie);
                        list.Add(ZombieType.TallNutFootballZombie);
                        list.Add(ZombieType.FootballDrown);
                        list.Add(ZombieType.BlackFootball_a);
                        list.Add(ZombieType.BlackFootball_b);
                        list.Add(ZombieType.BlackFootball_c);
                        list.Add(ZombieType.UltimateFootballZombie);
                        list.Add(ZombieType.FlagFootball);
                        list.Add(ZombieType.ProtalZombie);
                        return list;
                    };
                    PortalHelmetPeaLevelData.ZombieHealthRate = () => (2);
                    PortalHelmetPeaLevelData.WaveCount = () => (40);
                    PortalHelmetPeaLevelData.Logo = previewObj.GetComponent<SpriteRenderer>().sprite;
                    PortalHelmetPeaLevelData.Name = () => ("Cosmic Quarterback \nShowcase");
                    PortalHelmetPeaLevelData.RowCount = 6;

                    var bTag = default(Board.BoardTag);
                    bTag.enableTravelPlant = true;
                    PortalHelmetPeaLevelData.BoardTag = bTag;
                    CustomCore.RegisterCustomLevel(PortalHelmetPeaLevelData);
                }
            }
            if (StargloomAssetBundle == null)
            {
                MelonLogger.Msg("Missing Star Gloom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(StargloomAssetBundle, "StarGloomPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(StargloomAssetBundle, "StarGloomPreview");
                GameObject particleObj = ResourceLoader.GetResourceFromBundle(StargloomAssetBundle, "GloomStar");
                if ((prefabObj != null) && (previewObj != null) && (particleObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Star Gloom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<GloomShroom, StarGloom>(836, prefabObj, previewObj, new List<(int, int)>
                    { (1070, 23), (23, 1070) }, 1.9f, 0f, 20, 300, 30f, 425);
                    CustomCore.AddPlantAlmanacStrings(836, "Stargloom-Shroom", "(@theoneandonly30) Shoots stars alongside fumes.\n\n<color=#3D1400>Damage: </color><color=#8B0000>(20+20*5)*4/1.9 second</color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Shoots 8 stars in a circular pattern alongside fumes.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Gloom-Shroom + Starfruit</color>");
                }
            }

            if (SuperFireGloomAssetBundle == null)
            {
                MelonLogger.Msg("Missing Super Fire Gloom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SuperFireGloomAssetBundle, "FireGloomPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SuperFireGloomAssetBundle, "FireGloomPreview");
                GameObject particleObj = ResourceLoader.GetResourceFromBundle(SuperFireGloomAssetBundle, "GloomFire");

                if ((prefabObj != null) && (previewObj != null) && (particleObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Super Fire Gloom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<FireGloom, SuperFireGloom>(823, prefabObj, previewObj, new List<(int, int)>
                    { (1071, 817), (817, 1071), (837, 1070) }, 1.9f, 0f, 120, 300, 30f, 575);
                    CustomCore.TypeMgrExtra.IsFirePlant.Add((PlantType)823);
                    CustomCore.AddUltimatePlant((PlantType)823);
                    CustomCore.RegisterCustomParticle((ParticleType)200, particleObj);
                    SuperFireGloom.buff1 = CustomCore.RegisterCustomBuff("Hellfire Gloom : Heatwave deals double damage and the penalty for distance is heavily decreased by 76%", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)823);
                    SuperFireGloom.buff2 = CustomCore.RegisterCustomBuff("Hellfire Gloom : Every shot gives a frenzy point. Upon reaching 36 points it boosts damage for 8 seconds. Cannot gain frenzy points with boosted attacks.", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)823);
                    CustomCore.AddPlantAlmanacStrings(823, "Hellfire Gloom-shroom", "(@theoneandonly30) A beast that is easily capable of defeating hordes of weak zombies.\n\n<color=#3D1400>Damage: </color><color=#8B0000>120*4/1.9 second</color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Attack inflict enflamed and ember.\n<color=black>•</color> Every 12th shot releases a heatwave which deals 900 damage (scales based on distance.) and applies the ember effect to all zombies on the lawn.\n\n<color=black>Odyssey Modifiers: </color><color=#8B0000>\n<color=black>•</color> Hellwave : Heatwave deals double damage and the penalty for distance is heavily decreased by 76% \n<color=black>•</color> Superfrenzy : Every shot gives a frenzy point. Upon reaching 36 points it boosts damage for 8 seconds. Cannot gain frenzy points with boosted attacks. \n\n</color><size=36>Fusion Formula : <color=#8B0000>(Gloom-Shroom + Jalapeno) + (Fume-Shroom + Jalapeno + Doom-Shroom)</color>");


                    CustomLevelData SuperFireGloomLevelData = new CustomLevelData();
                    SuperFireGloomLevelData.BgmType = MusicType.Night;
                    SuperFireGloomLevelData.SceneType = SceneType.Night_6;
                    SuperFireGloomLevelData.AdvBuffs = () =>
                    {
                        var list = new List<int>();
                        list.Add(SuperFireGloom.buff1);
                        list.Add(SuperFireGloom.buff2);
                        list.Add(16);
                        list.Add(13);
                        list.Add(9);
                        list.Add(27);
                        list.Add(32);
                        return list;
                    };
                    SuperFireGloomLevelData.PreSelectCards = () =>
                    {
                        var list = new List<PlantType>();
                        list.Add((PlantType)817);
                        list.Add((PlantType)1071);
                        list.Add((PlantType)823);
                        return list;
                    };
                    SuperFireGloomLevelData.ZombieList = () =>
                    {
                        var list = new List<ZombieType>();
                        list.Add(ZombieType.NormalZombie);
                        list.Add(ZombieType.ConeZombie);
                        list.Add(ZombieType.BucketZombie);
                        list.Add(ZombieType.SuperDancePolZombie);
                        list.Add(ZombieType.Jackson_a);
                        list.Add(ZombieType.Jackson_b);
                        list.Add(ZombieType.Jackson_c);
                        list.Add(ZombieType.Driver_a);
                        list.Add(ZombieType.Driver_b);
                        list.Add(ZombieType.Driver_c);
                        list.Add(ZombieType.UltimateJacksonDriver);
                        list.Add(ZombieType.CherryPaperZ95);
                        return list;
                    };
                    SuperFireGloomLevelData.WaveCount = () => (30);
                    SuperFireGloomLevelData.Logo = previewObj.GetComponent<SpriteRenderer>().sprite;
                    SuperFireGloomLevelData.Name = () => ("Hellfire Gloom \nShroom Showcase");
                    SuperFireGloomLevelData.RowCount = 6;

                    var bTag = default(Board.BoardTag);
                    bTag.isNight = true;
                    bTag.enableAllTravelPlant = true;
                    bTag.enableTravelPlant = true;
                    SuperFireGloomLevelData.BoardTag = bTag;
                    CustomCore.RegisterCustomLevel(SuperFireGloomLevelData);
                }
            }

            if (SuperFireStarAssetBundle == null)
            {
                MelonLogger.Msg("Missing Super Fire Star's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SuperFireStarAssetBundle, "SuperFireStarPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SuperFireStarAssetBundle, "SuperFireStarPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Super Fire Star's Asset Bundle");
                    CustomCore.RegisterCustomPlant<StarFruit, SuperFireStar>(837, prefabObj, previewObj, new List<(int, int)>
                    { (823, 23), (23, 823) }, .5f, 0f, 20, 300, 30f, 425);
                    CustomCore.TypeMgrExtra.IsFirePlant.Add((PlantType)837);
                    CustomCore.AddUltimatePlant((PlantType)837);
                    CustomCore.AddPlantAlmanacStrings(837, "Hellfire Star", "(@theoneandonly30) Shoots deadly doomfire stars.\n\n<color=#3D1400>Damage: </color><color=#8B0000>360*5/0.5 seconds</color>\n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Shoots 5 stars which inflict ember and enflamed effect.\n<color=black>•</color> Heatwave damage is cut in half but every 2nd heatwave is accompanied by a doom star card.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Gloom-Shroom + Starfruit</color>");
                }
            }

            if (PitcherMysteryAssetBundle == null)
            {
                MelonLogger.Msg("Missing Pitcher Mystery's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(PitcherMysteryAssetBundle, "PitcherMisteryPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(PitcherMysteryAssetBundle, "PitcherMisteryPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Pitcher Mystery's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Plant, PitcherMystery>(835, prefabObj, previewObj, new List<(int, int)>
                    { }, 1.9f, 0f, 80, 300, 7.5f, 125);
                    CustomCore.AddPlantAlmanacStrings(835, "Pitcher Mystery", "(@theoneandonly30) ???.");
                }
            }

            if (TwinDoomNutAssetBundle == null)
            {
                MelonLogger.Msg("Missing Twin Doom Nut's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(TwinDoomNutAssetBundle, "WallNutPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(TwinDoomNutAssetBundle, "WallNutPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Twin Doom Nut's Asset Bundle");
                    CustomCore.RegisterCustomPlant<DoomNut, TwinDoomNut>(838, prefabObj, previewObj, new List<(int, int)>
                    { (1236, 3), (3, 1236) }, 0f, 0f, 0, 4000, 30f, 300);
                    CustomCore.TypeMgrExtra.IsNut.Add((PlantType)838);
                    CustomCore.TypeMgrExtra.UncrashablePlants.Add((PlantType)838);
                    CustomCore.AddUltimatePlant((PlantType)838);
                    TwinDoomNut.buff1 = CustomCore.RegisterCustomBuff("Twin Doom-Nut/Blover Doom-Nut : The attack range is significantly increased.", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)838);
                    TwinDoomNut.buff2 = CustomCore.RegisterCustomBuff("Twin Doom-Nut : Reduces the amount of Ember Points needed to apply the Ember debuff. Upon inflicting the Ember debuff through Ember Points, does a small explosion that deals 10 damage and spreads 2 Ember Points. This modifier affects all doom plants.", BuffType.AdvancedBuff, () => (true), 3500, default, (PlantType)838);
                    CustomCore.AddPlantAlmanacStrings(838, "Twin Doom Nut", "(@theon0eandonly30) Does damage to a group of zombies and heals. \n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Has anti-crush properties. \n<color=black>•</color> Damage received capped at 50. \n<color=black>•</color> Every 3 seconds every zombie near it receives (Ember Score*30) damage and knockbacks them by (Ember Score/10) tiles then they give them a single ember score, Then for every embered zombie near it the plant heals by 20 and then heal by 400. \n\n<color=#6f19bf>Ember Score: </color><color=#8B0000>\nUpon reaching 5 Ember Points apply Embered debuff to the affected zombie, Ember Points give advantages to certain doom plants and ember points can be given by certain doom plants.\n\n<color=black>Odyssey Modifiers: </color><color=#8B0000>\n<color=black>•</color> Uncontained : The attack range is significantly increases. \n<color=black>•</color> Blooming Ember : Reduces the amount of Ember Points needed to apply the Ember debuff. Upon inflicting the Ember debuff through Ember Points, does a small explosion that deals 10 damage and spreads 2 Ember Points. This modifier affects all doom plants. \n\n</color><size=36>Fusion Formula : <color=#8B0000>Doom Nut + Wall Nut</color>");
                }

                CustomLevelData DoomsdayLevelData = new CustomLevelData();
                DoomsdayLevelData.BgmType = MusicType.UltimateBattle;
                DoomsdayLevelData.SceneType = SceneType.Night_6;
                DoomsdayLevelData.NeedSelectCard = false;
                DoomsdayLevelData.Logo = previewObj.GetComponent<SpriteRenderer>().sprite;
                DoomsdayLevelData.Name = () => ("Doomsday!");
                DoomsdayLevelData.RowCount = 6;
                DoomsdayLevelData.AdvBuffs = () =>
                {
                    var list = new List<int>();
                    list.Add(16);
                    list.Add(13);
                    list.Add(9);
                    list.Add(27);
                    list.Add(32);
                    return list;
                };
                DoomsdayLevelData.ConveyBeltPlantTypes = () =>
                {
                    var list = new List<PlantType>();
                    list.Add(PlantType.DoomPeashooter);
                    list.Add(PlantType.ScaredyDoom);
                    list.Add(PlantType.DoomNut);
                    list.Add(PlantType.DoomBlover);
                    list.Add(PlantType.DoomFume);
                    list.Add(PlantType.DoomStar);
                    list.Add(PlantType.PuffDoom);
                    list.Add(PlantType.DoomCactus);
                    list.Add(PlantType.DoomChomper);
                    return list;
                };
                DoomsdayLevelData.ZombieList = () =>
                {
                    var list = new List<ZombieType>();
                    list.Add(ZombieType.NormalZombie);
                    list.Add(ZombieType.ConeZombie);
                    list.Add(ZombieType.BucketZombie);
                    list.Add(ZombieType.BucketNutZombie);
                    list.Add(ZombieType.DancePolZombie2);
                    list.Add(ZombieType.JacksonZombie);
                    list.Add(ZombieType.Gargantuar);
                    list.Add(ZombieType.RedGargantuar);
                    list.Add(ZombieType.BlueGargantuar);
                    return list;
                };
                DoomsdayLevelData.WaveCount = () => (40);

                var bTag = default(Board.BoardTag);
                bTag.isNight = true;
                bTag.isConvey = true;
                bTag.isFreeCardSelect = false;
                DoomsdayLevelData.BoardTag = bTag;
                CustomCore.RegisterCustomLevel(DoomsdayLevelData);


                CustomLevelData DoomsdayOdysseyLevelData = new CustomLevelData();
                DoomsdayOdysseyLevelData = DoomsdayLevelData;
                DoomsdayOdysseyLevelData.Name = () => ("Doomsday! 2");
                DoomsdayOdysseyLevelData.AdvBuffs = () =>
                {
                    var list = new List<int>();
                    list.Add(SuperFireGloom.buff1);
                    list.Add(SuperFireGloom.buff2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(16);
                    list.Add(13);
                    list.Add(9);
                    list.Add(27);
                    list.Add(32);
                    return list;
                };
                DoomsdayOdysseyLevelData.ConveyBeltPlantTypes = () =>
                {
                    var list = new List<PlantType>();
                    list.Add(PlantType.WallNut);
                    list.Add(PlantType.DoomNut);
                    list.Add(PlantType.DoomShroom);
                    list.Add((PlantType)1071);
                    list.Add((PlantType)817);
                    list.Add(PlantType.DoomStar);
                    list.Add(PlantType.Blover);
                    list.Add((PlantType)818);
                    list.Add((PlantType)817);
                    list.Add(PlantType.TallIceNut);
                    list.Add(PlantType.TallFireNut);
                    list.Add(PlantType.DoomGatling);
                    return list;
                };
                DoomsdayOdysseyLevelData.ZombieList = () =>
                {
                    var list = new List<ZombieType>();
                    list.Add(ZombieType.NormalZombie);
                    list.Add(ZombieType.ConeZombie);
                    list.Add(ZombieType.BucketZombie);
                    list.Add(ZombieType.CherryPaperZ95);
                    list.Add(ZombieType.Driver_a);
                    list.Add(ZombieType.Driver_b);
                    list.Add(ZombieType.QuickJacksonZombie);
                    list.Add(ZombieType.UltimateJacksonDriver);
                    list.Add(ZombieType.UltimateFootballZombie);
                    list.Add(ZombieType.GatlingBlackFootball);
                    list.Add(ZombieType.BlackFootball);
                    return list;
                };

                var bTag2 = default(Board.BoardTag);
                bTag2.isNight = true;
                bTag2.enableAllTravelPlant = true;
                bTag2.enableTravelPlant = true;
                bTag2.isConvey = true;
                bTag2.isFreeCardSelect = false;
                DoomsdayOdysseyLevelData.BoardTag = bTag2;
                CustomCore.RegisterCustomLevel(DoomsdayOdysseyLevelData);
            }

            if (DoomNutBloverAssetBundle == null)
            {
                MelonLogger.Msg("Missing Blover Doom Nut's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(DoomNutBloverAssetBundle, "NutBloverPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(DoomNutBloverAssetBundle, "NutBloverPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Blover Doom Nut's Asset Bundle");
                    CustomCore.RegisterCustomPlant<NutBlover, DoomBloverNut>(839, prefabObj, previewObj, new List<(int, int)>
                    { (838, 22), (22, 838) }, 0f, 0f, 0, 4000, 30f, 300);
                    CustomCore.TypeMgrExtra.IsNut.Add((PlantType)839);
                    CustomCore.TypeMgrExtra.FlyingPlants.Add((PlantType)839);
                    CustomCore.AddUltimatePlant((PlantType)839);
                    CustomCore.AddPlantAlmanacStrings(839, "Doom Blover-Nut", "(@theoneandonly30) Heals and kills. \n\n<color=black>Specials: </color><color=#8B0000>\n<color=black>•</color> Every 4.5 seconds every zombie near it receives (Ember Score*15) damage and knockbacks them by (Ember Score/10) tiles (2 tile limit) then they give them a single ember score, Then for every embered zombie near it it will heal every plant below it by 35 and then heal by 400. \n</color><size=36>Fusion Formula : <color=#8B0000>Twin Doom Nut < Blover</color>");
                }
            }

            if (SuperFireCattailAssetBundle == null)
            {
                MelonLogger.Msg("Missing Super Fire Cattail's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SuperFireCattailAssetBundle, "FireCattailPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SuperFireCattailAssetBundle, "FireCattailPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Super Fire Cattail's Asset Bundle");
                    CustomCore.RegisterCustomPlant<FireCattail, SuperFireCattail>(822, prefabObj, previewObj, new List<(int, int)>
                    { (1069, 16) }, 1.5f, 0f, 180, 300, 30f, 425);
                    CustomCore.TypeMgrExtra.IsWaterPlant.Add((PlantType)822);
                    CustomCore.TypeMgrExtra.IsFirePlant.Add((PlantType)822);
                    CustomCore.AddUltimatePlant((PlantType)822);
                    SuperFireCattail.buff1 = CustomCore.RegisterCustomBuff("Blazing Cattail : Shots can leave a sea of fire (5% chance).", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)822);
                    SuperFireCattail.buff2 = CustomCore.RegisterCustomBuff("Blazing Cattail : Attacks deal 5x damage.", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)822);
                    CustomCore.AddPlantAlmanacStrings(822, "Blazing Cattail", "(@pvzfusionmatthewfanart) Shoots Blazing Spikes that cause an inferno!\n\n<color=#3D1400>Damage: </color><color=#8B0000>180*2/1.5 second</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> When projectiles land it will generate an firey-explosion (3x3) that deals 300 damage and enflames zombies, It will also shoot 8 fire spikes in a circle which deals 120 damage each.\n\n<color=black>Odyssey Modifier's: </color><color=#8B0000>\n<color=black>•</color> Shots can leave a sea of fire (5% chance), this lasts for 8 seconds and deal 30 damage every 0.02 second. \n<color=black>•</color> Attacks deal 5x damage.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Flame Cattail + Jalapeno</color>");
                }
            }

            if (SuperIceCattailAssetBundle == null)
            {
                MelonLogger.Msg("Missing Super Ice Cattail's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SuperIceCattailAssetBundle, "IceCattailPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SuperIceCattailAssetBundle, "IceCattailPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Super Ice Cattail's Asset Bundle");
                    CustomCore.RegisterCustomPlant<IceCattail, SuperIceCattail>(821, prefabObj, previewObj, new List<(int, int)>
                    { (1068, 1212) }, 1.5f, 0f, 120, 300, 30f, 425);
                    CustomCore.TypeMgrExtra.IsWaterPlant.Add((PlantType)821);
                    CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)821);
                    CustomCore.AddUltimatePlant((PlantType)821);
                    SuperIceCattail.buff1 = CustomCore.RegisterCustomBuff("Icebomb Cattail : Shoots an explosive ice cube every 10 shots.", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)821);
                    SuperIceCattail.buff2 = CustomCore.RegisterCustomBuff("Icebomb Cattail : Attacks will now split again in 4 each dealing 30 damage also piercing 2 times (37% chance).", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)821);
                    CustomCore.AddPlantAlmanacStrings(821, "Icebomb Cattail", "(@tarfreaky7) Shoots Icicles that explode!\n\n<color=#3D1400>Damage: </color><color=#8B0000>120*2/1.5 second</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> When projectiles land it will generate an icy-explosion (3x3) that deals 200 damage and adds 5 cryo points to the surrounding zombies and chills them for 8 seconds, It will also shoot 8 icicles in a circle which deals 80 damage and pierce 2 times.\n\n<color=black>Odyssey Modifier's: </color><color=#8B0000>\n<color=black>•</color> Shoots an explosive ice cube every 10 shots. \n<color=black>•</color> Attacks will now split again in 4 each dealing 30 damage also piercing 2 times (37% chance). \n\n</color><size=36>Fusion Formula : <color=#8B0000>Frost Cattail + Frozen Cherry</color>");
                }
            }

            if (ShrineAssetBundle == null)
            {
                MelonLogger.Msg("Missing Shrine's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(ShrineAssetBundle, "ShrinePrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(ShrineAssetBundle, "ShrinePreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Shrine's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, Shrine2>(820, prefabObj, previewObj, new List<(int, int)>
                    { }, 0.5f, 0f, 0, 1000, 30f, 725);
                    //CustomCore.AddPlantAlmanacStrings(820, "Shrine", "(@amrailed) Requires Sacrifice.\n\n<color=#3D1400>Damage: </color><color=#8B0000>200/0.5 second</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Clicking with LMB cycles through its modes (Execute, Slice, Heal, Illuminate).\n<color=black>•</color> Clicking with RMB triggers the mode and sacrifices plants in a 3x3 area. Only accepts Advanced Plants (+1 Score) and Odyssey Plants (+3 Score)\n\n<color=black>Modes: </color><color=#8B0000>\n<color=black>•</color> Execute : Instantly kills (score) of the strongest zombies.\n<color=black>•</color> Slice : Deals (540*score) damage to all zombies.\n<color=black>•</color> Heal : Recovers (300*score + (5% of the plant being healed max toughness)*score) HP to all plants on the lawn.\n<color=black>•</color> Illuminate : Generates (25*score) sun from every zombie.\r\n\n</color><size=36>Cost : <color=#8B0000>725</color>\n</color><size=36>Recharge : <color=#8B0000>30 secs</color>");
                    CustomCore.AddPlantAlmanacStrings(820, "Shrine", "(@amrailed) Requires Sacrifice.\n\n<color=#3D1400>Damage: </color><color=#8B0000>(200-900)/0.5 second</color>\n\n<color=black>•</color> Clicking on it sacrifices all plants in a 3x3 area. For each 200 hp the plant sacrificed has, does these things : Heals all plants by 15, all zombies take 5 damage, increases aura damage by 2 (permanent, 900 limit), temporarily increases aura damage by 10 (temporary, slowly decays into 0, no limits).\n\n<color=black>•</color> Upon taking damage, Release a scream for 1 second that does : 0.4 tiles of knockback and 0.1 tile of true knockback, applies to cold effect, and deals 20 damage. every 0.1 second the scream is active. \r\n\n</color><size=36>Cost : <color=#8B0000>725</color>\n</color><size=36>Recharge : <color=#8B0000>30 secs</color>");
                }
            }

            if (HybridMelonAssetBundle == null)
            {
                MelonLogger.Msg("Missing Hybrid Melon Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(HybridMelonAssetBundle, "FireMelonPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(HybridMelonAssetBundle, "MelonpultPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully loaded Hybrid Melon's Asset Bundle");
                    CustomCore.RegisterCustomPlant<WinterMelon>(260, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1116, 1149),
                        new ValueTuple<int, int>(1149, 1116),
                    }, 1.5f, 0f, 300, 8000, 30f, 225);
                    CustomCore.TypeMgrExtra.IsFirePlant.Add((PlantType)260);
                    CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)260);
                    CustomCore.AddUltimatePlant((PlantType)260);
                    Hybrid_Melon.buff1 = CustomCore.RegisterCustomBuff("Swift Cloning : Attack cooldown is reduced to 0.5 seconds, now has a chance to place obsidian seed on tiles next to it", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)260);
                    Hybrid_Melon.buff2 = CustomCore.RegisterCustomBuff("Frostburnt Growth : Seed growth time is reduced to 30 seconds and it burns the lane and freezes zombies when it grows", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)260);
                    CustomCore.AddPlantAlmanacStrings(260, "Obsidian Melon", "(@amrailed) Clones itself.\n\n<color=#3D1400>Damage: </color><color=#8B0000>300/0.5 Seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a projectile lands, it will turn into an obsidian seed.\n\n<color=black>Odyssey Modifier's: </color><color=#8B0000>\n<color=black>•</color> Swift Cloning : Attack cooldown is reduced to 0.5 seconds, now has a chance to place obsidian seed on tiles next to it.\n<color=black>•</color>Frostburnt Growth : Seed growth time is reduced to 30 seconds and it burns the lane and freezes zombies when it grows.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Winter Melon + Summer Melon</color>");
                }
            }

            if (DawningShroomAssetBundle == null)
            {
                MelonLogger.Msg("Missing Dawning Shroom Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(DawningShroomAssetBundle, "FumeShroomPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(DawningShroomAssetBundle, "FumeShroomPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Dawning Shroom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<FumeShroom, DawningShroom>(824, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(7, 1031),
                    }, 1f, 0f, 20, 300, 7.5f, 100);
                    CustomCore.AddPlantAlmanacStrings(824, "Dawning Shroom", "(@Rabbit) Generates sun with ease!\n\n<color=#3D1400>Damage: </color><color=#8B0000>20/1 Second</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Everytime it shoots generate 5 sun.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Fume-Shroom + Sun-Shroom</color>");
                }
            }

            if (EventiShroomAssetBundle == null)
            {
                MelonLogger.Msg("Missing Eventi Shroom Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(EventiShroomAssetBundle, "FumeShroomPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(EventiShroomAssetBundle, "FumeShroomPreview");
                GameObject bulletObj = ResourceLoader.GetResourceFromBundle(EventiShroomAssetBundle, "DoomBullet");

                if ((prefabObj != null) && (previewObj != null) && (bulletObj != null))
                {
                    MelonLogger.Msg("Successfully Eventi Shroom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<FumeShroom, EventiShroom>(826, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(824, 7),
                        new ValueTuple<int, int>(7, 824),
                    }, 1f, 0f, 20, 300, 7.5f, 100);
                    CustomCore.AddPlantAlmanacStrings(826, "Eventi Shroom", "(@Rabbit) Make zombies feel despair.\n\n<color=#3D1400>Damage: </color><color=#8B0000>40/1 Second</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Shoots a lunar projectile alongside the fumes which deals 20 damage and has a 10% chance to curse zombies, if a zombie is already cursed deal 300 damage instead.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Dawning-Shroom+Fume-Shroom</color>");
                }
            }

            if (SniperHypnoAssetBundle == null)
            {
                MelonLogger.Msg("Missing Sniper Hypno's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SniperHypnoAssetBundle, "SniperPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SniperHypnoAssetBundle, "SniperPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Sniper Hypno's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SniperPea, SniperHypno>(825, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1109, 900),
                        new ValueTuple<int, int>(900, 1109),
                    }, 1.5f, 0f, 300, 300, 50f, 750);
                    CustomCore.AddPlantAlmanacStrings(825, "Sniper Hypno", "(@randomperson) Fill zombies with love!\n\n<color=#3D1400>Damage: </color><color=#8B0000>300/1.5 Seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Each shot hypnotizes the zombie.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sniper-Pea + Empress-Shroom</color>");
                }
            }

            if (SniperFreezeAssetBundle == null)
            {
                MelonLogger.Msg("Missing Sniper Freeze's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SniperFreezeAssetBundle, "SniperPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SniperFreezeAssetBundle, "SniperPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Sniper Freeze's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SniperPea, SniperFreeze>(827, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1109, 10),
                        new ValueTuple<int, int>(10, 1109),
                    }, 1.5f, 0f, 300, 300, 50f, 750);
                    CustomCore.AddPlantAlmanacStrings(827, "Cryo Sniper", "(@randomperson) Freeze zombies on shot!\n\n<color=#3D1400>Damage: </color><color=#8B0000>300/1.5 Seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Each shot freezes the zombie.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sniper-Pea + Ice-Shroom</color>");
                }
            }

            if (DecaySniperAssetBundle == null)
            {
                MelonLogger.Msg("Missing Sniper Decay's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(DecaySniperAssetBundle, "SniperPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(DecaySniperAssetBundle, "SniperPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Sniper Decay's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SniperPea, DecaySniper>(828, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1109, 29),
                        new ValueTuple<int, int>(29, 1109),
                    }, 1.5f, 0f, 300, 300, 50f, 750);
                    CustomCore.AddPlantAlmanacStrings(828, "Decay Pea", "(@amrailed) Poisons zombies on shot!\n\n<color=#3D1400>Damage: </color><color=#8B0000>300/1.5 Seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Each shot adds 4-6 poison points.\n<color=black>•</color> Deals higher damage the more Poison Points the target has (300+(80*PP's)).\n<color=black>•</color> Each shot makes the zombies switch lanes.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sniper-Pea + Garlic</color>");
                }
            }

            if (FrenzergAssetBundle == null)
            {
                MelonLogger.Msg("Missing Frenzerg Shroom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(FrenzergAssetBundle, "FrenzergPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(FrenzergAssetBundle, "FrenzergPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Frenzerg Shroom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<ScaredyDoom, Frenzerg>(829, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1046, 9),
                        new ValueTuple<int, int>(9, 1046),
                    }, 1.5f, 0f, 20, 300, 50f, 750);
                    CustomCore.AddPlantAlmanacStrings(829, "Frenzerg-Shroom", "(@amrailed) Shoots Doomicicles that goes faster with each shot!\n\n<color=#3D1400>Damage: </color><color=#8B0000>20/1.5 Seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Subspecies of Doomberg-Shroom use Scaredy-Shroom and Fume-Shroom to switch between the two.\n<color=black>•</color> Each shot accumulates 10 cryo points and decrease attack speed by 0.1 seconds upto 0.25 seconds MIN.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Doomberg-Shroom + Scaredy-Shroom</color>");
                    CustomCore.AddFusion(1046, 829, 7);
                }
            }

            /*if ( == null)
            {
                MelonLogger.Msg("Missing Charmer Magnet's Bundle");
            }
            else
            {
                GameObject prefabObj = null;
                GameObject previewObj = null;
                foreach (UnityEngine.Object obj in ulthypnetBundleRequest.assetBundle.LoadAllAssets())
                {
                    GameObject testObj = obj.TryCast<GameObject>();
                    if (testObj != null)
                    {
                        if (testObj.name == "HypnoMagnetPrefab")
                        {
                            MelonLogger.Msg("Successfully loaded Charmer Magnet's Prefab");
                            prefabObj = testObj.Cast<GameObject>();
                            PlantType pType = (PlantType)261;
                            prefabObj.AddComponent<Shooter>().thePlantType = pType;
                        }
                        else if (testObj.name == "HypnoMagnetPreview")
                        {
                            MelonLogger.Msg("Successfully loaded Charmer Magnet's Preview");
                            previewObj = testObj.Cast<GameObject>();
                        }
                    }
                }

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Charmer Magnet's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, UltimateHypnoMagnet>(261, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1157, 900),
                        new ValueTuple<int, int>(900, 1157),
                    }, 3f, 0f, 0, 300, 50f, 750);
                    CustomCore.AddPlantAlmanacStrings(261, "Charmer-Magnet", "(@amrailed) Spawns Ultimate zombies!\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Use metal objects to spawn zombies related to the object.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Doomberg-Shroom + Scaredy-Shroom</color>");
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Jackbox, summonJack);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Helmet, summonStriker);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Machine, summonBowling);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.SuperMachine, summonSpider);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Door, summonGramps);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Bucket, summonGramps);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Ladder, summonHypnodancer);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Pickaxe, summonHypnodancer);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.Jumper, summonTrident);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.IronHead, summonAbyssal);
                    CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)261, BucketType.RedIronHead, summonAbyssal);
                }
            }*/

            if (ClusterDoomAssetBundle == null)
            {
                MelonLogger.Msg("Missing Clustroom Shroom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(ClusterDoomAssetBundle, "DoomShroomPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(ClusterDoomAssetBundle, "DoomShroomPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Clustroom Shroom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<DoomShroom, Clustroom>(830, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(11, 11),
                        new ValueTuple<int, int>(11, 11),
                    }, 1.5f, 0f, 1800, 300, 50f, 750);
                    CustomCore.AddPlantAlmanacStrings(830, "Clustroom-Shroom", "(@amrailed) Ill put you six feet under.\n\n<color=#3D1400>Damage: </color><color=#8B0000>1800</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> A doom explosion happens in every zombie.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Doom-Shroom + Doom-Shroom</color>");
                    CustomCore.AddFusion(928, 830, 8);
                    CustomCore.AddFusion(928, 8, 830);
                }
            }

            if (SunnySniperAssetBundle == null)
            {
                MelonLogger.Msg("Missing Sunny Sniper's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SunnySniperAssetBundle, "SniperPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SunnySniperAssetBundle, "SniperPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Sunny Sniper's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SniperPea, SunnySniper>(831, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1109, 1),
                        new ValueTuple<int, int>(1, 1109),
                    }, 3f, 0f, 500, 300, 50f, 750);
                    CustomCore.AddPlantAlmanacStrings(831, "Illuminater Pea", "(@zhvachkalol) Generates sun on shot!\n\n<color=#3D1400>Damage: </color><color=#8B0000>500/3 Seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Each shot generates 25 sun when normal zombies are shot, 50 sun for gargantuars, 75 sun for odyssey zombies.\n<color=black>•</color> Deals higher damage the more sun you have, 500+sun/10 rounded down.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sniper-Pea + Sunflower</color>");
                }
            }

            if (GarlicPuffAssetBundle == null)
            {
                MelonLogger.Msg("Missing Stinky Shroom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(GarlicPuffAssetBundle, "SmallPuffPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(GarlicPuffAssetBundle, "SmallPuffPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Stinky Shroom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SmallPuff, StinkyShroom>(832, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(6, 29),
                        new ValueTuple<int, int>(29, 6),
                    }, 1.5f, 0f, 20, 300, 50f, 25);
                    CustomCore.TypeMgrExtra.IsPuff.Add((PlantType)832);
                    CustomCore.AddPlantAlmanacStrings(832, "Toxic Shroom", "(@zhvachkalol) Compact and poisonous!\n\n<color=#3D1400>Damage: </color><color=#8B0000>20+20/1.5 Seconds</color>\n\n</color><size=36>Fusion Formula: <color=#8B0000>Puff Shroom + Garlic</color>");
                }
            }

            if (IceChomperAssetBundle == null)
            {
                MelonLogger.Msg("Missing Ice Chomper's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(IceChomperAssetBundle, "ChomperPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(IceChomperAssetBundle, "ChomperPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Ice Chomper's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Chomper>(833, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(10, 5),
                        new ValueTuple<int, int>(5, 10),
                    }, 0f, 0f, 20, 300, 50f, 25);
                    CustomCore.AddPlantAlmanacStrings(833, "Ice Chomper", "(@sherif) Eat and freeze!\n\n<color=#3D1400>Damage: </color><color=#8B0000>Instant Death</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a zombie is successfully eaten it will freeze the entire screen, also chills the zombies in a 3x3 area around him.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Chomper + Ice-Shroom</color>");
                }
            }

            if (IceDoomUltimateStarAssetBundle == null)
            {
                MelonLogger.Msg("Missing Apocastar's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(IceDoomUltimateStarAssetBundle, "UltimateStarPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(IceDoomUltimateStarAssetBundle, "UltimateStarPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Apocastar's Asset Bundle");
                    CustomCore.RegisterCustomPlant<UltimateStar, Apocastar>(259, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(909, 1040),
                        new ValueTuple<int, int>(1040, 909),
                    }, 1.5f, 0f, 80, 300, 50f, 900);
                    CustomCore.AddPlantAlmanacStrings(259, "Apocastar", "(@amrailed) Insane destructive capabilities!\n\n<color=#3D1400>Damage: </color><color=#8B0000>30*5/1.5 seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Generates a Apocalyptic meteor with every shot.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Magnetar + Apocalyshroom</color>");
                }
            }

            if (IceFlowerAssetBundle == null)
            {
                MelonLogger.Msg("Missing Ice Flower's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(IceFlowerAssetBundle, "SunflowerPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(IceFlowerAssetBundle, "SunflowerPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Ice Flower's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Producer, IceFlower>(801, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1, 10),
                        new ValueTuple<int, int>(10, 1),
                    }, 25f, 25f, 20, 300, 50f, 125);
                    CustomCore.AddPlantAlmanacStrings(801, "Snowflower", "(@amrailed) Freezes the screen multiple times!\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a sunny bullet passes through it consumes it and turns it into 1 point, when it gains 20 points generate an Ice-Shroom card. Passively gains 1 point every second\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sunflower + Ice-Shroom</color>");
                }
            }

            if (FireFlowerAssetBundle == null)
            {
                MelonLogger.Msg("Missing Fire Flower's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(FireFlowerAssetBundle, "SunflowerPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(FireFlowerAssetBundle, "SunflowerPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Fire Flower's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Producer, FireFlower>(802, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1, 16),
                        new ValueTuple<int, int>(16, 1),
                    }, 25f, 25f, 20, 300, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(802, "Blazeflower", "(@amrailed) Burns the screen multiple times!\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a sunny bullet passes through it consumes it and turns it into 1 point, when it gains 20 points generate a Jalapeno card. Passively gains 1 point every second\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sunflower + Jalapeno</color>");
                }
            }

            if (RichSunflowerAssetBundle == null)
            {
                MelonLogger.Msg("Missing Rich Sunflower's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(RichSunflowerAssetBundle, "GoldSunflowerPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(RichSunflowerAssetBundle, "GoldSunflowerPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Rich Sunflower's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Producer, RichSunflower>(262, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1188, 1188),
                    }, 25f, 5f, 20, 300, 50f, 300);
                    CustomCore.AddPlantAlmanacStrings(262, "Rich Sunflower", "(@amrailed) Consumes sun projectile and turns them to odyssey plants!\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a sunny bullet passes through it consumes it and turns it into 1 point, also consumes suncicles for 3 points, when it gains 100 points generate a random odyssey plant infront of her, if the grid is already occupied then attempt to fuse with the plant infront, if it cant then it will go to the next square and try again.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Golden-Sunflower + Golden-Sunflower</color>");
                }
            }

            if (JalaChomperAssetBundle == null)
            {
                MelonLogger.Msg("Missing Fire Chomper's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(JalaChomperAssetBundle, "ChomperPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(JalaChomperAssetBundle, "ChomperPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Fire Chomper's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Chomper>(834, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(16, 5),
                        new ValueTuple<int, int>(5, 16),
                    }, 0f, 0f, 20, 300, 50f, 275);
                    CustomCore.AddPlantAlmanacStrings(834, "Fire Chomper", "(@sherif) Eat and Scorch an entire lane!\n\n<color=#3D1400>Damage: </color><color=#8B0000>Instant Death+1800</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a zombie is successfully eaten it will cause the destruction of a lane, it also give the enflamed status effect to zombies in a 3x3 area around him.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Chomper + Jalapeno</color>");
                }
            }

            if (ObsidianChomperAssetBundle == null)
            {
                MelonLogger.Msg("Missing Obsidian Chomper's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(ObsidianChomperAssetBundle, "ChomperPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(ObsidianChomperAssetBundle, "ChomperPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Obsidian Chomper's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Chomper, ObsidianChomper>(263, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(833, 834),
                        new ValueTuple<int, int>(834, 833),
                    }, 0f, 0f, 800, 8000, 50f, 275);
                    CustomCore.TypeMgrExtra.IsFirePlant.Add((PlantType)263);
                    CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)263);
                    CustomCore.AddPlantAlmanacStrings(263, "Obsidian Maw", "(@sherif) Crush zombies with its immense teeth!\n\n<color=#3D1400>Damage: </color><color=#8B0000>Instant Death+1800+600</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a zombie is successfully eaten it will cause the destruction of a lane, freezes the screen, unlike regular chompers does not have any cooldown, Also shoots obsidian pelts that deals 600 damage based on how many zombies there are in a 3x3 area.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Fire Chomper + Ice Chomper</color>");
                }
            }

            if (FumeUmbrellaAssetBundle == null)
            {
                MelonLogger.Msg("Missing Fume Umbrella's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(FumeUmbrellaAssetBundle, "UmbrellaleafPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(FumeUmbrellaAssetBundle, "UmbrellaleafPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Fume Umbrella's Asset Bundle");
                    CustomCore.RegisterCustomPlant<CabbageUmbrella, FumeUmbrella>(803, prefabObj, previewObj, new List<(int, int)>
                    {
                        (30, 7), (7, 30),
                    }, 0f, 0f, 800, 1000, 7.5f, 200);
                    CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)803);
                    CustomCore.AddPlantAlmanacStrings(803, "Fumebrella", "(@talpido) Prevents zombies from progressing further!\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Pushes zombies that encounter it.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Umbrella-Leaf + Fume-Shroom</color>");
                }
            }

            if (FrostUmbrellaAssetBundle == null)
            {
                MelonLogger.Msg("Missing Frost Umbrella's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(FrostUmbrellaAssetBundle, "UmbrellaleafPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(FrostUmbrellaAssetBundle, "UmbrellaleafPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Frost Umbrella's Asset Bundle");
                    CustomCore.RegisterCustomPlant<CabbageUmbrella, FrostUmbrella>(807, prefabObj, previewObj, new List<(int, int)>
                    {
                        (803, 10), (10, 803),
                    }, 0f, 0f, 800, 1000, 7.5f, 200);
                    CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)807);
                    CustomCore.AddPlantAlmanacStrings(807, "Frosbrella", "(@talpido) Freeze!\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> Pushes and slows zombies that encounter it, Has a 10% chance to freeze a zombie.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Fumebrella + Ice-Shroom</color>");
                }
            }

            if (DoomFlowerAssetBundle == null)
            {
                MelonLogger.Msg("Missing Doom Flower's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(DoomFlowerAssetBundle, "SunflowerPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(DoomFlowerAssetBundle, "SunflowerPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Doom Flower's Asset Bundle");
                    CustomCore.RegisterCustomPlant<PeaSunFlower, DoomFlower>(804, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1, 11),
                        new ValueTuple<int, int>(11, 1),
                    }, 25f, 25f, 20, 300, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(804, "Doomflower", "(@amrailed) Generates multiple doom shrooms!\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color> When a sunny bullet passes through it consumes it and turns it into 1 point, when it gains 20 points generate a Doom-Shroom infront of her, if the grid is already occupied then attempt to fuse with the plant infront, if it cant does not do anything.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sunflower + Doom-Shroom</color>");
                }
            }

            if (SunnyGatlingAssetBundle == null)
            {
                MelonLogger.Msg("Missing Sunny Gatling's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SunnyGatlingAssetBundle, "GatlingPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SunnyGatlingAssetBundle, "GatlingPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Sunny Gatling's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, SunnyGatling>(805, prefabObj, previewObj, new List<(int, int)>
                    {
                        new ValueTuple<int, int>(1, 1032),
                        new ValueTuple<int, int>(1032, 1),
                    }, 5f, 25f, 100, 300, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(805, "Sunny Gatling", "(@iamnoobieboy) Shoots quadruple the sun!\n\n<color=#3D1400>Damage: </color><color=#8B0000>100*4/5 seconds</color>\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sunflower + Gatling-Pea</color>");
                }
            }

            if (SunnyCommandoAssetBundle == null)
            {
                MelonLogger.Msg("Missing Sunny Commando's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SunnyCommandoAssetBundle, "SuperGatlingPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SunnyCommandoAssetBundle, "SuperGatlingPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Sunny Commando's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SuperSnowGatling, SunnyCommando>(806, prefabObj, previewObj, new List<(int, int)>
                    {
                        (1, 1168), (1168, 1), (1000, 1090), (1090, 1000)
                    }, 5f, 25f, 100, 300, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(806, "Sunny Commando", "(@iamnoobieboy)Shoots sun at a fast rate!\n\n<color=#3D1400>Damage: </color><color=#8B0000>100*6/5 seconds</color>\n\n</color><size=36>Fusion Formula: <color=#8B0000>Sunflower + Commando-Pea</color>");
                }
            }

            /*if (melonadeMortarBundleRequest == null)
            {
                MelonLogger.Msg("Missing Melonade Mortar's Bundle");
            }
            else
            {
                GameObject prefabObj = null;
                GameObject previewObj = null;
                foreach (UnityEngine.Object obj in melonadeMortarBundleRequest.assetBundle.LoadAllAssets())
                {
                    GameObject testObj = obj.TryCast<GameObject>();
                    if (testObj != null)
                    {
                        if (testObj.name == "MelonadeMortarPrefab")
                        {
                            MelonLogger.Msg("Successfully loaded Melonade Mortar's Prefab");
                            prefabObj = testObj.Cast<GameObject>();
                            PlantType pType = (PlantType)264;
                            prefabObj.AddComponent<Thrower>().thePlantType = pType;
                        }
                        else if (testObj.name == "MelonadeMortarPreview")
                        {
                            MelonLogger.Msg("Successfully loaded Melonade Mortar's Preview");
                            previewObj = testObj.Cast<GameObject>();
                        }
                    }
                }

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Melonade Mortar's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Thrower, MelonadeMortar>(264, prefabObj, previewObj, new List<(int, int)>
                    {
                        (1142, 1106), (1106, 1142)
                    }, 1f, 0f, 40, 300, 50f, 850);
                    CustomCore.AddPlantAlmanacStrings(264, "Melonade Mortar", "(@mrboomglass) Shoots freezing golden melons!\n\n<color=#3D1400>Damage: </color><color=#8B0000>40*4/1 seconds + 140*4/3 seconds</color>\n\n<color=black>Special: </color><color=#8B0000>\n<color=black>•</color>Shoots Gold-Melons and Polar Ice Peas. Gold-Melons shot by her slowdown by 8 seconds and injects 10 Cryo-Points, it also drops small silver coins which give out 25 coins.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Golden-Melon + Snow Gatling</color>");
                    MelonadeMortar.buff1 = CustomCore.RegisterCustomBuff("Fast Reload : Horizontal and vertical attacks have 0.5 second Interval instead of 3 and 1 respectively", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)264);
                    MelonadeMortar.buff2 = CustomCore.RegisterCustomBuff("Below Zero : Melonade Mortar's Ult immedietely kills zombies with less then 1200 health", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)264);
                    CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)264);
                    CustomCore.RegisterSuperSkill(264, (_) => 4000, (plant) =>
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            var RowFromY = Mouse.Instance.GetRowFromY(1, 1);
                            UnityEngine.Vector2 shoot = plant.transform.Find("Cannon").Find("Shoot").transform.position;
                            var bullet = plant.board.GetComponent<CreateBullet>().SetBullet(shoot.x, shoot.y, RowFromY, BulletType.Bullet_goldMelonCannon, 14);
                            var pos2 = bullet.cannonPos;
                            pos2.x = 1;
                            pos2.y = 1;
                            bullet.cannonPos = pos2;
                            bullet.rb.velocity = new(1.5f, 0);
                            bullet.theStatus = BulletStatus.GoldMelon_cannon;
                            bullet.Damage = 240;
                        }
                    });
                }
            }*/

            if (MarigoldBombAssetBundle == null)
            {
                MelonLogger.Msg("Missing Marigold Bomb's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(MarigoldBombAssetBundle, "SunBombPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(MarigoldBombAssetBundle, "SunBombPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Marigold Bomb's Asset Bundle");
                    CustomCore.RegisterCustomPlant<CherryBomb>(809, prefabObj, previewObj, new List<(int, int)>
                    {
                        (2, 31), (31, 2)
                    }, 0f, 0f, 1800, 300, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(809, "Marigold Bomb", "(@lotus)Explodes and generates coins.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Marigold + Cherry-Bomb</color>");
                }
            }
            if (ExplodoNutAssetBundle == null)
            {
                MelonLogger.Msg("Missing Explodo Nut's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(ExplodoNutAssetBundle, "SuperSunNutPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(ExplodoNutAssetBundle, "SuperSunNutPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Explodo Nut's Asset Bundle");
                    CustomCore.RegisterCustomPlant<WallNut>(808, prefabObj, previewObj, new List<(int, int)>
                    {
                        (3, 1005), (1005, 3)
                    }, 0f, 0f, 120, 4000, 50f, 400);
                    CustomCore.AddFusion(1005, 808, 0);
                    CustomCore.AddFusion(1005, 0, 808);
                    CustomCore.TypeMgrExtra.IsNut.Add((PlantType)808);
                    CustomCore.AddPlantAlmanacStrings(808, "Explodo Nut", "(@amrailed) Blasts zombies that eats them.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Explodo-shooter + Wall-Nut</color>");
                }
            }
            if (TwinTycoonShooterAssetBundle == null)
            {
                MelonLogger.Msg("Missing Twin Tycoon Shooter's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(TwinTycoonShooterAssetBundle, "PeaSunFlowerPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(TwinTycoonShooterAssetBundle, "PeaSunFlowerPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Twin Tycoon Shooter's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, TwinTycoonShooter>(810, prefabObj, previewObj, new List<(int, int)>
                    {
                        (1150, 0), (0, 1150)
                    }, 1.5f, 0f, 40, 300, 50f, 225);
                    CustomCore.AddPlantAlmanacStrings(810, "Twin Tycoon Shooter", "(@iamnoobieboy) Shoots deal more damage with more coins the user has.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Gold-Magnet + Peashooter</color>");
                }
            }
            if (GarlicBoomAssetBundle == null)
            {
                MelonLogger.Msg("Missing Garlic Boom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(GarlicBoomAssetBundle, "CherryBombPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(GarlicBoomAssetBundle, "CherryBombPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Garlic Boom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<CherryBomb>(811, prefabObj, previewObj, new List<(int, int)>
                    {
                        (2, 29), (29, 2)
                    }, 0f, 0f, 1800, 300, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(811, "Garlic Boom", "(@lotus) Explodes and poisons zombie.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Garlic + Cherry-Bomb</color>");
                }
            }
            if (SilverGatlingAssetBundle == null)
            {
                MelonLogger.Msg("Missing Silver Gatling's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SilverGatlingAssetBundle, "GatlingPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SilverGatlingAssetBundle, "GatlingPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Silver Gatling's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, SilverGatling>(812, prefabObj, previewObj, new List<(int, int)>
                    {
                        (1032, 31), (31, 1032)
                    }, 1.5f, 0f, 40, 300, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(812, "Silver Gatling", "(@amrailed) Shoots higher damaging Silver Coins.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Gatling Pea + Marigold</color>");
                }
            }
            if (GoldenGatlingAssetBundle == null)
            {
                MelonLogger.Msg("Missing Golden Gatling's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(GoldenGatlingAssetBundle, "GatlingPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(GoldenGatlingAssetBundle, "GatlingPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Golden Gatling's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, GoldenGatling>(813, prefabObj, previewObj, new List<(int, int)>
                    { }, 1.5f, 0f, 120, 300, 50f, 175);
                    CustomCore.RegisterSuperSkill(812, (_) => 3000, (plant) =>
                    {
                        plant.Die();
                        CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)813);
                    });
                    CustomCore.RegisterSuperSkill(813, (_) => 6000, (plant) =>
                    {
                        if (plant.gameObject.TryGetComponent<GoldenGatling>(out var p))
                        {
                            MelonCoroutines.Start(p.SuperSkill());
                        }
                    });
                    CustomCore.AddPlantAlmanacStrings(813, "Golden Gatling", "(@amrailed) Shoots even higher damaging Golden Coins, And its ultimate is able to shoot a barrage of golden coins.\n\n</color><size=36>Fusion Formula: <color=#8B0000>Gatling Pea + Gold-Bean</color>");
                }
            }
            if (GoldenTycoonGatlingAssetBundle == null)
            {
                MelonLogger.Msg("Missing Golden Tycoon Gatling's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(GoldenTycoonGatlingAssetBundle, "GatlingPeaPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(GoldenTycoonGatlingAssetBundle, "GatlingPeaPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Golden Tycoon Gatling's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, GoldenTycoonGatling>(265, prefabObj, previewObj, new List<(int, int)>
                    { (813, 810), (810, 813) }, 1.5f, 0f, 500, 300, 50f, 175);
                    CustomCore.RegisterSuperSkill(265, (_) => 6000, (plant) =>
                    {
                        if (plant.gameObject.TryGetComponent<GoldenTycoonGatling>(out var p))
                        {
                            MelonCoroutines.Start(p.SuperSkill());
                        }
                    });
                    CustomCore.AddUltimatePlant((PlantType)265);
                    GoldenTycoonGatling.buff1 = CustomCore.RegisterCustomBuff("Richer, yet richer : Each 100 sun increaes damage by 1", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)265);
                    GoldenTycoonGatling.buff2 = CustomCore.RegisterCustomBuff("Active Tycoon : Bullets produced by Golden Tycoon Gatling now generates Coins (and Sun if \"Richer, yet richer\" is unlocked)", BuffType.AdvancedBuff, () => (true), 5000, default, (PlantType)265);
                    CustomCore.AddPlantAlmanacStrings(265, "Golden Tycoon Gatling", "(@amrailed) Shoots gold coins that are deal immense damage.\n\n<color=#3D1400>Damage: </color><color=#8B0000>(500+(Coins/100))*4/1.5 seconds</color>\n\n<color=black>Ultimate (6000 coins) : Shoots a barrage of 100 coins every 0.005 seconds in 3 different directions</color><color=#8B0000>\n\n<color=black>Odyssey Modifier's: </color><color=#8B0000>\n<color=black>•</color> Richer, yet Richer : Each 100 sun increases damage by 1.\n<color=black>•</color> Active Tycoon : Bullets produced by Golden Tycoon Gatling now generates Coins (and Sun if \"Richer, yet richer\" is unlocked).\n\n</color><size=36>Fusion Formula: <color=#8B0000>Golden Gatling + Twin Tycoon Shooter</color>");
                }
            }
            if (ObsidianSeedAssetBundle == null)
            {
                MelonLogger.Msg("Missing Obsidian Seed's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(ObsidianSeedAssetBundle, "WallNutPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(ObsidianSeedAssetBundle, "WallNutPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Obsidian Seed's Asset Bundle");
                    CustomCore.RegisterCustomPlant<WallNut, ObsidianSeed>(266, prefabObj, previewObj, new List<(int, int)>
                    { }, 1.5f, 0f, 500, 2000, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(266, "Obsidian Seed", "(@amrailed) Grows to an Obsidian Melon.\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Grows into an Obsidian Melon in 90 seconds (30 if Obsidian Melon's buff is unlocked).\n\n</color><size=36>Source : <color=#8B0000>Obsidian Melon</color>");
                }
            }
            /*if (laserIceUmbrellaBundleRequest == null)
            {
                MelonLogger.Msg("Missing Cryohelix Umbrella's Bundle");
            }
            else
            {
                GameObject prefabObj = null;
                GameObject previewObj = null;
                foreach (UnityEngine.Object obj in laserIceUmbrellaBundleRequest.assetBundle.LoadAllAssets())
                {
                    GameObject testObj = obj.TryCast<GameObject>();
                    if (testObj != null)
                    {
                        if (testObj.name == "LaserUmbrellaPrefab")
                        {
                            MelonLogger.Msg("Successfully loaded Cryohelix Umbrella's Prefab");
                            prefabObj = testObj.Cast<GameObject>();
                            PlantType pType = (PlantType)267;
                            prefabObj.AddComponent<LaserUmbrella>().thePlantType = pType;
                        }
                        else if (testObj.name == "LaserUmbrellaPreview")
                        {
                            MelonLogger.Msg("Successfully loaded Cryohelix Umbrella's Preview");
                            previewObj = testObj.Cast<GameObject>();
                        }
                        else if (testObj.name == "LightBall")
                        {
                            MelonLogger.Msg("Successfully loaded Cryohelix Umbrella's Light Ball");
                            LaserIceUmbrellalightBall = testObj.Cast<GameObject>();
                        }
                        else if (testObj.name == "theLight")
                        {
                            MelonLogger.Msg("Successfully loaded Cryohelix Umbrella's Light Beam");
                            LaserIceUmbrellatheLight = testObj.Cast<GameObject>();
                        }
                    }
                }

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Cryohelix Umbrella's Asset Bundle");
                    CustomCore.RegisterCustomPlant <LaserUmbrella, LaserIceUmbrella>(267, prefabObj, previewObj, new List<(int, int)>
                    { (937, 10), (10, 937) }, 1.5f, 0f, 300, 1000, 50f, 175);
                    CustomCore.AddPlantAlmanacStrings(267, "Cryohelix Umbrella", "(@loifnt) Freezes zombies from across the lawn.\n\n<color=#3D1400>Damage: </color><color=#8B0000>300</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> You are my special.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Laser Umbrella + Ice-Shroom</color>");
             */
            if (SuperGatlingMineAssetBundle == null)
            {
                MelonLogger.Msg("Missing Super Gatling Mine's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SuperGatlingMineAssetBundle, "PeaMinePrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SuperGatlingMineAssetBundle, "PeaMinePreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Super Gatling Mine's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SuperSnowGatling, SuperGatlingMine>(814, prefabObj, previewObj, new List<(int, int)>
                    { (1168, 4), (4, 1168) }, 1.5f, 0f, 60, 300, 50f, 625);
                    CustomCore.AddPlantAlmanacStrings(814, "Comminedo-Mine", "(@oildeone) Shoots potatoes that deal high damage.\n\n<color=#3D1400>Damage: </color><color=#8B0000>60*6/1.5 seconds</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Has a 3% chance to shoot a barrage.\n<color=black>•</color> Can revive itslef after explosion.\n<color=black>•</color> Shoots faster the closer zombies are up to 0.1 seconds at max.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Pea-Commando + Potato Mine</color>");
                }
            }
            if (SummerCabbageAssetBundle == null)
            {
                MelonLogger.Msg("Missing Jala Cabbage's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(SummerCabbageAssetBundle, "CabbagepultPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(SummerCabbageAssetBundle, "CabbagepultPreview");
                //GameObject bulletObj = ResourceLoader.GetResourceFromBundle(SummerCabbageAssetBundle, "PeaMinePreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Jala Cabbage's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Cabbage>(815, prefabObj, previewObj, new List<(int, int)>
                    { (16, 26), (26, 16) }, 1.5f, 0f, 60, 300, 50f, 225);
                    CustomCore.AddPlantAlmanacStrings(815, "Summer Cabbage", "(@lotus) Shoots flaming cabbages.\n\n<color=#3D1400>Damage: </color><color=#8B0000>60/1.5 seconds</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Attacks inflict enflamed effect.\n<color=black>•</color> If it hits an enflamed zombie, it will generate a 3x3 explosion that deals 20 damage.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Cabbage-Pult + Jalapeno</color>");
                }
            }
            /*if (bladeStarBundleRequest == null)
            {
                MelonLogger.Msg("Missing Blade Star's Bundle");
            }
            else
            {
                GameObject prefabObj = null;
                GameObject previewObj = null;
                GameObject bulletObj = null;
                foreach (UnityEngine.Object obj in bladeStarBundleRequest.assetBundle.LoadAllAssets())
                {
                    GameObject testObj = obj.TryCast<GameObject>();
                    if (testObj != null)
                    {
                        if (testObj.name == "SwordStarPrefab")
                        {
                            MelonLogger.Msg("Successfully loaded Blade Star's Prefab");
                            prefabObj = testObj.Cast<GameObject>();
                            PlantType pType = (PlantType)816;
                            prefabObj.AddComponent<SwordStarfruit>().thePlantType = pType;
                        }
                        else if (testObj.name == "SwordStarPreview")
                        {
                            MelonLogger.Msg("Successfully loaded Blade Star's Preview");
                            previewObj = testObj.Cast<GameObject>();
                        }
                    }
                }

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Blade Star's Asset Bundle");
                    CustomCore.RegisterCustomPlant<SwordStarfruit, BladeStar>(816, prefabObj, previewObj, new List<(int, int)>
                    { (33, 249), (249, 33) }, 1.5f, 0f, 60, 300, 50f, 225);
                    CustomCore.AddPlantAlmanacStrings(816, "Blade Star", "(@artemplayingpvz) Shoots out flying blades that deal heavy damage.\n\n<color=#3D1400>Damage: </color><color=#8B0000>60/1.5 seconds</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Attacks inflict enflamed effect.\n<color=black>•</color> If it hits an enflamed zombie, it will generate a 3x3 explosion that deals 20 damage.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Cabbage-Pult + Jalapeno</color>");
                }
            }*/
            if (MagmaShroomAssetBundle == null)
            {
                MelonLogger.Msg("Missing Magma Shroom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(MagmaShroomAssetBundle, "FireFumePrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(MagmaShroomAssetBundle, "FireFumePreview");
                //GameObject bulletObj = null;

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Magma Shroom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Shooter, MagmaShroom>(817, prefabObj, previewObj, new List<(int, int)>
                    { (11, 1193), (1193, 11), (16, 1043), (1043, 16) }, 1.5f, 0f, 60, 300, 50f, 325);
                    CustomCore.AddPlantAlmanacStrings(817, "Magma Shroom", "(@amrailed) Shoots magma that scorches zombie.\n\n<color=#3D1400>Damage: </color><color=#8B0000>60*4/1.5 seconds</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Attacks inflict the enflamed and ember effect in a 1x1 area.\n<color=black>•</color> If it hits an enflamed zombie, it will penetrate the zombie and switch lanes.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Fume-Shroom + Doom-Shroom + Jalapeno</color>");
                }
            }
            if (JalaHypnoAssetBundle == null)
            {
                MelonLogger.Msg("Missing Jala Hypno's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(JalaHypnoAssetBundle, "HypnoShroomPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(JalaHypnoAssetBundle, "HypnoShroomPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Jala Hypno's Asset Bundle");
                    CustomCore.RegisterCustomPlant<Plant>(818, prefabObj, previewObj, new List<(int, int)>
                    { (16, 8), (8, 16)}, 1.5f, 0f, 60, 300, 50f, 325);
                    CustomCore.AddPlantAlmanacStrings(818, "Scorchip Shroom", "(@amrailed) Converts zombies that eat her into a hypnontized Jalapeno Zombie.\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Turns zombies that eat it into a Hypnotized Jalapeno Zombie.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Hypno-Shroom + Jalapeno</color>");
                }
            }
            if (UltimateJalaDoomFumeAssetBundle == null)
            {
                MelonLogger.Msg("Missing Volcano Shroom's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(UltimateJalaDoomFumeAssetBundle, "FireFumePrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(UltimateJalaDoomFumeAssetBundle, "FireFumePreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Volcano Shroom's Asset Bundle");
                    CustomCore.RegisterCustomPlant<UltimateFume, VolcanoShroom>(268, prefabObj, previewObj, new List<(int, int)>
                    { (817, 818), (818, 817)}, 0.5f, 0f, 225, 300, 30f, 525);
                    CustomCore.AddUltimatePlant((PlantType)268);
                    VolcanoShroom.buff1 = CustomCore.RegisterCustomBuff("Volcanic Ash : Every 0.5 seconds it will generate an explosion with 1/5th of the base damage with a 3x3 range it will also spread the enflamed effect.", BuffType.AdvancedBuff, () => true, 5000, default, (PlantType)268);
                    VolcanoShroom.buff2 = CustomCore.RegisterCustomBuff("Stream of Death : Base damage is increased to 125", BuffType.AdvancedBuff, () => true, 5000, default, (PlantType)268);
                    printString("Volcano Shroom Buff 1 registered as " + (VolcanoShroom.buff1).ToString());
                    printString("Volcano Shroom Buff 2 registered as " + (VolcanoShroom.buff2).ToString());
                    CustomCore.AddPlantAlmanacStrings(268, "Volcano Shroom", "(@amrailed) Shoots deadly fireballs that scorches zombie.\n\n<color=#3D1400>Damage: </color><color=#8B0000>50/0.1 seconds</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> Attacks inflict the enflamed and ember effect to every zombie in the same row as it.\n<color=black>•</color> Has a 40% chance to transfigure zombies (with < 50% Max HP) into hypnotized jalapeno zombies.\n\n<color=black>Odyssey Modifier's: </color><color=#8B0000>\n<color=black>•</color> Volcanic Ash : Every 0.5 seconds it will generate an explosion with 1/5th of the base damage with a 3x3 range it will also spread the enflamed effect.\n<color=black>•</color> Stream Of Death : Base damage is increased to 125.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Magma-Shroom + Scorchip-Shroom</color>");
                }
            }
            if (UmbrellaMineAssetBundle == null)
            {
                MelonLogger.Msg("Missing Umbrella Mine's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(UmbrellaMineAssetBundle, "UmbrellaleafPrefab");
                GameObject previewObj = ResourceLoader.GetResourceFromBundle(UmbrellaMineAssetBundle, "UmbrellaleafPreview");

                if ((prefabObj != null) && (previewObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Umbrella Mine's Asset Bundle");
                    CustomCore.RegisterCustomPlant<CabbageUmbrella, UmbrellaMine>(819, prefabObj, previewObj, new List<(int, int)>
                    { (30, 4), (4, 30)}, 0.5f, 0f, 225, 1000, 30f, 525);
                    CustomCore.TypeMgrExtra.IsPotatoMine.Add((PlantType)819);
                    CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)819);
                    CustomCore.AddPlantAlmanacStrings(819, "Umbrella Mine", "(@amrailed) Bounces and explode.\n\n<color=#3D1400>Damage: </color><color=#8B0000>1800 (Cremator)</color>\n\n<color=black>Special's: </color><color=#8B0000>\n<color=black>•</color> When armed explode lose 100 hp and gets unarmed. When not armed knockbacks zombies and deal 120 damage.\n\n</color><size=36>Fusion Formula : <color=#8B0000>Umbrella-Leaf + Potato-Mine</color>");
                }
            }

            // -- Zombies - \\

            /*if (DeathcatcherAssetBundle == null)
            {
                MelonLogger.Msg("Missing Deathcatcher Zombie's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(DeathcatcherAssetBundle, "Deathcatcher");

                if ((prefabObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Deathcatcher Zombie's Asset Bundle");
                    CustomCore.RegisterCustomZombie<Zombie, Deathcatcher>((ZombieType)256, prefabObj, 300, 0, 90000, 0, 0);
                    CustomCore.AddZombieAlmanacStrings(256, "Deathcatcher", "<size=36>Revives zombies from the dead in a weaker form.\n\n<color=black>Toughness: </color><color=#4B0082>Cannot be killed by projectiles</color>");
                }
            }*/
            if (HypnoPaperZombieAssetBundle == null)
            {
                MelonLogger.Msg("Missing Hypno Paper Zombie's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(HypnoPaperZombieAssetBundle, "ElitePaperZombie");

                if ((prefabObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Hypno Paper Zombie's Asset Bundle");
                    CustomCore.RegisterCustomSprite(300, HypnoPaperZombieAssetBundle.GetAsset<Sprite>("book1"));
                    CustomCore.RegisterCustomZombie<ElitePaperZombie, HypnoPaperZombie>((ZombieType)255, prefabObj, 300, 400, 3000, 0, 1500);
                    CustomCore.AddZombieAlmanacStrings(255, "Hypno Paper Zombie", "<size=36>Floods the area with zombies if not treated carefully.\n\n<color=black>Damage: </color><color=#4B0082>400 / 0.5 Seconds (bite)</color>\n<color=black>Toughness: </color><color=#4B0082>3000+1500 (Type II)</color>\n<color=black>Traits: </color><color=#4B0082>Spawns (Giga Buckshot Commando | Giga Rugby-Nut Zombie | Gatling Cherry Newspaper | Kirov | Michael Zomboni) every 0.75 seconds when its newspaper is broken.</color>");
                }
            }
            if (FireClawZombieAssetBundle == null)
            {
                MelonLogger.Msg("Missing Fire Claw Zombie's Bundle");
            }
            else
            {
                GameObject prefabObj = ResourceLoader.GetResourceFromBundle(FireClawZombieAssetBundle, "ElitePaperZombie");

                if ((prefabObj != null))
                {
                    MelonLogger.Msg("Successfully Loaded Fire Claw Zombie's Asset Bundle");
                    CustomCore.RegisterCustomZombie<Zombie, FireClawZombie>((ZombieType)254, prefabObj, 300, 1000, 1500, 8000, 0);
                    CustomCore.AddZombieAlmanacStrings(254, "Scorch Tall-Nut Sentinel Zombie", "<size=36>Deals ten times damage to plants.\n\n<color=black>Damage: </color><color=#4B0082>1000 / 0.5 Seconds (bite)</color>\n<color=black>Toughness: </color><color=#4B0082>1500+8000 (Type I)</color>");
                }
            }
            CustomLevelData BossRushLevelData = new CustomLevelData();
            BossRushLevelData.BgmType = MusicType.Boss;
            BossRushLevelData.SceneType = SceneType.Day;
            BossRushLevelData.AdvBuffs = () =>
            {
                var list = new List<int>();
                list.Add(16);
                list.Add(13);
                list.Add(27);
                list.Add(32);
                return list;
            };
            BossRushLevelData.ConveyBeltPlantTypes = () =>
            {
                var list = new List<PlantType>();
                return list;
            };
            BossRushLevelData.ZombieList = () =>
            {
                List<ZombieType> ZombieList = new List<ZombieType>();
                ZombieList.Add(ZombieType.SnowShieldZombie);
                ZombieList.Add(ZombieType.SnowGunZombie);
                ZombieList.Add(ZombieType.SnowDrownZombie);

                ZombieList.Add(ZombieType.FootballZombie);
                ZombieList.Add(ZombieType.TallNutFootballZombie);
                ZombieList.Add(ZombieType.FlagFootball);
                ZombieList.Add(ZombieType.FootballDolphin);
                ZombieList.Add(ZombieType.GatlingFootballZombie);
                return ZombieList;
            };
            //BossRushLevelData.ConveyBeltPlantTypes = () => (Plugin.BossRushOdysseyPlantList);
            BossRushLevelData.WaveCount = () => (40);
            BossRushLevelData.Logo = BossRushLogo;
            BossRushLevelData.Name = () => ("Boss Rush");
            BossRushLevelData.RowCount = 5;
            BossRushLevelData.NeedSelectCard = false;

            var bTagBR = default(Board.BoardTag);
            bTagBR.enableAllTravelPlant = true;
            bTagBR.enableTravelPlant = true;
            bTagBR.isFreeCardSelect = false;
            bTagBR.disableNormalSun = true;
            bTagBR.disableMower = true;
            bTagBR.isConvey = true;
            BossRushLevelData.BoardTag = bTagBR;
            Plugin.BossRushLevelID = CustomCore.RegisterCustomLevel(BossRushLevelData);
        }
    }

    [HarmonyPatch(typeof(Chomper))]
    public static class ChomperPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Chomp")]
        public static void ChompFix(Chomper __instance, Zombie zombie)
        {
            if (__instance.thePlantType == (PlantType)833)
            {
                Board.Instance.CreateFreeze(__instance.transform.position);
            }
            if (__instance.thePlantType == (PlantType)834)
            {
                Board.Instance.CreateFireLine(__instance.thePlantRow);
            }
            if (__instance.thePlantType == (PlantType)263)
            {
                Board.Instance.CreateFreeze(__instance.transform.position);
                Board.Instance.CreateFireLine(__instance.thePlantRow);
                __instance.swallowMaxCountDown = 0.1f;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("BiteEvent")]
        public static void BiteEvent(Chomper __instance)
        {
            var pos = __instance.transform.position;
            var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + 1.1f), 2f);
            foreach (var z in array)
            {
                if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie))
                {
                    if (__instance.thePlantType == (PlantType)833)
                    {
                        zombie.SetCold(6);
                    }
                    if (__instance.thePlantType == (PlantType)834)
                    {
                        zombie.SetJalaed();
                    }
                    if (__instance.thePlantType == (PlantType)263)
                    {
                        UnityEngine.Vector3 position = __instance.transform.position;
                        Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(position.x + 0.1F, position.y + 1.1f, __instance.thePlantRow, BulletType.Bullet_steelPea, 0);

                        bullet.Damage = 600;
                        bullet.normalSpeed = (new System.Random()).Next(10, 15);
                        bullet.theBulletRow = __instance.thePlantRow;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SuperSnowGatling))]
        public class SuperSnowGatlingPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("GetBulletType")]
            public static void PostGetBulletType(SuperSnowGatling __instance, ref BulletType __result)
            {
                if (__instance.thePlantType == (PlantType)806)
                {
                    __result = BulletType.Bullet_smallSun;
                }
                else if (__instance.thePlantType == (PlantType)814)
                {
                    __result = BulletType.Bullet_puffPotato;
                }
            }
            [HarmonyPrefix]
            [HarmonyPatch("SuperShoot")]
            public static bool PreSuperShoot(SuperSnowGatling __instance, ref float angle, ref float speed, ref float x, ref float y)
            {
                if (__instance.thePlantType == (PlantType)806)
                {
                    var b = CreateBullet.Instance.SetBullet(x, y, __instance.thePlantRow, BulletType.Bullet_smallSun, 15);
                    b.transform.Rotate(0, 0, angle);
                    b.normalSpeed = speed;
                    return false;
                }
                if (__instance.thePlantType == (PlantType)814)
                {
                    var b = CreateBullet.Instance.SetBullet(x, y, __instance.thePlantRow, BulletType.Bullet_puffPotato, 15);
                    b.transform.Rotate(0, 0, angle);
                    b.normalSpeed = speed;
                    return false;
                }
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(WallNut), "TakeDamage")]
    public static class WallNutPatch
    {
        public static void Prefix(WallNut __instance, int damage, int damageType)
        {
            if (__instance.thePlantType == (PlantType)808)
            {
                UnityEngine.Vector3 pos = __instance.transform.position + new UnityEngine.Vector3(0.8f, 1.1f);
                Bullet bullet = Board.Instance.GetComponent<CreateBullet>().SetBullet(pos.x, pos.y, __instance.thePlantRow, BulletType.Bullet_superCherry, 0);
                bullet.Damage = (int)Il2CppSystem.Math.Floor(((float)damage) / 2);
            }
        }
    }

    [HarmonyPatch(typeof(CherryBomb))]
    public static class CherryBombPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Bomb")]
        public static void PreBomb(CherryBomb __instance)
        {
            if (__instance.thePlantType == (PlantType)809)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + .8f), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && (zombie.theZombieRow == __instance.thePlantRow || zombie.theZombieRow == __instance.thePlantRow + 1 || zombie.theZombieRow == __instance.thePlantRow - 1))
                    {
                        zombie.TakeDamage(DmgType.NormalAll, (int)(Board.Instance.theTotalNumOfCoin / 10));
                        if ((new System.Random()).Next(1, 4) <= 1)
                        {
                            CreateItem.Instance.SetCoin(Mouse.Instance.GetColumnFromX(zombie.transform.position.x), zombie.theZombieRow, 39, 0);

                        }
                        else
                        {
                            CreateItem.Instance.SetCoin(Mouse.Instance.GetColumnFromX(zombie.transform.position.x), zombie.theZombieRow, 38, 0);
                        }
                    }
                }
            }
            else if (__instance.thePlantType == (PlantType)811)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + .8f), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && (zombie.theZombieRow == __instance.thePlantRow || zombie.theZombieRow == __instance.thePlantRow + 1 || zombie.theZombieRow == __instance.thePlantRow - 1))
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            zombie.AddPoisonLevel();
                        }
                        zombie.Garliced();
                    }
                }
            }
        }
    }
    /*[HarmonyPatch(typeof(LaserUmbrella))]
    public static class LaserUmbrellaPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Connected")]
        public static void PostConnect(LaserUmbrella __instance, Plant plant)
        {
            if (__instance.lightBall is not null)
            {
                Plugin.printString(__instance.lightBall.name);
                Plugin.printString(__instance.lightBall.transform.parent.name);
            }
        }
    }*/
    [HarmonyPatch(typeof(Bullet_cabbage))]
    public static class Bullet_cabbagePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("HitZombie")]
        public static void PostHitZombie(Bullet_cabbage __instance, Zombie zombie)
        {
            if (__instance.Damage == 60)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + .2f), .4f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var ozom) && ozom.theZombieRow == __instance.theBulletRow)
                    {
                        if (ozom.isJalaed is true)
                        {
                            ozom.JalaedExplode(false, 20);
                        }
                        ozom.SetJalaed();
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(Plant))]
    public static class PlantPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("TakeDamage")]
        public static void PreTakeDamage(Plant __instance)
        {
            if (__instance.thePlantType == (PlantType)818)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y + .3f), 1f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && zombie.theZombieRow == __instance.thePlantRow)
                    {
                        if (TypeMgr.UltimateZombie(zombie.theZombieType) == false)
                        {
                            __instance.Die(Plant.DieReason.BySelf);
                            zombie.DestoryZombie();
                            CreateZombie.Instance.SetZombieWithMindControl(zombie.theZombieRow, ZombieType.JalapenoZombie, zombie.transform.position.x);
                            break;
                        }
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(SeedLibrary))]
    public static class SeedLibraryPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void PostStart(SeedLibrary __instance)
        {
            Plugin.newCard(820);
            Plugin.newCard(835);
            Plugin.newCard(842);
        }
    }

    [HarmonyPatch(typeof(InitBoard))]
    public static class InitBoardPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ReadySetPlant")]
        public static void PostReadySetPlant(InitBoard __instance)
        {
            if (GameAPP.theBoardLevel == Plugin.BossRushLevelID)
            {
                Plugin.printString("Boss Rush Initialized");
                MelonCoroutines.Start(Plugin.instance.InitializeBossRush());
            }
        }
    }

    [HarmonyPatch(typeof(ConveyManager))]
    public static class ConveyManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetCardPool")]
        public static void PostGetCardPool(ref Il2CppSystem.Collections.Generic.List<PlantType> __result)
        {
            CustomLevelData customLevelData;
            if (Utils.IsCustomLevel(out customLevelData) && customLevelData.BoardTag.isConvey)
            {
                if (GameAPP.theBoardLevel == Plugin.BossRushLevelID)
                {
                    if (Plugin.instance.CurrentBossRushStage == BossRushStage.Normal)
                    {
                        __result = Plugin.BossRushSet1.ToIl2CppList<PlantType>();
                    }
                    else
                    {
                        __result = Plugin.BossRushSet2.ToIl2CppList<PlantType>();
                    }
                }
                else
                {
                    __result = customLevelData.ConveyBeltPlantTypes().ToIl2CppList<PlantType>();
                }
            }
        }
    }
    /*[HarmonyPatch(typeof(UIMgr))]
    public static class UIMgrPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("EnterChallengeMenu")]
        [HarmonyPriority(800)]
        public static void PostEnterChallengeMenu(UIMgr __instance)
        {
            Transform transform = GameAPP.canvas.GetChild(0).FindChild("Levels");
            Transform transform2 = transform.FindChild("PageCustomLevel");
            foreach (Transform @object in transform.GetComponentsInChildren<Transform>(true))
            {
                if (@object.name.Contains("Page"))
                {
                    Plugin.printString(@object.name);
                }
            }
            if (transform2)
            {
                Transform transform3 = transform2.FindChild("Pages");
                foreach (Transform transform4 in transform3.GetComponentInChildren<Transform>(true))
                {
                    if (transform4.GetChildCount() >= 2)
                    {
                        Image image;
                        Advanture_Btn button;
                        if (transform4.GetChild(0).TryGetComponent<Image>(out image) && transform4.GetChild(1).TryGetComponent<Advanture_Btn>(out button))
                        {
                            if (button.buttonNumber == Plugin.BossRushLevelID)
                            {
                                image.transform.localScale = new(1, 1, 1);
                            }
                        }
                    }
                }
            }
        }
    }*/
}
