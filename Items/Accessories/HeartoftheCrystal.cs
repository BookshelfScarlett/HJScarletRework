using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    public class HeartoftheCrystal: HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Equips;
        public override void ExSD()
        {
            Item.width = Item.height = 60;
            Item.accessory = true;
            Item.SetUpRarityPrice(ItemRarityID.Red);

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.HJScarlet().heartoftheCrystal = true;
            player.GetCritChance<MagicDamageClass>() += 15;
            player.GetDamage<MagicDamageClass>() *= 0.82f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentNebula, 10).
                AddIngredient(ItemID.ManaFlower).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
