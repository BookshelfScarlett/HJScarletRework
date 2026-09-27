using HJScarletRework.Core.NetSync;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Firearm
{
    public class Reflux : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 75;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateCopper);
        }
        public override void ExSD()
        {
            Item.damage = 25;
            Item.shootSpeed = 19;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.SetUpNoUseGraphicItem(true);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.knockBack = 7f;
            Item.useTime = Item.useAnimation = 32;
            Item.shoot = ProjectileType<RefluxHeldProj>();
            Item.HJScarlet().borderlandWeapon = true;
        }
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (!player.IsOwnerSide())
                return;
            if (player.HasProj(Item.shoot))
                return;
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.netUpdate = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.QuadBarrelShotgun).
                AddIngredient(ItemID.IllegalGunParts, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
