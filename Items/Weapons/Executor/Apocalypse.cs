using HJScarletRework.Globals.Executor;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class GrandFinale : ExecutorWeaponClass
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionTime => 15;
        public override void ExSD()
        {
            Item.width = Item.height = 120;
            Item.damage = 1547;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 12f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = RarityType<MatterRarity>();
            Item.shootSpeed = 16;
            Item.shoot = ProjectileType<GrandFinaleProj>();
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ExModifyTooltips(tooltips);
        }
    }
}
