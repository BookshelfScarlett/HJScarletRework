using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class PinballPurgatory : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Pink);
            Item.accessory = true;
            Item.defense = 1;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().mayaPumper = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MayaBall>().
                AddIngredient(ItemID.HellstoneBar, 10).
                AddIngredient(ItemID.SoulofLight, 10).
                AddIngredient(ItemID.Diamond, 10).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
