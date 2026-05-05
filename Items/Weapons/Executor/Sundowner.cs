using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class Sundowner : ExecutorWeaponClass
    {
        public override int ExecutionTime => 30;
        public override void ExSD()
        {
            Item.damage = 54;
            Item.SetUpNoUseGraphicItem(true,false);
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.rare = RarityType<SolarRarity>();
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;

        }
        public override bool CanUseItem(Player player) => !player.HasProj<SundownerHeldProj>();
        public override void HoldItem(Player player)
        {
            if (player.HasProj<SundownerHeldProj>(out int projID))
                return;
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, projID, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.netUpdate = true;
        }
    }
}
