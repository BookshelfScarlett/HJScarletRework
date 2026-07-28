using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Tiles;
using HJScarletRework.Globals.List;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.CrossMod
{
    public class UCACrossModSupport : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void SetStaticDefaults()
        {
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
            if (HJScarletRework.CrossMod_UCA is null)
                return;
            if (HJScarletRework.CrossMod_Calamity is not null)
                return;
            int shadowbolt = HJScarletRework.CrossMod_UCA.Find<ModItem>("ShadowBoltStaffAlt").Type;
            //元素射线
            int elementalRay = HJScarletRework.CrossMod_UCA.Find<ModItem>("ElementRayAlt").Type;
            //灵魂穿透者
            int soulPiercer = HJScarletRework.CrossMod_UCA.Find<ModItem>("SoulPiercerAlt").Type;
            //耀界之光
            int vividClarity = HJScarletRework.CrossMod_UCA.Find<ModItem>("VividClarityAlt").Type;
            //风暴管束者
            int stormBlade = HJScarletRework.CrossMod_UCA.Find<ModItem>("StormRulerAlt").Type;

            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                //影流法杖：合成站更改为物质喷泉，添加物质精华
                if (recipe.HasResult(shadowbolt) && recipe.createItem.stack == 1 && recipe.HasIngredient(ItemID.LunarBar) && recipe.HasIngredient(ItemID.ShadowbeamStaff) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.RemoveTile(TileID.LunarCraftingStation);
                    recipe.AddIngredient<EssenceofMatter>(10);
                    recipe.AddTile<FountainofMatter>();
                }
                //元素法杖：合成站更改为最终金属砧，三精华
                if (recipe.HasResult(elementalRay) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.RemoveTile(TileID.LunarCraftingStation);
                    recipe.AddIngredient<EssenceofLife>(10);
                    recipe.AddIngredient<EssenceofMatter>(10);
                    recipe.AddIngredient<EssenceofTime>(10);
                    recipe.AddTile(FinalAnvilTile);
                }
                //耀界：合成站更改为最终金属砧，加入所有版本的考验罐子
                if (recipe.HasResult(vividClarity) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.DisableRecipe();
                }
                //灵魂穿透者：合成站为生命喷泉，加入生命物质
                if (recipe.HasResult(soulPiercer) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.RemoveTile(TileID.LunarCraftingStation);
                    recipe.AddIngredient<EssenceofLife>(10);
                    recipe.AddTile<FountainofLife>();
                }
                //风暴管束者：毙掉原本的合成，加入三矿锭和毁灭者刃
                if (recipe.HasResult(stormBlade) && recipe.HasTile(TileID.LunarCraftingStation))
                {
                    recipe.RemoveTile(TileID.LunarCraftingStation);
                    recipe.RemoveIngredient(ItemID.FragmentVortex);
                    recipe.AddIngredient<DestroyerBlade>();
                    recipe.AddIngredient<LivingBar>(10);
                    recipe.AddIngredient<CubistBar>(10);
                    recipe.AddIngredient<EternalBar>(10);
                    recipe.AddTile(FinalAnvilTile);
                }

            }
            Recipe.Create(vividClarity)
                    .AddIngredient(elementalRay)
                    .AddIngredient(shadowbolt)
                    .AddIngredient(soulPiercer)
                    .AddIngredient<TankOfThePastCave>(10)
                    .AddIngredient<TankOfThePastCave>(10)
                    .AddIngredient<TankOfThePastCorruption>(10)
                    .AddIngredient<TankOfThePastCrimson>(10)
                    .AddIngredient<TankOfThePastDesert>(10)
                    .AddIngredient<TankOfThePastForest>(10)
                    .AddIngredient<TankOfThePastHallow>(10)
                    .AddIngredient<TankOfThePastJungle>(10)
                    .AddIngredient<TankOfThePastSky>(10)
                    .AddIngredient<TankOfThePastSnowland>(10)
                    .AddIngredient<TankOfThePastUnderworld>(10)
                    .AddTile(FinalAnvilTile)
                    .Register();
        }
    }
}
