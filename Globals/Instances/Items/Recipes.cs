using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Accessories;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Mounts.Rudders;
using ContinentOfJourney.Items.Placables.FishingCrate;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using HJScarletRework.Items.Weapons.Executor.Thrown;
using HJScarletRework.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public void GlobalAccessoriesRecipe()
        {
            Recipe.Create(ItemID.WarriorEmblem).
                AddIngredient<EmblemExecutor>().
                DisableDecraft().
                AddTile(TileID.TinkerersWorkbench).
                Register();

            Recipe.Create(ItemID.ManaFlower).
                AddIngredient<ArtificalManaStar>().
                AddIngredient(ItemID.NaturesGift).
                AddTile(TileID.TinkerersWorkbench).
                DisableDecraft().
                Register();

            Recipe.Create(ItemID.DestroyerEmblem).
                AddIngredient(ItemID.Amber, 15).
                AddIngredient<DisasterEssence>(50).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(ItemID.CobaltShield).
                AddRecipeGroup(HJScarletRecipeGroup.AnyCobaltBar, 10).
                DisableDecraft().
                AddTile(TileID.Anvils).
                Register();

            Recipe.Create(ItemID.CrossNecklace).
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldBar, 16).
                AddIngredient(ItemID.BrokenBatWing).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(ItemID.MedicatedBandage).
                AddIngredient<BambooShield>().
                AddIngredient(ItemID.AdhesiveBandage).
                AddTile(TileID.TinkerersWorkbench).
                DisableDecraft().
                Register();


            Recipe.Create(ItemID.PocketMirror).
                AddIngredient(ItemID.MagicMirror).
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldBar, 10).
                DisableDecraft().
                AddTile(TileID.TinkerersWorkbench).
                Register();

            Recipe.Create(ItemID.AvengerEmblem).
                AddIngredient<EmblemExecutor>().
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                DisableDecraft().
                Register();

            Recipe.Create(ItemID.SunStone).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                AddIngredient<DisasterEssence>(50).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

        }
        public void GlobalMaterialRecipes()
        {
            Recipe.Create(ItemType<FinalBar>()).
                AddIngredient<CrownofSilveryLight>(15).
                DisableDecraft().
                AddTile(FinalAnvilTile).
                Register();
        }
        public void GlobalWeaponRecipes()
        {
            Recipe.Create(ItemID.Spear).
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 12).
                DisableDecraft().
                AddTile(TileID.Anvils).
                Register();


            Recipe.Create(ItemID.NorthPole).
                AddIngredient<AzureFrostmark>().
                AddIngredient(ItemID.Ectoplasm, 50).
                AddTile(TileID.Autohammer).
                DisableDecraft().
                Register();


            Recipe.Create(ItemID.Amarok).
                AddIngredient(ItemID.HelFire).
                AddIngredient(ItemID.FrostCore).
                DisableDecraft().
                Register();


            Recipe.Create(ItemType<MoltenKnife>()).
                AddIngredient<NetherStar>().
                AddTile(TileID.DemonAltar).
                DisableDecraft().
                Register();

        }
        public void GlobalMiscRecipes()
        {
            Recipe.Create(ItemID.Autohammer).
                AddIngredient(ItemID.ChlorophyteWarhammer).
                AddIngredient(ItemID.GlowingMushroom, 100).
                AddIngredient(ItemID.Ectoplasm, 30).
                DisableDecraft().
                AddTile(TileID.CrystalBall).
                Register();
            Recipe.Create(ItemID.GuideVoodooDoll).
                AddIngredient(ItemID.GuideVoodooFish).
                Register();

            //墓们
            Recipe.Create(ItemID.Tombstone).
                AddIngredient(ItemID.StoneBlock, 50).
                AddTile(TileID.HeavyWorkBench).
                DisableDecraft().
                Register();
            for (int i = ItemID.Headstone; i <= ItemID.Obelisk; i++)
            {
                Recipe.Create(i).
                    AddIngredient(ItemID.StoneBlock, 50).
                    AddTile(TileID.HeavyWorkBench).
                    DisableDecraft().
                    Register();
            }
            for (int i = ItemID.RichGravestone1; i <= ItemID.RichGravestone5; i++)
            {
                Recipe.Create(i).
                    AddIngredient(ItemID.StoneBlock, 50).
                    AddTile(TileID.HeavyWorkBench).
                    DisableDecraft().
                    Register();
            }
            Recipe.Create(ItemID.GoldenKey).
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldBar, 10).
                AddIngredient(ItemID.Bone, 30).
                DisableDecraft().
                AddTile(TileID.Anvils).
                Register();

            Recipe.Create(ItemID.ObsidianSwordfish).
                AddIngredient(ItemID.HotlineFishingHook).
                AddIngredient(ItemID.HellButterfly, 300).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

        }
        public void FargoMutantCrossMod()
        {

            if (!ModLoader.TryGetMod("Fargowiltas", out Mod fargoWiltas))
                return;
            Recipe.Create(ItemType<AzureFrostmark>()).
                AddRecipeGroup(HJScarletRecipeGroup.AnyIceCrate, 5).
                DisableDecraft().
                AddTile(TileID.Solidifier).
                Register();
            Recipe.Create(ItemType<DungeonBreaker>()).
                AddRecipeGroup(HJScarletRecipeGroup.AnyDungeonCrate, 5).
                DisableDecraft().
                AddTile(TileID.Solidifier).
                Register();

            //旅人宝匣添加的特殊掉落

            //影子镐
            Recipe.Create(ItemType<ShadowPickaxe>()).
                AddRecipeGroup(HJScarletRecipeGroup.AnyDarkCrate, 5).
                DisableDecraft().
                AddTile(TileID.Solidifier).
                Register();
            //马赛克法杖
            Recipe.Create(ItemType<MosaicStaff>()).
                AddIngredient<ShadowCrate>(5).
                DisableDecraft().
                AddTile(TileID.Solidifier).
                Register();
            //河流法杖
            Recipe.Create(ItemType<RiverStaff>()).
                AddIngredient<DistortedCrate>(5).
                DisableDecraft().
                AddTile(TileID.Solidifier).
                Register();
            //船舵（们）
            int[] anchor = [ItemType<ClockworkRudder>(), ItemType<DangerousRudder>(), ItemType<FlourishRudder>(), ItemType<HotRudder>(), ItemType<LunarRudder>(), ItemType<NoRudder>(), ItemType<TriangularRudder>(), ItemType<WoodenRudder>()];
            for (int i = 0; i < anchor.Length; i++)
            {
                Recipe.Create(anchor[i]).
                    AddIngredient<CubistCrate>(5).
                    DisableDecraft().
                    AddTile(TileID.Solidifier).
                    Register();
            }
            //叶绿复合弓
            Recipe.Create(ItemType<ChlorophyteCompositeBow>()).
                AddRecipeGroup(HJScarletRecipeGroup.AnyLivingCrate, 5).
                DisableDecraft().
                AddTile(TileID.Solidifier).
                Register();
            //时转
            Recipe.Create(ItemType<TimeTurner>()).
                AddIngredient<CountdownCrate>(5).
                DisableDecraft().
                AddTile(TileID.Solidifier).
                Register();
        }
        public override void AddRecipes()
        {
            GlobalAccessoriesRecipe();
            GlobalMaterialRecipes();
            GlobalWeaponRecipes();
            GlobalMiscRecipes();
            FargoMutantCrossMod();
        }
    }
}
