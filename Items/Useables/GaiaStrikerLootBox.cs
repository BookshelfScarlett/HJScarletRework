using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Useables
{
    public class GaiaStrikerLootBox : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.ScarletRarityHashSet.Add(Type);
        }

        public override void ExSD()
        {
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.maxStack = 9999;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.shoot = ProjectileType<GaiaStrikerLootProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Player p = player;
            Vector2 pos = Vector2.UnitY * 10f + p.MountedCenter;
            Projectile.NewProjectileDirect(p.GetSource_FromThis(), pos, Vector2.UnitY, type, 0, 0, p.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DreamingLight>().
                AddIngredient<TheJudgement>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                AddCondition(HJScarletCraftingConditions.FirstTimeGaiaStriker).
                Register();
        }
    }
}
