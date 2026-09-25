using ContinentOfJourney.Items.Material;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class GoldenAppleEnchanted : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.LegalFoodList.Add(Type);
        }
        public override void ExSD()
        {
            Item.DefaultToFood(16, 16, BuffType<GoldenAppleBuffEnchanted>(), GetSeconds(60) * 60);
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.value = Item.sellPrice(platinum: 3);
            Item.HJScarlet().drawBuffIconAndDetail = true;
        }
        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.WellFed3, GetSeconds(60) * 60);
            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GoldenApple>().
                AddIngredient<EternalBar>(50).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
