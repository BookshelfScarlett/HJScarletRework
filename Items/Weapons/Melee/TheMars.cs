using ContinentOfJourney;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class TheMars : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override void ExSD()
        {
            Item.damage = 164;
            Item.useTime = Item.useAnimation = 35;
            Item.knockBack = 1f;
            Item.UseSound = HJScarletSounds.TheMars_Toss with { MaxInstances = 0 , Volume = 0.85f};
            //投射出去之后会被减速
            Item.shootSpeed = 24f;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ProjectileType<TheMarsProj>();
            Item.rare = ItemRarityID.Red;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (DownedBossSystem.downedBarrier)
                damage *= 1.15f;
        }
        public override Color MainTooltipColor => Color.LightGray;
    }
}
