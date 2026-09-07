using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class RandomNewFolder : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void ExSD()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.maxStack = Item.CommonMaxStack;
            Item.ammo = AmmoID.None;
            Item.consumable = false;
            Item.knockBack = 1f;
            Item.shootSpeed = 8f;
        }
        public override void UpdateInventory(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Item item = player.HeldItem;
                if (item.IsLegal() && item.useAmmo > 0 && item.DamageType.CountsAsClass<RangedDamageClass>())
                {
                    Item.ammo = item.useAmmo;
                }
                else
                    Item.ammo = AmmoID.None;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EndlessMusketPouch).
                AddIngredient(ItemID.EndlessQuiver).
                AddIngredient(ItemID.RocketI, 3996).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}
