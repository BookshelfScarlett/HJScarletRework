using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Firearm
{
    public class ConferenceCall : ExecutorWeaponClass
    {
        public static int BulletsPerShot = 5;
        public override int ExecutionProgress => 40;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.damage = 78;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.SetUpNoUseGraphicItem(true);
            Item.knockBack = 2f;
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ProjectileType<ConferenceCallHeldProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.HJScarlet().borderlandWeapon = true;
        
        }
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {
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
                AddIngredient(ItemID.Shotgun).
                AddIngredient(ItemID.QuadBarrelShotgun).
                AddIngredient(ItemID.Boomstick).
                AddIngredient(ItemID.IllegalGunParts).
                AddIngredient(ItemID.ChlorophyteBar,10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
