using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    /// <summary>
    /// 是的，我把书架做成了武器
    /// </summary>
    public class WeHaveBookshelf :ExecutorWeaponClass
    {
        public override string Texture => GetVanillaAssetPath(Globals.Enums.VanillaAsset.Item, ItemID.Bookcase);
        public override int ExecutionProgress => 20;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true);
            Item.damage = 120;
            Item.knockBack = 5f;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<WeHaveBookshelfHeldProj>();
            Item.shootSpeed = 16f;
        }
       
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
