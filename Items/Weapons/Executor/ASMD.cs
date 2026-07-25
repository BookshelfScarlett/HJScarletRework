using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using ContinentOfJourney.Items.Rockets;
using ContinentOfJourney.Items.ThrowerWeapons;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class ASMD : ExecutorWeaponClass
    {
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override int ExecutionProgress => 24;
        public static int ExecutionIceBlockCount = 7;
        public override void ExSSD()
        {
            HJScarletList.FrostRarityHashSet.Add(Type);
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Frost);
        }
        public override void ExSD()
        {
            Item.damage = 1200;
            Item.useTime = Item.useAnimation = 50;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem(true);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<ASMDHeldProj>();
            Item.HJScarlet().ExecutionProj = ProjectileType<ASMDExecutionBullet>();
            Item.shootSpeed = 18f;
            Item.knockBack = 3;

        }
        public override bool CanShoot(Player player)
        {
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj<ASMDHeldProj>(out int projID))
                return;
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, projID, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.netUpdate = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FortSniper>().
                AddIngredient<Duality>().
                AddIngredient<ItemFrozenArtifact>(100).
                AddIngredient<FinalBar>(5).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
