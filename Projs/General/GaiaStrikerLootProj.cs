using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using HJScarletRework.Items.Weapons.Executor;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.General
{
    public class GaiaStrikerLootProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Typeless;
        public override string Texture => GetInstance<GaiaStrikerLootBox>().Texture;
        public AnimationStruct Helper = new AnimationStruct(3);
        public bool PlaySound = false;
        public ref float Timer => ref Projectile.ai[0];
        public ref float Oscillation => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = GetSeconds(60);
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.extraUpdates = 0;
            Projectile.scale = 1.5f;
        }
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Charge with { MaxInstances = 1, Pitch = .5f}, Projectile.Center);
            Helper.MaxProgress[0] = 30 * Projectile.MaxUpdates;
            Helper.MaxProgress[1] = 25 * Projectile.MaxUpdates;
        }
        public override void ProjAI()
        {
            if (!Helper.IsDone[0])
            {
                if (Helper.OnAnimationBegin(0))
                    DoOnAnimationBeginState2();
            UpdateFadingParticle();
                if(Projectile.IsMe())
                {
                    Main.mouseItem = new Item();
                    Owner.inventory[58] = new Item();
                }
                Helper.UpdateAniState(0);
            }
            else if (!Helper.IsDone[1])
            {
                Owner.HJScarlet().firstTimeCraftGaia = true;
                Helper.UpdateAniState(1);
                UpdateFadingParticle();
                if (Helper.GetAniProgress(1) > .40f && !PlaySound)
                    DoAnimationStateOneMidSound();
            }
            else
            {
                Timer++;
                Projectile.position += Main.rand.NextVector2Circular(5,5);
                if (Timer > Projectile.MaxUpdates * 30)
                {
                    Projectile.netUpdate = true;
                    Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, ItemType<GaiaStriker>());
                    CreateBloodyExplosion();
                    Projectile.Kill();
                }
            }
            UpdateIdleState();
        }

        public void UpdateIdleState()
        {
            //锤子应当朝向的位置
            Projectile.velocity *= .01f;
            Oscillation += ToRadians(2.5f);
            float anchorPosX = Owner.MountedCenter.X;
            float anchorPosY = Owner.MountedCenter.Y - (60f * MathF.Sin(Oscillation) / 9f) - 300f;
            //递增的值越大，锤子的摆动幅度越大
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(anchorPosX, anchorPosY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.10f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
                //angleToWhat = ToRadians(-90f);
            float angleToWhat = ToRadians(-90);
            //if (Owner.direction < 0)
            Projectile.spriteDirection = Owner.direction;
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);

        }

        public void UpdateFadingParticle()
        {
             float mult = 1;
            if (Main.rand.NextBool(4))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = -Vector2.UnitY;
                    BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(40,5), dir * Main.rand.NextFloat(-12f, 13f), 0.8f, Projectile.rotation, false);
                }
            }
            if (Main.rand.NextBool())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = -Vector2.UnitY;
                    Vector2 pos = Projectile.Center.ToRandCirclePos(30,10) - dir* -5f * mult * Owner.direction;
                    ECSParticle.SmokeParticle(pos, dir * Main.rand.NextFloat(-16f, 17f), RandLerpColor(Color.Red, Color.Black), Main.rand.Next(30, 48), RandRotTwoPi, 0.78f, Main.rand.NextFloat(.75f, 1.1f) * .3f * Projectile.scale, Main.rand.NextBool(), BlendState.NonPremultiplied);
                }
            }
            if (Main.rand.NextBool() && !PlaySound)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = -Vector2.UnitY;
                    Vector2 pos = Projectile.Center.ToRandCirclePos(40, 20) - dir * 10f * mult;
                    ECSParticle.BloodDrop(pos, dir.RotatedByRandom(ToRadians(15)) * Main.rand.NextFloat(17, 21f), RandLerpColor(Color.DarkRed, Color.Black), 120, 1f, Main.rand.NextFloat(.75f, 1.1f) * .15f, 1, true, BlendState.AlphaBlend);
                }
            }

        }
        public void CreateBloodyExplosion()
        {
            //爆开
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                Vector2 vel = RandVelTwoPi(0.9f, 6.4f);
                BloodyMetaball.SpawnParticle(pos, vel, 0.35f, RandRotTwoPi, true);
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                Vector2 vel = RandVelTwoPi(0.9f, 9.4f);
                BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.75f, vel.ToRotation(), false);
                BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.15f, RandRotTwoPi, true);
            }
            for (int i = 0; i < 24; i++)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(3f, 6f), ProjectileType<GaiaStrikerBloodyBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.ai[2] = 2;
            }
            new CrossGlow(Projectile.Center, Color.Red, 40, 1, 0.30f).Spawn();
            new CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, 0.28f).Spawn();
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 60, 80, Projectile.rotation, 0.15f, easingFunc: EaseOutBack);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Explosion with { MaxInstances = 0, Pitch = 0.74f, Volume = .477f }, Projectile.Center);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Toss with { MaxInstances = 0, Pitch = -0.64f, Volume = .577f }, Projectile.Center);
        }


        public void DoOnAnimationBeginState2()
        {
            float angleToWhat = ToRadians(-90);
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = angleToWhat;
            Projectile.Center = Owner.MountedCenter - Vector2.UnitY * 100f;
        }
        public void DoAnimationStateOneMidSound()
        {
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Charge with { MaxInstances = 0, Pitch = 0.4f, Volume = .65f }, Projectile.Center);
            SoundEngine.PlaySound(HJScarletSounds.Gaia_Smash with { MaxInstances = 0, Pitch = -0.4f, Volume = .45f }, Projectile.Center);
            PlaySound = true;
            for (int i = 0; i < 64; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(5);
                Vector2 dir = RandDirTwoPi;
                ECSParticle.SmokeParticle(pos, dir * Main.rand.NextFloat(-16f, 17f), RandLerpColor(Color.Red, Color.Black), Main.rand.Next(30, 48), RandRotTwoPi, 0.68f, Main.rand.NextFloat(.75f, 1.1f) * .4f * Projectile.scale, Main.rand.NextBool(), BlendState.NonPremultiplied);
            }
        }

        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Helper.IsDone[0])
                return false;
            Texture2D tex = Projectile.GetTexture();
            Vector2 ori = tex.ToOrigin();
            float rotFixer =  0;
            SpriteEffects se = SpriteEffects.None;
            Vector2 posBase = Projectile.Center - Main.screenPosition;
            ApplyHammerTrail(tex, rotFixer, ori, se);
            ApplyProjDraw(tex, rotFixer, ori, se, posBase);
            ApplyGlowCenter(se, posBase);
            return false;
        }



        public void ApplyHammerTrail(Texture2D tex, float rotFixer, Vector2 ori, SpriteEffects se)
        {
            if (!Helper.IsDone[1])
                return;
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float rot = -0 + rotFixer;
                float ratios = (1 - (float)i / length);
                int aValue = (int)(Lerp(180, 255, EaseInCubic(ratios)));
                Color c = Color.Lerp(Color.Red, Color.White, ratios) with { A = (byte)aValue } * ratios;
                SB.Draw(tex, pos, null, c, rot, ori, Projectile.scale, se, 0);
            }
        }
        public void ApplyProjDraw(Texture2D tex, float rotFixer, Vector2 ori, SpriteEffects se, Vector2 posBase)
        {
            bool finalClearUp = Helper.IsDone[0] && Helper.IsDone[1];
            float finalClearUpRatios = Timer / (30f * Projectile.MaxUpdates);
            Color edgeDrawColor = Color.Crimson.ToAddColor();
            Color edgeTargetColor = Color.DarkRed.ToAddColor(120);
            Color edgeBoxColor = (finalClearUp) ? Color.Lerp(edgeDrawColor, edgeTargetColor, EaseOutBack(finalClearUpRatios)) : edgeDrawColor;
            float easedProgress = Helper.GetAniProgress(1);
            Vector2 posOffset = Vector2.Zero;
            if (finalClearUp)
            {
                posOffset = Main.rand.NextVector2CircularEdge(10, 10);
                for (int i = 0; i < 16; i++)
                    SB.Draw(tex, posBase + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, edgeBoxColor * EaseInBack(easedProgress), -0 + rotFixer, ori, Projectile.scale, se, 0);
            }
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, posBase + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, edgeBoxColor* EaseInBack(easedProgress), -0 + rotFixer, ori, Projectile.scale, se, 0);
            tex.ApplyMeltShader(Color.Red, 1 - EaseOutCubic(easedProgress));

            Color mainColor = Color.Lerp(Color.White, Color.DarkRed, 0.14f) with { A = 255};
            Color targetColor = Color.DarkRed with { A= 255};
            Color boxColor = (finalClearUp) ? Color.Lerp(mainColor, targetColor, EaseOutBack(finalClearUpRatios)) : mainColor;
            if(finalClearUp)
            SB.Draw(tex, posBase + posOffset, null, boxColor, -0 + rotFixer, ori, Projectile.scale, se, 0);
            SB.Draw(tex, posBase, null, boxColor, -0 + rotFixer, ori, Projectile.scale, se, 0);
            SB.EndShaderArea();
        }
        public void ApplyGlowCenter(SpriteEffects se, Vector2 posBase)
        {
            float progress2 = Helper.GetAniProgress(2);
            if (progress2 > .02f)
            {
                SB.EnterShaderArea();
                Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
                float scale = Projectile.scale * 0.35f;
                float eased = EaseOutBack(progress2);
                Vector2 dir = -PiOver4.ToRotationVector2() * 30f;
                Vector2 glowPos = posBase + dir;
                SB.Draw(glow, glowPos, null, Color.DarkRed * eased, 0, glow.ToOrigin(), scale, se, 0);
                SB.Draw(glow, glowPos, null, Color.Red * eased, 0, glow.ToOrigin(), scale * .95f, se, 0);
                SB.Draw(glow, glowPos, null, Color.White * eased, 0, glow.ToOrigin(), scale * .90f, se, 0);
                SB.EndShaderArea();
            }
        }
    }
}
