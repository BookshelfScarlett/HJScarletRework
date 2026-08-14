using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Magic
{
    public class Fleshtumor : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override void ExSD()
        {
            Item.damage = 66;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem(true, true);
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = null;
            Item.mana = 5;
            Item.knockBack = 4f;
            Item.shoot = ProjectileType<FleshtumorHeldProj>();
            Item.shootSpeed = 16;
        }
        public override bool CanShoot(Player player)
        {
            return true;
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj(Item.shoot))
                return;
            Vector2 dir = player.ToMouseVector2();
            int projDamage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(Item.damage);
            if (Main.myPlayer != player.whoAmI)
                return;
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, projDamage, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.netUpdate = true;
            ScarletSound(HJScarletSounds.Misc_KnifeExpired, player.Center, 1, 0, -.2f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
}
