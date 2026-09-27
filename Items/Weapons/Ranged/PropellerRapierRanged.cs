using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class PropellerRapierRanged : HJScarletWeapon
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override string Texture => GetInstance<PropellerRapier>().Texture;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.SetUpNoUseGraphicItem(false, true);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.damage = 555;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<PropellerRapierRangedProj>();
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

            base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient<PropellerRapier>().
                AddIngredient<CrownofSilveryLight>(5).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
