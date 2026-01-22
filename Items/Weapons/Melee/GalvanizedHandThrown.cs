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

namespace HJScarletRework.Items.Weapons.Melee
{
    public class GalvanizedHandThrown :ThrownSpearClass
    {
        public override string Texture => GetInstance<GalvanizedHand>().Texture;
        public override void SetStaticDefaults() => Type.ShimmerEach<GalvanizedHand>();
        public override void ExSD()
        {
            Item.damage = 106;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 12f;
            Item.UseSound = HJScarletSounds.Evolution_Thrown with { MaxInstances = 0 };
            Item.shootSpeed = 16;
            Item.shoot = ProjectileType<EvolutionThrownProj>();
            Item.rare = ItemRarityID.Purple;
        }
        public override Color MainTooltipColor => Color.LightGreen;
    }
}
