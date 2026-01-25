using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
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
