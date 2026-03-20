using HJScarletRework.Executor;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class Apocalypse : ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1.0f;
        public override int ExecutionTime => 15;
        public override void ExSD()
        {
            Item.width = Item.height = 120;
            Item.damage = 1547;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 10;
            Item.knockBack = 12f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = RarityType<MatterRarity>();
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ExModifyTooltips(tooltips);
        }
    }
}
