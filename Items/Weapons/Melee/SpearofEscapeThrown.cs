using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SpearofEscapeThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<SpearOfEscape>().Texture;
        public override void SetStaticDefaults() => Type.ShimmerEach<SpearOfEscape>();
        public override void ExSD()
        {
            Item.damage = 106;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 12f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shootSpeed = 16;
        }
    }
}
