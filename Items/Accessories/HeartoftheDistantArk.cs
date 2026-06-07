using ContinentOfJourney.Items.Material;
using HJScarletRework.Core;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players.Dashes;
using HJScarletRework.Globals.Systems;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheDistantArk : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public int LifeMax2 = 20;
        public override void SetStaticDefaults()
        {
            HJScarletList.RareItemRarityDrawDictionary.Add(Type, Rarity.RarityShiny.RareItemRarity.RareType.Copper);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.accessory = true;
            Item.expert = true;
            Item.defense = 3;
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeMax2);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noKnockback = true;
            player.statLifeMax2 += LifeMax2;
            player.ApplyDash(ScarletContent.DashType<HeartoftheDistantArkDash>());
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EoCShield).
                AddIngredient(ItemID.CobaltShield).
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 10).
                AddIngredient<DeepBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
