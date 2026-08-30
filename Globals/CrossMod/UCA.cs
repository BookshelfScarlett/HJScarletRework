using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.List;
using HJScarletRework.Items.Materials;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.CrossMod
{
    public class UCACrossModSupportProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override void SetStaticDefaults()
        {
            if (!HJScarletConfigServer.Instance.CrossModSupport)
                return;
            if (HJScarletRework.CrossMod_UCA is null)
                return;
            int heavySwing = HJScarletRework.CrossMod_UCA.Find<ModProjectile>("StormRulerHeldHeavySwing").Type;
            int normalSwing = HJScarletRework.CrossMod_UCA.Find<ModProjectile>("StormRulerHeldSwingProj").Type;
            int kingofStorm = HJScarletRework.CrossMod_UCA.Find<ModProjectile>("StormRulerHeld_KingofStorm").Type;
            int Lunge = HJScarletRework.CrossMod_UCA.Find<ModProjectile>("StormRulerHeldProj_Lunge").Type;
            int LungeKingofStorm = HJScarletRework.CrossMod_UCA.Find<ModProjectile>("StormRulerHeldProj_Lunge_KingofStorm").Type;
            ScarletItemIDSets.GiantKiller[heavySwing] = true;
            ScarletItemIDSets.GiantKiller[normalSwing] = true;
            ScarletItemIDSets.GiantKiller[kingofStorm] = true;
            ScarletItemIDSets.GiantKiller[Lunge] = true;
            ScarletItemIDSets.GiantKiller[LungeKingofStorm] = true;
        }
    }
    public class UCACrossModSupport : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void SetStaticDefaults()
        {
            if (!HJScarletConfigServer.Instance.CrossModSupport)
                return;

            if (HJScarletRework.CrossMod_UCA is null)
                return;
            int carnage = HJScarletRework.CrossMod_UCA.Find<ModItem>("CarnageRay").Type;
            HJScarletList.ShinyRarityItemDictionary.Add(carnage, Enums.ShinyRarityType.ScarletRed);
            int night = HJScarletRework.CrossMod_UCA.Find<ModItem>("NightsRayAlt").Type;
            int shadow = HJScarletRework.CrossMod_UCA.Find<ModItem>("ShadowBoltStaffAlt").Type;
            HJScarletList.ShinyRarityItemDictionary.Add(night, Enums.ShinyRarityType.ForeverNight);
            HJScarletList.ShinyRarityItemDictionary.Add(shadow, Enums.ShinyRarityType.ForeverNight);
            int vivid = HJScarletRework.CrossMod_UCA.Find<ModItem>("VividClarityAlt").Type;
            int element = HJScarletRework.CrossMod_UCA.Find<ModItem>("ElementRayAlt").Type;
            int sword = HJScarletRework.CrossMod_UCA.Find<ModItem>("StormRulerAlt").Type;
            HJScarletList.ShinyRarityItemDictionary.Add(vivid, Enums.ShinyRarityType.FateWhite);
            HJScarletList.ShinyRarityItemDictionary.Add(element, Enums.ShinyRarityType.FateWhite);
            HJScarletList.ShinyRarityItemDictionary.Add(sword, Enums.ShinyRarityType.FateWhite);
            ScarletItemIDSets.GiantKiller[sword] = true;
            int terra = HJScarletRework.CrossMod_UCA.Find<ModItem>("TerraRay").Type;
            HJScarletList.ShinyRarityItemDictionary.Add(terra, Enums.ShinyRarityType.Life);
            int plasma = HJScarletRework.CrossMod_UCA.Find<ModItem>("PlasmaRodAlt").Type;
            int soul = HJScarletRework.CrossMod_UCA.Find<ModItem>("SoulPiercerAlt").Type;
            HJScarletList.ShinyRarityItemDictionary.Add(plasma, Enums.ShinyRarityType.Nebula);
            HJScarletList.ShinyRarityItemDictionary.Add(soul, Enums.ShinyRarityType.Nebula);

        }
    }
    public class UCACrossModSupportSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            if (!HJScarletConfigServer.Instance.CrossModSupport)
                return;

            if (HJScarletRework.CrossMod_UCA is null)
                return;
            if (HJScarletRework.CrossMod_Calamity is not null)
                return;
            int carnageRay = HJScarletRework.CrossMod_UCA.Find<ModItem>("CarnageRay").Type;
            int nightRay = HJScarletRework.CrossMod_UCA.Find<ModItem>("NightsRayAlt").Type;
            int terraRay = HJScarletRework.CrossMod_UCA.Find<ModItem>("TerraRay").Type;
            int shadowbolt = HJScarletRework.CrossMod_UCA.Find<ModItem>("ShadowBoltStaffAlt").Type;
            //元素射线
            int elementalRay = HJScarletRework.CrossMod_UCA.Find<ModItem>("ElementRayAlt").Type;
            //灵魂穿透者
            int soulPiercer = HJScarletRework.CrossMod_UCA.Find<ModItem>("SoulPiercerAlt").Type;
            //耀界之光
            int vividClarity = HJScarletRework.CrossMod_UCA.Find<ModItem>("VividClarityAlt").Type;
            //风暴管束者
            int stormBlade = HJScarletRework.CrossMod_UCA.Find<ModItem>("StormRulerAlt").Type;
            int[] ucaWeapons = [carnageRay, nightRay, terraRay, shadowbolt, elementalRay, soulPiercer, vividClarity, stormBlade];
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                if (recipe.HasResult(terraRay))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(shadowbolt))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(elementalRay) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(vividClarity) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(soulPiercer) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.DisableRecipe();
                }
                if (recipe.HasResult(stormBlade) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.DisableRecipe();
                }

            }
            Recipe.Create(stormBlade).
                AddIngredient<TornadoScythe>().
                AddIngredient<SoulofBlight>(5).
                AddIngredient<DeepBar>(5).
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(terraRay).
                AddIngredient(nightRay).
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(terraRay).
                AddIngredient(carnageRay).
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(elementalRay).
                AddIngredient(terraRay).
                AddIngredient<UniversalCube>().
                AddTile(TileID.LunarCraftingStation).
                Register();

            Recipe.Create(shadowbolt).
                AddIngredient(ItemID.ShadowbeamStaff).
                AddIngredient<CubistBar>(5).
                AddIngredient<LivingBar>(5).
                AddIngredient<EternalBar>(5).
                AddTile(FinalAnvilTile).
                Register();

            Recipe.Create(soulPiercer).
                AddIngredient(ItemID.ShadowbeamStaff).
                AddIngredient<EssenceofTime>(5).
                AddIngredient<EssenceofLife>(5).
                AddIngredient<EssenceofMatter>(5).
                AddTile(FinalAnvilTile).
                Register();

            Recipe.Create(vividClarity)
                .AddIngredient(elementalRay)
                .AddIngredient(shadowbolt)
                .AddIngredient(soulPiercer)
                .AddIngredient<CrownofSilveryLight>(15)
                .AddTile(FinalAnvilTile)
                .Register();
        }
    }
}
