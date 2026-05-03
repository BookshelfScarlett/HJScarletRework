using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Projs.General;
using HJScarletRework.Projs.Magic;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items
{
    public class Wreach : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletItemProj.Wreach.Path;
        public override void ExSD()
        {
            Item.width = Item.height = 50;
            Item.damage = 20;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<CoronaFireball>();
            Item.shootSpeed = 6f;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            foreach (var keys in HJScarletList.ExecutorWeaponDictionary.Keys)
            {
                player.QuickSpawnItem(source, keys);
                Main.NewText(HJScarletList.ExecutorWeaponDictionary[keys]);
            }
            return true;
            //Vector2 ownerMW = player.LocalMouseWorld();
            Projectile proj = Projectile.NewProjectileDirect(source, Main.MouseWorld, RandVelTwoPi(2f), ProjectileType<FloatingPlants>(), 0, knockback, player.whoAmI);
            proj.rotation = RandRotTwoPi;
            proj.ai[1] = Main.rand.Next(0, 7);

            //添加需要的攻击单位
            return false;
        }
    }
}
