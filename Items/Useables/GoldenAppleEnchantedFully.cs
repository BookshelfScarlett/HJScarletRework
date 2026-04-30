using ContinentOfJourney.Items.Material;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class GoldenAppleEnchantedFully :HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override string Texture => GetInstance<GoldenAppleEnchanted>().Texture;
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
        }
        public override void UpdateInventory(Player player)
        {
            player.buffImmune[BuffType<GoldenAppleBuff>()] = true;
            player.buffImmune[BuffType<GoldenAppleBuffEnchanted>()] = true;
            player.HJScarlet().goldenAppleDamageAbsorb = 200;
            player.HJScarlet().goldenAppleEnchantedFully = true;
            player.lavaImmune = true;
            player.noKnockback = true;
            player.lifeRegen += 4;
            player.statDefense += 50;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GoldenAppleEnchanted>(50).
                AddIngredient<EssenceofBright>(10).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
