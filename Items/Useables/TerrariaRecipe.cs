using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class TerrariaRecipe : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
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
            return false;
        }
        public override bool CanRightClick()
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.HJScarlet().terraRecipe;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            HJScarletPlayer modPlayer = Main.LocalPlayer.HJScarlet();
            bool isEquipped = Main.LocalPlayer.HJScarlet().terraRecipe;
            bool isPressingLeftAlt = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt);
            string value = this.GetLocalizationKey($"Active");
            if (!modPlayer.terraRecipe)
                return;
            if (isPressingLeftAlt)
            {
                //获取表单与对应的名字。
                List<int> foodList = modPlayer.terraRecipe_CurEat;
                List<int> notEatenFoodList = modPlayer.terraRecipe_haventEat;
                //直接遍历一个表单
                //这里表单是严格11对应的，理论来说不会出现问题，大概
                string combineValue = null;
                int line = 1;
                if (foodList.Count != 0)
                    combineValue = $"{line}-";
                for (int i = 0; i < foodList.Count; i++)
                {
                    //尽管如此，这里仍然需要过一个可能的保护
                    string perInstance = $"[i:{foodList[i]}]";
                    //将其放进这个列表里合并起来
                    combineValue += $"{perInstance}";
                    //如果i每次都%8==0,新开一行
                    if ((i + 1) % 10 == 0)
                    {
                        //过一个判定看这个值是不是超过了列表数
                        //这样就不会新开一个什么都没有的行，因为写法上的问题
                        if (i + 1 < notEatenFoodList.Count)
                        {
                            line++;
                            combineValue += $"\n{line}-";
                        }
                    }
                }
                //处理未食用的食物清单。相同的逻辑
                string combineValue2 = null;
                line = 1;
                if (notEatenFoodList.Count != 0)
                    combineValue2 = $"{line}-";
                for (int i = 0; i < notEatenFoodList.Count; i++)
                {
                    //尽管如此，这里仍然需要过一个可能的保护
                    string perInstance = $"[i:{notEatenFoodList[i]}]";
                    //将其放进这个列表里合并起来
                    combineValue2 += $"{perInstance}";
                    //如果i每次都%8==0,新开一行
                    if ((i + 1) % 10 == 0)
                    {
                        if (i + 1 < notEatenFoodList.Count)
                        {
                            line++;
                            combineValue2 += $"\n{line}-";
                        }
                    }
                }
                //最后使用replace的插值字符串。
                tooltips.ReplaceAllTooltip(this.GetLocalizationKey($"FoodList"), null, combineValue, combineValue2, foodList.Count, notEatenFoodList.Count);

            }
            else if (isEquipped)
                tooltips.CreateTooltip(value, Color.GreenYellow);
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //与沉浸背包的使用存在bug，在启用沉浸背包时不要给这个贴图转向
            bool hasImmersiveInventory = ModLoader.HasMod("ImmersiveInventory");
            if (Main.LocalPlayer.HJScarlet().terraRecipe && !hasImmersiveInventory)
            {
                spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, PiOver2, origin, scale, 0, 0);
                return false;
            }
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            bool hasImmersiveInventory = ModLoader.HasMod("ImmersiveInventory");
            if (Main.LocalPlayer.HJScarlet().terraRecipe && !hasImmersiveInventory)
            {
                Texture2D tex = TextureAssets.Item[Type].Value;
                Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
                Rectangle iFrame = tex.Frame();
                spriteBatch.Draw(tex, position, iFrame, Color.White, rotation + PiOver2, tex.Size() / 2, scale, 0f, 0f);
                return false;
            }
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
        public override void RightClick(Player player)
        {
            var shorthand = player.HJScarlet();
            player.HJScarlet().terraRecipe = !player.HJScarlet().terraRecipe;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Book).
                AddRecipeGroup(HJScarletRecipeGroup.AnyGoldCritter).
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();
            CreateRecipe().
                AddIngredient(ItemID.Book).
                AddIngredient(ItemID.GoldenDelight).
                AddIngredient(ItemID.BlackInk).
                AddTile(TileID.WorkBenches).
                DisableDecraft().
                Register();

        }
    }
}
