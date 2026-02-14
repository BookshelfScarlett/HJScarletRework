using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class EvolutionThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<Evolution>().Texture;
        public override void ExSD()
        {
            Item.damage = 313;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 12f;
            Item.UseSound = HJScarletSounds.Evolution_Thrown with { MaxInstances = 0};
            Item.shootSpeed = 16;
            Item.shoot = ProjectileType<EvolutionThrownProj>();
            Item.rare = RarityType<LivingRarity>();
        }
        public override Color MainTooltipColor => Color.LightGreen;
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                LivingRarity.DrawRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
    }
}
