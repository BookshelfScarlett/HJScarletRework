using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class DiamondYoyo : GemYoyoWeapon
    {
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Database.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            base.ExSD();
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.damage = 15;
            Item.shootSpeed = 13f;
            Item.shoot = ProjectileType<DiamondYoyoProj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodYoyo).
                AddIngredient(ItemID.Diamond, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
