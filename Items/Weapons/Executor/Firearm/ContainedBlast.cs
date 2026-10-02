using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using HJScarletRework.Core.NetCode;
using ContinentOfJourney.Items.Rockets;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Firearm
{
    public class ContainedBlast : ExecutorWeaponClass
    {
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override int ExecutionProgress => 75;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.FateGolden);
            ScarletItemIDSets.ForceToTacticalExecute[Type] = true;
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 320;
            Item.SetUpNoUseGraphicItem(true, false);
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.useTime = Item.useAnimation = 8;
            Item.shootSpeed = 16f;
            Item.knockBack = 3f;
            Item.shoot = ProjectileType<ContainedBlastHeldProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.HJScarlet().borderlandWeapon = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void HoldItem(Player player)
        {
            if (!player.IsOwnerSide())
                return;
            if (player.HasProj<ContainedBlastHeldProj>(out int projID))
                return;
            Vector2 dir = player.ToMouseVector2();
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, projID, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.netUpdate = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.VortexBeater).
                AddIngredient<TheBlackBox>().
                AddIngredient(ItemID.IllegalGunParts, 15).
                AddIngredient<CubistBar>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
