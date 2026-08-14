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
    public class PestilenceFlower : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override void ExSD()
        {
            Item.damage = 66;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem(true, true);
            Item.mana = 5;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = null;
            Item.knockBack = 4f;
            Item.shoot = ProjectileType<PestilenceFlowerHeldProj>();
            Item.shootSpeed = 16;
        }
        public override bool CanShoot(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj(Item.shoot))
                return;
            if (Main.myPlayer != player.whoAmI)
                return;
            int projDamage = (int)player.GetTotalDamage<MagicDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, projDamage, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.netUpdate = true;
            ScarletSound(HJScarletSounds.Misc_KnifeExpired, player.Center, 1, 0, -.2f);

        }

    }
}
