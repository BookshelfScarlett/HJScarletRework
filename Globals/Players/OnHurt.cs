using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
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
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (ShadowCastAcc && Main.rand.NextBool(10))
            {
                return false;
            }
            //星月夜的自活
            if (desterrennacht && desterranRespawnChargeTimer == 0)
            {
                desterranRespawnChargeTimer = GetSeconds(90);
                desterrannachtImmortalTime = 3;
                Player.statLife = Player.statLifeMax2 / 2;
                SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { MaxInstances = 0, Pitch = 0.5f }, Player.Center);
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitY * Main.rand.NextFloat(-11f, -6f) + Vector2.UnitX * Main.rand.NextFloat(-10f, 11f);
                    Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, 0, 1, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitY * Main.rand.NextFloat(-15f, -6f);
                    Vector2 vel = Vector2.UnitY * Main.rand.NextFloat(-8f, -1f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, 0, 1, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
                }
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if(desterrennacht && desterrannachtImmortalTime > 0)
            {
                SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { MaxInstances = 0, Pitch = 0.9f - desterrannachtImmortalTime * 0.1f }, Player.Center);
                desterrannachtImmortalTime--;
                Player.GetImmnue(ImmunityCooldownID.General, 60, true);
                for (int i = 0; i < 24; i++)
                    new TurbulenceGlowOrb(Player.Center.ToRandCirclePos(20f), 1.2f, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 80, 0.1f, RandRotTwoPi).Spawn();
                for (int i = 0;i<20;i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                    Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-5f, 6f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, 0, 1, .1f * Main.rand.NextFloat(0.65f,0.75f)).Spawn();
                }

                return true;
            }
            return base.ConsumableDodge(info);
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            ////星月夜的自活成功后的闪避，这里应该需要考虑一下……用什么钩子
            return base.FreeDodge(info);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            float totalProjDamageModify = 1f;
            if (protectorShiver)
                totalProjDamageModify -= 0.12f;
            modifiers.FinalDamage *= totalProjDamageModify;
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitByProjectile(proj, ref modifiers);
            float totalProjDamageModify = 1f;
            if (protectorShiver)
                totalProjDamageModify -= 0.12f;
            modifiers.FinalDamage *= totalProjDamageModify;
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            base.OnHitByNPC(npc, hurtInfo);
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            base.OnHitByProjectile(proj, hurtInfo);
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if(PreciousTargetAcc && info.Damage > 5)
            {
                PreciousTargetCrtis -= 300;
                if (PreciousTargetCrtis < PreciousCritsMin)
                    PreciousTargetCrtis = PreciousCritsMin;
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            float finalDamageModiflication = 1f;
            if (Player.HasBuff<BlackKeyExecutionBuff>())
            {
                finalDamageModiflication -= blackKeyDefenseBuff;
                Player.buffImmune[BuffType<BlackKeyExecutionBuff>()] = true;
            }
            modifiers.FinalDamage *= finalDamageModiflication;
            if (blackKeyHeal != 0)
            {
                Player.Heal(blackKeyHeal);
                SoundEngine.PlaySound(HJScarletSounds.Heal_Minor with { Volume = 0.75f }, Player.Center);
                //一些粒子
                new CrossGlow(Player.Center, Color.RoyalBlue, 40, 1, 0.12f).Spawn();
                new CrossGlow(Player.Center, Color.AliceBlue, 40, 1, 0.08f).Spawn();

                for (int i = 0; i < 10; i++)
                {
                    new StarShape(Player.ToRandRec() + Vector2.UnitY * 10f, -Vector2.UnitY, Color.RoyalBlue, 0.25f, 40).Spawn();
                }
                for (int i = 0; i < 8; i++)
                {
                    new KiraStar(Player.ToRandRec() + Vector2.UnitY * 10f, -Vector2.UnitY, Color.RoyalBlue, 40, 0, 1, .024f, useAlt: true).Spawn();
                }
                for (int i = 0; i < 15; i++)
                {
                    new HRShinyOrb(Player.ToRandRec() + Vector2.UnitY * 10f, -Vector2.UnitY, Color.RoyalBlue, 40, 0, 1, .0824f).Spawn();
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 spawnPos = Player.Center + Vector2.UnitY * (Player.height / 2 + 5) + Vector2.UnitY * Main.rand.NextFloat(-11f, -6f) + Vector2.UnitX * Main.rand.NextFloat(-10f, 11f);
                    Vector2 vel = Vector2.UnitY * Main.rand.NextFloat(-6f, -1f);
                    new HRShinyOrb(spawnPos, vel, RandLerpColor(Color.RoyalBlue, Color.AliceBlue), 40, 0, 1, .1f * Main.rand.NextFloat(0.65f, 0.75f)).Spawn();
                }
            }
        }
    }
}
