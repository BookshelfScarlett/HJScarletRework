using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public abstract class PreHardmodeOreRapier : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem();
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.useTime = Item.useAnimation = 25;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 11f;
        }
    }
    public class PlatinumRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<PlatinumRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();
            Item.shoot = ProjectileType<PlatinumRapierThrownProj>();
            Item.damage = 36;
        }
    }
    public class GoldRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<GoldRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();
            Item.shoot = ProjectileType<GoldRapierThrownProj>();
            Item.damage = 32;
        }
    }
    public class SilverRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<SilverRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();
            Item.shoot = ProjectileType<SilverRapierThrownProj>();
            Item.damage = 28;
        }
    }
    public class TungstenRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<TungstenRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();
            Item.shoot = ProjectileType<TungstenRapierThrownProj>();
            Item.damage = 24;
        }
    }
    public class LeadRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<LeadRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();
            Item.shoot = ProjectileType<LeadRapierThrownProj>();
            Item.damage = 24;
        }
    }
    public class IronRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<IronRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();

            Item.shoot = ProjectileType<IronRapierThrownProj>();
            Item.damage = 24;
        }
    }
    public class TinRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<TinRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();
            Item.shoot = ProjectileType<TinRapierThrownProj>();
            Item.damage = 24;
        }
    }
    public class CopperRapierThrown : PreHardmodeOreRapier
    {
        public override string Texture => GetInstance<CopperRapier>().Texture;
        public override void ExSD()
        {
            base.ExSD();
            Item.shoot = ProjectileType<CopperRapierThrownProj>();
            Item.damage = 24;
        }
    }
}
