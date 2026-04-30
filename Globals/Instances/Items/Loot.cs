using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Items.Weapons.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
         public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                case ItemID.GolemBossBag:
                    itemLoot.AddLoot<DisasterEssence>(1, 10, 20);
                    break;
                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    itemLoot.AddLoot<AzureFrostmark>(4);
                    break;
                case ItemID.DungeonFishingCrate:
                case ItemID.DungeonFishingCrateHard:
                    itemLoot.AddLoot<DungeonBreaker>(4);
                    itemLoot.AddLoot(ItemID.TallyCounter, 4);
                    break;
                case ItemID.WallOfFleshBossBag:
                    itemLoot.AddLoot<ExecutorEmblem>(3);
                    break;

            }
            if (item.type == ItemType<WallofShadowTreasureBag>())
            {
                itemLoot.AddLoot<ExecutorBadge>(3);
                itemLoot.AddLoot<DeathTolls>(4);
            }
            if (item.type == ItemType<PriestessRodTreasureBag>())
            {
                itemLoot.AddLoot<ClimaticHawstring>(3);
            }
            if (Main.masterMode)
            {
                if (item.type == ItemType<ScarabBeliefTreasureBag>())
                    itemLoot.AddLoot<SacarbWings>(10);
                
            }
        }
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.ManaFlower).
                AddIngredient<ArtificalManaStar>().
                AddIngredient(ItemID.NaturesGift).
                AddTile(TileID.TinkerersWorkbench).
                DisableDecraft().
                Register();

            Recipe.Create(ItemID.Spear).
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 12).
                DisableDecraft().
                AddTile(TileID.Anvils).
                Register();

            Recipe.Create(ItemID.SunStone).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                AddIngredient<DisasterEssence>(50).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(ItemID.DestroyerEmblem).
                AddIngredient(ItemID.Amber, 15).
                AddIngredient<DisasterEssence>(50).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(ItemID.NorthPole).
                AddIngredient<AzureFrostmark>().
                AddIngredient(ItemID.Ectoplasm, 50).
                AddTile(TileID.Autohammer).
                DisableDecraft().
                Register();

            Recipe.Create(ItemID.AvengerEmblem).
                AddIngredient<ExecutorEmblem>().
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
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
