using ContinentOfJourney.Items.Material;
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
            Recipe.Create(ItemType<CrownofSilveryLight>(), 30).
                AddIngredient<FinalBar>(1).
                DisableDecraft().
                AddTile(FinalAnvilTile).
                Register();
            Recipe.Create(ItemType<FinalBar>()).
                AddIngredient<CrownofSilveryLight>(30).
                DisableDecraft().
                AddTile(FinalAnvilTile).
                Register();
        }
        public override void AddRecipes()
        {
            GlobalAccessoriesRecipe();
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

            if (!ModLoader.TryGetMod("Fargowiltas", out Mod fargoWiltas))

                return;
            Recipe.Create(ItemType<AzureFrostmark>()).
                AddRecipeGroup(HJScarletRecipeGroup.AnyIceCrate, 5).
                AddTile(TileID.Solidifier).
                Register();
            Recipe.Create(ItemType<DungeonBreaker>()).
                AddRecipeGroup(HJScarletRecipeGroup.AnyDungeonCrate, 5).
                AddTile(TileID.Solidifier).
                Register();
        }

    }
}
