using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class TerrariaRecipe : HJScarletItemClass
    {
        public static int FoodCount = 5;
        public static int StatLifePerAdd = 5;
        public override string AssetPath => AssetHandler.Useables;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FoodCount, StatLifePerAdd);
        public override void ExSD()
        {
            Item.width = 50;
            Item.height = 50;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.stack = 1;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool ConsumeItem(Player player)
        {
            return true;
        }
        public override bool CanRightClick()
        {
            return true;
        }
        public override void UpdateInventory(Player player)
        {
            player.HJScarlet().terraRecipe = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            HJScarletPlayer modPlayer = Main.LocalPlayer.HJScarlet();
            string value = this.GetLocalizationKey("Active");
            tooltips.CreateTooltip(value, Color.GreenYellow, LineName: "ActiveName");
            if (Main.keyState.PressingAlt())
            {
                IReadOnlyList<string> foodList = modPlayer.terrariaRecipeEatenList;
                IReadOnlyList<string> notEatanFoodList = modPlayer.terrariaRecipeNotEatenList;
                string combination = null;
                int line = 1;
                if (foodList.Count != 0)
                    combination = $"{line}-";
                for (int i = 0; i < foodList.Count; i++)
                {
                    string perFood = $"[i:{foodList[i]}]";
                    combination += perFood;
                    //如果i每次都%10==0,新开一行
                    if ((i + 1) % 10 == 0)
                    {
                        //过一个判定看这个值是不是超过了列表数
                        //这样就不会新开一个什么都没有的行，因为写法上的问题

                        if (i + 1 < foodList.Count)
                        {
                            line++;
                            combination += $"\n{line}-";
                        }
                    }
                }


                string combination2 = null;
                line = 1;
                if (notEatanFoodList.Count != 0)
                    combination2 = $"{line}-";
                for (int i = 0; i < notEatanFoodList.Count; i++)
                {
                    string perFood = $"[i:{notEatanFoodList[i]}]";
                    combination2 += perFood;
                    //如果i每次都%10==0,新开一行
                    if ((i + 1) % 10 == 0)
                    {
                        //过一个判定看这个值是不是超过了列表数
                        //这样就不会新开一个什么都没有的行，因为写法上的问题

                        if (i + 1 < notEatanFoodList.Count)
                        {
                            line++;
                            combination2 += $"\n{line}-";
                        }
                    }
                }
                tooltips.ReplaceAllTooltip(this.GetLocalizationKey("FoodList"), null, combination, combination2, foodList.Count, notEatanFoodList.Count);
            }
        }
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), ItemType<TerrariaRecipeDeactive>());
            //player.HJScarlet().terraRecipe = false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Book).
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldCritter).
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.WorkBenches).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.Book).
                AddIngredient(ItemID.GoldenDelight).
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.WorkBenches).
                Register();

        }
    }
}
