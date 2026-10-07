using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    [LegacyName("VanguardEmblem")]
    public class EmblemVanguard : HJScarletItemClass
    {
        public static int InvinceTime = 3;
        public static int Cooldown = 27;
        public override string AssetPath => AssetHandler.Equips;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(InvinceTime, Cooldown);
        public override void ExSD()
        {
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().emblemVanguard = true;
            player.noKnockback = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CobaltShield).
                AddIngredient(ItemID.HallowedBar, 10).
                AddIngredient<SoulofBlight>(15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
