using ContinentOfJourney.Buffs;
using ContinentOfJourney.Items.Accessories;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class FlybackHandClockMounted : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public ref float ActualTimer => ref Projectile.ai[1];
        public int CacheOwnerHP
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public int ClockFrame = 0;
        private Vector2 NextPos = Vector2.Zero;
        private int PlayBellTime = 0;
        public int EdgeIndex1 = -1;
        public int EdgeIndex2 = -1;
        public bool IsSpawnEdgeProj = false;
        public override void SetDefaults()
        {
            Projectile.height = Projectile.width = 100;
            Projectile.damage = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.85f;
            SpawnMinAndSecond();
            UpdateClockEdge();
            UpdateGeneralParticle();
            UpdateActualAI();
        }

        private void UpdateActualAI()
        {
            Projectile edgeOrb1 = Main.projectile[EdgeIndex1];
            edgeOrb1.localAI[0] = Projectile.Center.X;
            edgeOrb1.localAI[1] = Projectile.Center.Y;

            Projectile edgeOrb2 = Main.projectile[EdgeIndex2];
        
            edgeOrb2.localAI[0] = Projectile.Center.X;
            edgeOrb2.localAI[1] = Projectile.Center.Y;
            Projectile.timeLeft = 2;
            ActualTimer++;
            //敲钟五次
            int totalBellTime = 5;
            if (ActualTimer > 60 && PlayBellTime < totalBellTime)
            {
                ActualTimer = 0;
                SoundEngine.PlaySound(SoundID.Item35 with { Pitch = PlayBellTime * 0.1f + 0.1f }, Owner.Center);
                PlayBellTime++;
            }
            //如果大于一次，我们直接……执行操作。
            if (Owner.JustPressRightClick() && PlayBellTime > 0)
            {
                bool hasCal = ModLoader.HasMod(HJScarletMethods.CalamityMod);
                Owner.Center = Projectile.Center;
                SoundEngine.PlaySound(HJScarletSounds.GrabCharge, Projectile.Center);
                //将损失的血量等拆分，包括魔力也是，然后再根据敲钟次数进行迭代
                int shouldHeal = (Owner.HJScarlet().flybackhandHealthRecord / 2) / totalBellTime * PlayBellTime;
                int shouldRestoreMana = (Owner.HJScarlet().flybackHandManaRecord / 2) / totalBellTime * PlayBellTime;
                //如果玩家启用了灾厄，则全价恢复所有的状态
                Owner.Heal(shouldHeal + shouldHeal * hasCal.ToInt());
                Owner.statMana = Math.Max(shouldRestoreMana + shouldRestoreMana * hasCal.ToInt(), Owner.statManaMax2);
                //假定玩家启用了灾厄，这里强制恢复玩家所有的飞行时间
                if (hasCal)
                    Owner.RefreshMovementAbilities();

                //顺便增加玩家归零针的普攻攻速，5秒左右
                int buffTime = 60 * 2 * PlayBellTime;
                Owner.HJScarlet().flybackhandBuffTime = buffTime;
                Owner.HJScarlet().flybackhandBuffTimeCurrent = buffTime;
                Owner.HJScarlet().flybackhandCloclCD = GetSeconds(1) * PlayBellTime * 2;
                ImmnueDebuffOnNeed();
                //粒子。 
                for (int i = 0; i < 40; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(40f, 40f);
                    new TurbulenceGlowOrb(pos, 2f, RandLerpColor(Color.SkyBlue, Color.AliceBlue), 40, Main.rand.NextFloat(0.1f, 0.12f), Main.rand.NextFloat(TwoPi), true).Spawn();
                }
                //最后杀死射弹
                Projectile.Kill();
            }
        }

        private void ImmnueDebuffOnNeed()
        {

            Owner.buffImmune[BuffID.OnFire] = true;
            Owner.buffImmune[BuffID.OnFire3] = true;
            Owner.buffImmune[BuffID.Frostburn] = true;
            Owner.buffImmune[BuffID.Frostburn2] = true;
            Owner.buffImmune[BuffID.Bleeding] = true;
            Owner.buffImmune[BuffID.BrokenArmor] = true;
            Owner.buffImmune[BuffID.Burning] = true;
            Owner.buffImmune[BuffID.Chilled] = true;
            Owner.buffImmune[BuffID.Confused] = true;
            Owner.buffImmune[BuffID.Cursed] = true;
            Owner.buffImmune[BuffID.Weak] = true;
            Owner.buffImmune[BuffID.Stoned] = true;
            Owner.buffImmune[BuffID.Slow] = true;
            Owner.buffImmune[BuffID.Silenced] = true;
            Owner.buffImmune[BuffID.Poisoned] = true;
            Owner.buffImmune[BuffID.Darkness] = true;

            Owner.buffImmune[BuffID.ShadowFlame] = true;
            Owner.buffImmune[BuffID.Venom] = true;
            Owner.buffImmune[BuffType<DivineFireBuff>()] = true;
            Owner.buffImmune[BuffType<PlagueBuff>()] = true;
        }

        private void UpdateGeneralParticle()
        {
            if (Main.rand.NextBool())
                new StarShape(NextPos + Main.rand.NextVector2Circular(14f, 14f), -Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.4f), RandLerpColor(Color.White, Color.AliceBlue), 0.15f, 30).SpawnToPriority();
        }

        private void UpdateClockEdge()
        {
            Timer += 0.025f;
            //这里需要单独处理，因为你不会想让底图跟时钟一起动的
            NextPos = Projectile.Center + Vector2.UnitY * 20f * (MathF.Sin(Timer) / 9f);
            //泰拉本身是60帧运行的游戏
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 30)
            {
                ClockFrame++;
                Projectile.frameCounter = 0;
            }
            if (ClockFrame > 15)
                ClockFrame = 0;
        
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sharpTears = HJScarletTexture.Specific_Clock.Value;
            //底图
            SB.Draw(HJScarletTexture.Texture_BloomShockwave.Value, Projectile.Center - Main.screenPosition, null, Color.SkyBlue, 0, HJScarletTexture.Texture_BloomShockwave.Origin, 0.215f * Projectile.scale, SpriteEffects.None, 0);
            SB.End();
            //光圈，叠加
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SB.Draw(HJScarletTexture.Texture_SoftCircleEdge.Value, Projectile.Center - Main.screenPosition, null, Color.SkyBlue with { A = 100 }, 0, HJScarletTexture.Texture_SoftCircleEdge.Origin, 0.7f * Projectile.scale, SpriteEffects.None, 0);
            SB.End();
            SB.BeginDefault();
            //时钟处理
            Rectangle frames = sharpTears.Frame(1, 16, 0, ClockFrame);
            Vector2 origin = frames.Size() / 2;
            Vector2 targetSize = Projectile.scale * new Vector2(1f, 1f);
            Vector2 pos = NextPos - Main.screenPosition ;
            for (int i = 0; i < 8; i++)
                SB.Draw(sharpTears, pos + ToRadians( i * 60f).ToRotationVector2() * 1.2f, frames, Color.White with { A = 0}, 0, origin, targetSize, SpriteEffects.None, 0);
            SB.Draw(sharpTears, pos, frames, Color.White, 0, origin, targetSize, SpriteEffects.None, 0);
            return false;
        }
        private void SpawnMinAndSecond()
        {
            //发射两枚用于描边的射弹，同时也会进行一定程度的后续处理
            int projType = ProjectileType<FlybackHandEdgeProj>();
            if (IsSpawnEdgeProj)
                return;
            Vector2 dir = Vector2.UnitY.RotatedByRandom(TwoPi);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.MountedCenter, dir * 12f, projType, 0, 0, Owner.whoAmI);
            proj.localAI[0] = Projectile.Center.X;
            proj.localAI[1] = Projectile.Center.Y;
            proj.ai[1] = Projectile.whoAmI;
            EdgeIndex1 = proj.whoAmI;

            Projectile proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.MountedCenter, dir * -12f, projType, 0, 0, Owner.whoAmI);
            proj2.rotation = Main.rand.Next(1, 61) * ToRadians(6);
            proj2.localAI[0] = Projectile.Center.X;
            proj2.localAI[1] = Projectile.Center.Y;
            proj2.ai[1] = Projectile.whoAmI;
            EdgeIndex2 = proj2.whoAmI;
            IsSpawnEdgeProj = true;
        }
    }
}
