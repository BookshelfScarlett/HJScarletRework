using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class ColdSakura : ThrownSpearClass
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override bool HasLegendary => true;
        public override void ExSD()
        {
            Item.damage = 100;
            Item.knockBack = 12f;
            Item.useTime = Item.useAnimation = 30;
            Item.rare = RarityType<SakuraRarity>();
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }
        public override Color MainTooltipColor => base.MainTooltipColor;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
    }
}
