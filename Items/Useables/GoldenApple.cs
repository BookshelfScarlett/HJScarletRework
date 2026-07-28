using HJScarletRework.Buffs;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class GoldenApple : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.LegalFoodList.Add(Type);
        }
        public override void ExSD()
        {
            Item.DefaultToFood(16, 16, BuffType<GoldenAppleBuff>(), GetSeconds(60) * 10);
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.value = Item.sellPrice(gold: 3);
        }
        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.WellFed3, GetSeconds(60) * 10);
            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Apple).
                AddIngredient(ItemID.GoldDust, 100).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
