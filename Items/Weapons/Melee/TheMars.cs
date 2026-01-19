using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class TheMars : ThrownSpearClass
    {
        public override void ExSD()
        {
            Item.damage = 104;
            Item.useTime = Item.useAnimation = 38;
            Item.knockBack = 12f;
            Item.UseSound = HJScarletSounds.TheMars_Toss with { MaxInstances = 0 , Volume = 0.85f};
            //投射出去之后会被减速
            Item.shootSpeed = 24f;
            Item.shoot = ProjectileType<TheMarsProj>();
            Item.rare = ItemRarityID.Red;
        }
        public override Color MainTooltipColor => Color.LightGray;
    }
}
