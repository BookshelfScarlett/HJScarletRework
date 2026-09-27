using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor.Caster
{
    public class MythrilStaff : ExecutorWeaponClass
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Caster;
        public override int ExecutionProgress => 25;
        public override void ExSSD()
        {
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 1120;
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.useTime = Item.useAnimation = 40;
            Item.knockBack = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }
    }
}
