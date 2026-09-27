using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Magic;
using HJScarletRework.Projs.NPCs.Enemy;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Magic
{
    public class BrimstoneHeartHeldProj : HJScarletFloatingBook
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override int OriginalItemID => ItemType<BrimstoneHeart>();
        public override string Texture => GetInstance<BrimstoneHeart>().Texture;
        public List<Vector2> CenterPosList = [];
        private Vector2 TopLeftPoint = new Vector2(0, 0);
        private Vector2 TopRightPoint = new Vector2(30, -100);
        private Vector2 BottomLeftPoint = new Vector2(30, 100);
        private Vector2 BottomRightPoint = new Vector2(0, 0);
        private Vector4 RandValueSummary = new Vector4();
        public float RandOmega = 0;
        public int IFrameContinueTime = 0;

        public List<Vector2> OldPos = [];
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
            RandValueSummary = new Vector4(Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(0.7f, 1.1f));
            float maxPoints = 60;
            for (int i = 0; i < maxPoints; i++)
            {
                float progress = i / maxPoints;
                Vector2 finalPos = Vector2.CatmullRom(TopLeftPoint, TopRightPoint, BottomLeftPoint, BottomRightPoint, progress);
                finalPos.X *= 0.9f;
                finalPos.Y *= 0.18f;
                CenterPosList.Add(finalPos.RotatedBy(PiOver2));
            }
            RandOmega = RandRotTwoPi;
            if (!Owner.HJScarlet().brimstoneHeartKilling && Projectile.IsMe())
            {
                Owner.AddBuff(BuffID.Bleeding, GetSeconds(10));
                Owner.HJScarlet().iFrameHurtAdd += GetSeconds(3);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<SuicideKnifeInvisProj>(), 999, 0, Owner.whoAmI);
            }
            ScarletSound(HJScarletSounds.Gaia_Explosion, Projectile.Center, .4f, 1, .2f);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 20, 20, RandRotTwoPi);
            Vector2 dir = Owner.Center.GetNormalVector2(Main.MouseWorld);
            if (Projectile.IsMe())
            {
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = dir.ToRandVelocity(ToRadians(15), 0.9f, 5.4f);
                    BloodyMetaball.SpawnParticle(pos, vel, 0.15f, RandRotTwoPi, true);
                }
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = dir.ToRandVelocity(ToRadians(15), 0.9f, 6.4f);
                    BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.45f, vel.ToRotation(), false);
                    BloodyMetaball.SpawnParticle(pos, vel * 3.7f, 0.25f, RandRotTwoPi, true);
                }
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(30);
                    Vector2 vel = (dir).ToRandVelocity(ToRadians(30), 16f, 32f);
                    float scale = Main.rand.NextFloat(0.95f, 1.175f) * 0.1f;
                    Color c = RandLerpColor(Color.DarkRed, Color.Black);
                    ECSParticle.BloodDrop(pos, vel, c, Main.rand.Next(40, 90), 1, scale, 1, blendstate: BlendState.AlphaBlend);

                }
            }
        }
        public override void HandleLeftAttack()
        {
            if (!Projectile.IsMe())
                return;
            if (Timer < AttackSpeed)
                return;
            if (!Owner.CheckMana(Owner.HeldItem, (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                return;

            ScarletSound(HJScarletSounds.Gaia_Staff, Projectile.Center, .75f, 0, -.19f, .1f);
            ScarletSound(HJScarletSounds.Gaia_Explosion, Projectile.Center, .25f, 0, .9f, .1f);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 10f, 10, RandRotTwoPi);
            Timer = 0;
            Vector2 dir = Owner.Center.GetNormalVector2(Main.MouseWorld);
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                Vector2 vel = dir.ToRandVelocity(ToRadians(15), 0.9f, 5.4f);
                BloodyMetaball.SpawnParticle(pos, vel, 0.20f, RandRotTwoPi, true);
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                Vector2 vel = dir.ToRandVelocity(ToRadians(15), 0.9f, 6.4f);
                BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.55f, vel.ToRotation(), false);
                BloodyMetaball.SpawnParticle(pos, vel * 3.7f, 0.35f, RandRotTwoPi, true);
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(30);
                Vector2 vel = (dir).ToRandVelocity(ToRadians(30), 16f, 32f);
                float scale = Main.rand.NextFloat(0.95f, 1.175f) * 0.15f;
                Color c = RandLerpColor(Color.DarkRed, Color.Black);
                ECSParticle.BloodDrop(pos, vel, c, Main.rand.Next(40, 90), 1, scale, 1, blendstate: BlendState.AlphaBlend);

            }
            float statLifeRatios = Clamp((float)Owner.statLife / Owner.statLifeMax2, 0, 1);
            float damageMulter = Lerp(1f, 2.5f, (1 - statLifeRatios));
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(4);
                Vector2 vel = Projectile.SafeDir().RotateRandom(ToRadians(15f)).ToSafeNormalize() * Main.rand.NextFloat(0.9f, 1.1f) * 16f;
                Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), pos, vel, ProjectileType<BrimstoneHeartFireball>(), (int)(Projectile.damage * damageMulter), Projectile.knockBack, Owner.whoAmI);
            }
        }

        protected override void GlobalReset()
        {
            if (Main.rand.NextBool(12))
                ECSParticle.Stain(Projectile.Center.ToRandCirclePos(30), -Vector2.UnitY, RandLerpColor(Color.DarkRed, Color.Crimson), 30, 1, PiOver2, 0.28f, blendstate: BlendState.AlphaBlend);
            base.GlobalReset();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //基础绘制
            DrawAttack();
            base.PreDraw(ref lightColor);
            DrawHeartbeat();
            DrawFlowTrail();
            return false;
        }

        public void DrawAttack()
        {
            SB.EnterShaderArea();
            float progress2 = EaseOutBack(Helper.GetAniProgress(1));
            Texture2D tex = HJScarletTexture.Texture_Spirite.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Main.GlobalTimeWrappedHourly * 1.2f + Projectile.rotation;
            float ringScale = progress2 * .62f * Projectile.scale;
            SB.FastDraw(tex, pos, Color.DarkRed, rot, tex.Size() / 2f, ringScale, 0);
            SB.FastDraw(tex, pos, Color.Crimson, rot, tex.Size() / 2f, ringScale * .96f, 0);
            SB.EndShaderArea();
        }

        public void DrawFlowTrail()
        {
            SB.EnterShaderArea(BlendState.AlphaBlend);
            Vector4 vector4 = new(0.2f, 0.2f, 0.1f, 0.6f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 1.79f * RandValueSummary.X), new Vector2(1f, 0.985f), Color.Crimson);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            TrailFunc(texture2, Color.White, 4f);

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 2.79f * RandValueSummary.Y), new Vector2(1.23f, 0.975f), Color.DarkRed);
            texture2 = HJScarletTexture.Noise_Misc.Value;
            TrailFunc(texture2, Color.White * 0.42f, 4f);
            texture2 = HJScarletTexture.Noise_Smoke.Value;

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.79f * RandValueSummary.Z), new Vector2(1.2f, 1.34f), Color.Red);
            TrailFunc(texture2, Color.White * 0.42f, 8f);
            TrailFunc(texture2, Color.White * 0.62f, 4f);
            SB.EndShaderArea();

        }

        public void DrawHeartbeat()
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation - RotFixer + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 origin = tex.Size() / 2;
            Vector2 realDrawPos = drawPos;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float t = (float)(Math.Sin(Main.timeForVisualEffects));
            // ---- 心跳计算 ----
            // 周期（帧），越小跳得越快
            const float beatCycle = 60f;
            // 心跳时间相位
            float phase = Main.GlobalTimeWrappedHourly * 60f % beatCycle / beatCycle; // 0~1
                                                                                      // 双拍强度：第一拍在 0.05 附近，第二拍在 0.25 附近
            float pulse = GetHeartbeatPulse(phase);
            // 最大缩放增幅（例如 0.18 = 最多放大 18%）
            const float maxPulse = 0.25f;
            float beatScale = 1f + pulse * maxPulse;
            SB.FastDraw(tex, realDrawPos, Color.Crimson.ToAddColor(150) * .75f * pulse, rotation, origin, Projectile.scale * beatScale, se);
        }

        public void TrailFunc(Texture2D tex, Color c, float mult)
        {
            List<ScarletVertex> vertices = new List<ScarletVertex>();
            Vector2 dir = (-Vector2.UnitY) * 12f * mult;
            for (int i = 0; i < CenterPosList.Count; i++)
            {
                float progress = (float)i / CenterPosList.Count;
                Vector2 pos = Projectile.Center - Main.screenPosition - Vector2.UnitY * 15 - Vector2.UnitX;
                Vector2 posHead = CenterPosList[i] + pos;
                Vector2 posSrc = CenterPosList[i] + pos + dir * Projectile.Opacity;
                vertices.Add(new ScarletVertex(posHead, c, new Vector3(progress, 0, 0)));
                vertices.Add(new ScarletVertex(posSrc, c, new Vector3(progress, 1, 0)));
            }
            if (vertices.Count < 3)
                return;
            GD.Textures[0] = tex;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }

        // 心跳曲线：两次衰减的峰值（第一拍强，第二拍弱）
        static float GetHeartbeatPulse(float p)
        {
            // 第一拍：中心在 0.05，宽度 0.08
            float p1 = Pulse(p, 0.05f, 0.08f);
            // 第二拍：中心在 0.22，宽度 0.10，强度稍弱
            float p2 = Pulse(p, 0.22f, 0.10f) * 0.6f;
            return MathHelper.Clamp(p1 + p2, 0f, 1f);
        }

        // 在 [center-halfW, center+halfW] 内产生一个正弦脉冲，超出返回 0
        static float Pulse(float p, float center, float halfW)
        {
            float d = Math.Abs(p - center);
            if (d >= halfW) return 0f;
            return (float)Math.Sin((1f - d / halfW) * MathHelper.PiOver2);
        }
    }
}
