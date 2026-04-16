using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            //确保武器起码有伤害
            if (PreciousTargetAcc && item.damage > 0)
            {
                crit = PreciousTargetCrtis;
                int limitedCrit = PreciousAimAcc ? 125 : 115;
                if (PreciousTargetCrtis > limitedCrit)
                    PreciousTargetCrtis = limitedCrit;
            }
        }
        //潜在的问题是，这里实际上有可能因为写法差异导致出现多乘区
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            if(redDragonKnight && item.DamageType != ExecutorDamageClass.Instance && !item.DamageType.CountsAsClass<GenericDamageClass>() && item.damage > 0)
            {
                damage = StatModifier.Default;
                float ratios = (Player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(item.damage) - (float)item.damage) / (float)item.damage;
                damage *= (1f + ratios);
                if (item.consumable && item.DamageType.CountsAsClass<RangedDamageClass>())
                    damage *= 1.10f;
            }
            base.ModifyWeaponDamage(item, ref damage);
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(runeWizardExecutor && item.DamageType == ExecutorDamageClass.Instance)
            {
                int count = Main.rand.Next(2, 4);
                SoundEngine.PlaySound(SoundID.Item43 with { MaxInstances = 1, Pitch = 0.4f, PitchVariance = 0.2f, Volume = 0.75f }, Player.Center);
                for (int i = 0; i < count; i++)
                {
                    Vector2 vel = Player.ToMouseVector2().ToRandVelocity(ToRadians(15f), 11f, 13f);
                    Vector2 spawnPos = Player.Center - Player.ToMouseVector2() * Main.rand.NextFloat(150f, 190f);
                    Projectile proj = Projectile.NewProjectileDirect(source, spawnPos.ToRandCirclePosEdge(50f), vel, ProjectileType<RuneWizardProj>(), (damage / 2) / count, 1f, Player.whoAmI);
                }
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

    }
}
