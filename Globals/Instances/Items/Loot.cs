using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Placables.FishingCrate;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Items.Materials;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using HJScarletRework.Items.Weapons.Executor.Thrown;
using HJScarletRework.Items.Weapons.Magic;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            return base.OnPickup(item, player);
        }
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                #region 宝藏袋
                case ItemID.GolemBossBag:
                    itemLoot.AddLoot<DisasterEssence>(1, 10, 20);
                    break;
                case ItemID.PlanteraBossBag:
                    itemLoot.AddLoot<GrassKnife>(4);
                    itemLoot.AddLoot<PestilenceFlower>(4);
                    itemLoot.AddLoot<EmblemColdSteel>(4);
                    itemLoot.AddLoot<EmblemFirearm>(4);
                    itemLoot.AddLoot<EmblemThrown>(4);
                    break;
                case ItemID.WallOfFleshBossBag:
                    itemLoot.AddLoot<EmblemExecutor>(3);
                    itemLoot.AddLoot<MoltenKnife>(4);
                    itemLoot.AddLoot<Fleshtumor>(4);
                    break;
                case ItemID.MoonLordBossBag:
                    itemLoot.AddLoot<PrunusMume>(3);
                    break;
                case ItemID.EyeOfCthulhuBossBag:
                    itemLoot.AddLoot<TearEye>(4);
                    break;
                case ItemID.FishronBossBag:
                    itemLoot.AddLoot<FishronKnife>(4);
                    break;
                #endregion

                #region 板条箱

                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    itemLoot.AddLoot<AzureFrostmark>(4);
                    break;
                case ItemID.LockBox:
                    itemLoot.AddLoot<DungeonBreaker>(4);
                    itemLoot.AddLoot<DungeonKnife>(4);
                    break;
                case ItemID.DungeonFishingCrate:
                case ItemID.DungeonFishingCrateHard:
                    itemLoot.AddLoot(ItemID.TallyCounter, 4);
                    break;
                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    itemLoot.AddLoot<StarofHope>(4);
                    break;
                case ItemID.OasisCrate:
                case ItemID.OasisCrateHard:
                    itemLoot.AddLoot<DesertKnife>(4);
                    break;

                    #endregion
            }
            ModifyHardmodeCrateLooting(item.type, ref itemLoot);
            if (item.type == ItemType<WallofShadowTreasureBag>())
            {
                itemLoot.AddLoot<ExecutorBadge>(3);
                itemLoot.AddLoot<DeathTolls>(4);
            }
            if (item.type == ItemType<PriestessRodTreasureBag>())
            {
                itemLoot.AddLoot<ClimaticHawstring>(3);
            }
            if (item.type == ItemType<ShadowCrate>()
                || item.type == ItemType<ShinyCrate>()
                || item.type == ItemType<SolarCrate>()
                || item.type == ItemType<QuakyCrate>()
                || item.type == ItemType<CountdownCrate>()
                || item.type == ItemType<ForeverCrate>()
                || item.type == ItemType<MembraneCrate>()
                || item.type == ItemType<LivingCrate>()
                || item.type == ItemType<CubeCrate>()
                || item.type == ItemType<CubistCrate>())
            {
                itemLoot.AddLoot<PurePrismFate>(4, 25, 40);
            }
            if (item.type == ItemType<GoblinChariotTreasureBag>())
            {
                itemLoot.AddLoot<AngryBomb>(4);
            }
        }
        public void ModifyHardmodeCrateLooting(int crateType, ref ItemLoot loot)
        {
            switch (crateType)
            {
                case ItemID.FrozenCrateHard:
                    loot.AddLoot(ItemID.FrozenTurtleShell, 5);
                    loot.AddLoot(ItemID.ArcticDivingGear, 5);
                    break;
                case ItemID.JungleFishingCrateHard:
                    loot.AddLoot(ItemID.Bezoar, 5);
                    loot.AddLoot(ItemID.AdhesiveBandage, 5);
                    loot.AddLoot(ItemID.JungleRose, 5);
                    loot.AddLoot(ItemID.NaturesGift, 5);
                    break;
                case ItemID.WoodenCrateHard:
                    loot.AddLoot(ItemID.PaintSprayer, 5);
                    loot.AddLoot(ItemID.ExtendoGrip, 5);
                    loot.AddLoot(ItemID.PortableCementMixer, 5);
                    loot.AddLoot(ItemID.BrickLayer, 5);
                    break;
                case ItemID.IronCrateHard:
                    loot.AddLoot(ItemID.TigerClimbingGear, 5);
                    loot.AddLoot(ItemID.SharkToothNecklace, 5);
                    break;
                case ItemID.GoldenCrateHard:
                    loot.AddLoot(ItemID.GoblinTech, 5);
                    loot.AddLoot(ItemID.REK, 5);
                    loot.AddLoot(ItemID.GPS, 5);
                    break;
                case ItemID.OceanCrateHard:
                    loot.AddLoot(ItemID.HighTestFishingLine, 5);
                    loot.AddLoot(ItemID.TackleBox, 5);
                    loot.AddLoot(ItemID.AnglerEarring, 5);
                    break;
            }
            if (crateType == ItemType<QuakyCrate>())
            {
                loot.AddLoot(ItemID.FireGauntlet, 5);
                loot.AddLoot(ItemID.LavaWaders, 5);
                loot.AddLoot(ItemID.LavaproofTackleBag, 5);

            }
            if (crateType == ItemType<SolarCrate>() || crateType == ItemType<ShinyCrate>())
            {
                loot.AddLoot(ItemID.HorseshoeBundle, 5);
                loot.AddLoot(ItemID.CelestialStone, 5);

            }
            if (crateType == ItemType<ForeverCrate>() || crateType == ItemType<CountdownCrate>())
            {
                loot.AddLoot(ItemID.Shellphone, 5);
                loot.AddLoot(ItemID.GreedyRing, 5);

            }
            if (crateType == ItemType<CubistCrate>() || crateType == ItemType<CubeCrate>())
            {
                loot.AddLoot(ItemID.FrozenShield, 5);
                loot.AddLoot(ItemID.FrostsparkBoots, 5);
            }
            if (crateType == ItemType<LivingCrate>() || crateType == ItemType<MembraneCrate>())
            {

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
                AddIngredient<EmblemExecutor>().
                AddIngredient(ItemID.SoulofFright, 5).
                AddIngredient(ItemID.SoulofMight, 5).
                AddIngredient(ItemID.SoulofSight, 5).
                AddTile(TileID.MythrilAnvil).
                DisableDecraft().
                Register();

            Recipe.Create(ItemID.Amarok).
                AddIngredient(ItemID.HelFire).
                AddIngredient(ItemID.FrostCore).
                DisableDecraft().
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

            Recipe.Create(ItemType<MoltenKnife>()).
                AddIngredient<NetherStar>().
                AddTile(TileID.DemonAltar).
                DisableDecraft().
                Register();

            Recipe.Create(ItemID.PocketMirror).
                AddIngredient(ItemID.MagicMirror).
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldBar, 10).
                DisableDecraft().
                AddTile(TileID.TinkerersWorkbench).
                Register();

            if(!ModLoader.TryGetMod("Fargowiltas", out Mod fargoWiltas))

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
