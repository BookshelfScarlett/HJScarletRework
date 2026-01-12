using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class EvolutionThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<Evolution>().Texture;
        public override void SetStaticDefaults() => Type.ShimmerEach<Evolution>();
        public override void ExSD()
        {
            Item.damage = 106;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 12f;
            Item.UseSound = HJScarletSounds.Mana_Toss2 with { MaxInstances = 0};
            Item.shootSpeed = 16;
            Item.shoot = ProjectileType<EvolutionThrownProj>();
            Item.rare = ItemRarityID.Purple;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string path = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.Tooltip");
            tooltips.ReplaceAllTooltip(path, Color.LightGreen);

        }
    }
}
