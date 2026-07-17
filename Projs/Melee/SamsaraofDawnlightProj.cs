using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class SamsaraofDawnlightProj : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => GetInstance<SamsaraofDawnlightAlter>().Texture;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(30, 3);
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true; Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 1;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (AttackState == State.TurnintoSignarity)
                hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(320, 320));
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //overWiresUI.Add(index);
            behindNPCs.Add(index);
        }
        public virtual Color trailColor => new(255, 255, 255, 0);
        public override bool? CanCutTiles()
        {
            return true;
        }
        public void drawStar(Vector2 off, float scale, float starScale, Color c)
        {
            Vector2 pos = Projectile.Center + off * scale - Main.screenPosition;
            SB.Draw(HJScarletTexture.Particle_SharpTear, pos, null, c, 0, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f), SpriteEffects.None, 0);
            SB.Draw(HJScarletTexture.Particle_SharpTear, pos, null, c, (float)Math.PI / 2, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f), SpriteEffects.None, 0);

            SB.Draw(HJScarletTexture.Particle_SharpTear, pos, null, Color.White, 0, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f) * 0.66f, SpriteEffects.None, 0);
            SB.Draw(HJScarletTexture.Particle_SharpTear, pos, null, Color.White, PiOver2, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f) * 0.66f, SpriteEffects.None, 0);
        }
        public void ApplyTrailAlt(Texture2D tex, Color color, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            float laserLength = 150;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -150);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(1.13f);
            shader.Parameters["uFadeinLength"].SetValue(0.052f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 - Projectile.SafeDir() * 10f + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight * 3.92f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (AttackState == State.TurnintoSignarity)
            {
                PixelatedRenderManager.BeginDrawProj = false;
                DrawSignarity();
            }
            else
            {
                DrawPlate();
                PixelatedRenderManager.BeginDrawProj = true;
            }
            return false;
        }

        public void DrawPlate()
        {
            Projectile.localAI[0]++;
            Texture2D texture = Projectile.GetTexture();
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(texture, Projectile.Center - Main.screenPosition + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, Color.White.ToAddColor(), Projectile.rotation + DrawRotation, texture.ToOrigin(), Projectile.scale, 0, 0);
            }
            SB.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + DrawRotation, texture.ToOrigin(), Projectile.scale, 0, 0);

        }
        public void DrawSignarity()
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float time = Projectile.ai[1];
            float fac1 = Utils.Clamp(time / 90, 0f, 1f);
            fac1 = (float)(0.5f - 0.5f * Math.Cos(fac1 * Math.PI));
            if (time > 120)
                fac1 = 1f - (time - 120) / 20;

            float colorLerp = Utils.Clamp(time / 180, 0f, 1f);
            colorLerp = 0.5f - 0.5f * (float)Math.Cos(colorLerp * Math.PI);
            Color showColor = Color.Lerp(new Color(255, 208, 133, 0), new Color(212, 187, 255, 0), colorLerp);
            Texture2D glow1 = Request<Texture2D>("ContinentOfJourney/Images/Glow1").Value;
            float alpha = Utils.Clamp(time / 10, 0, 1);
            SB.Draw(glow1, drawPos, null, showColor * alpha * 0.7f, Projectile.rotation, new Vector2(100, 100), fac1 * 2.5f + (float)Math.Sin(Main.timeForVisualEffects) * 0.3f, SpriteEffects.None, 0);
            Texture2D giantLight = Request<Texture2D>("ContinentOfJourney/Images/GiantLight_Alt").Value;
            Vector2 gianltLightOri = new Vector2(500);
            SB.Draw(giantLight, drawPos, null, showColor * alpha * 0.3f, 0f, gianltLightOri, fac1, SpriteEffects.None, 0);
            SB.Draw(giantLight, drawPos, null, Color.Black * alpha, 0f, gianltLightOri, fac1 * 0.4f, SpriteEffects.None, 0);
            float vtime = (float)Main.timeForVisualEffects;
            for (int i = 0; i < 10; i++)
            {
                drawStar(
                    new Vector2(71, 13).RotatedBy((vtime + i * 233) / 57) + new Vector2(51, 57).RotatedBy((vtime + i * 71) / 37),
                    fac1 * 0.66f, fac1 * (0.5f + 0.5f * (float)Math.Sin((Main.timeForVisualEffects + i * 131) / 311)) * 1.2f,
                    showColor);
            }
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            if (AttackState == State.TurnintoSignarity)
                return;
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            ApplyTrailAlt(HJScarletTexture.Trail_ManaStreakTiny.Value, Color.DarkGray);
            ApplyTrailAlt(HJScarletTexture.Trail_FadedStreak.Value, Color.Gray, 10);
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.WhiteSmoke, 28);
            HJScarletMethods.EndShaderAreaPixel();
        }
        private int ai2
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public enum State
        {
            Shoot,
            TurnintoSignarity
        }
        public State AttackState
        {
            get => (State)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public ref float AltTimer => ref Projectile.ai[1];
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Shoot && Timer < 62)
            {
                Timer = 62;
                Projectile.netUpdate = true;

            }
        }
        public override void AI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.TurnintoSignarity:
                    DoTurnintoSignarity();
                    break;
            }
        }
        public float DrawRotation = 0;
        public void DoShoot()
        {
            if (Main.rand.NextBool(3))
                ECSParticle.LightntingGlow(Projectile.ToRandRec() - Projectile.SafeDir() * 10f, Projectile.velocity / 8f, RandLerpColor(Color.White, Color.Silver), 40, 1, 0.45f);
            if (Main.rand.NextBool(2))
                ECSParticle.ShinyCrossStarECS(Projectile.ToRandRec() - Projectile.SafeDir() * 10f, Projectile.velocity / 8f, RandLerpColor(Color.White, Color.Silver), 40, 1, 0.85f * Main.rand.NextFloat(0.9f, 1.1f), 0.2f);

            Projectile.rotation = Projectile.velocity.ToRotation();
            DrawRotation += 0.2f;
            Timer++;
            if (Timer <= 31)
                return;
            Projectile.velocity *= 0.9f;
            if (Projectile.ai[0] <= 44)
                return;
            if (!Projectile.IsMe())
                return;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDir(), ProjectileType<SamsaraofDawnlightSun>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.Center.X, Projectile.Center.Y);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Projectile.SafeDir(), ProjectileType<SamsaraofDawnlightMoon>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.Center.X, Projectile.Center.Y);
            SoundEngine.PlaySound(HJScarletSounds.Misc_AirFlowAlt with { Pitch = 0.5f }, Projectile.Center);
            SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { Pitch = 0.5f }, Projectile.Center);
            AttackState = State.TurnintoSignarity;
            Projectile.netUpdate = true;
        }

        public void DoTurnintoSignarity()
        {
            Projectile.velocity *= 0.2f;
            Projectile.ai[1]++;
            Projectile.hide = true;
            ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(60), Vector2.UnitX * Main.rand.NextFloat(0.9f, 1.3f), RandLerpColor(Color.Silver, Color.White), 40, 1, 0.45f, 0.2f);
            if (Projectile.ai[1] == 20)
            {
                for (int i = 0; i < 30; i++)
                {
                    new TurbulenceGlowOrb(Projectile.ToRandRec(), Main.rand.NextFloat(0.9f, 1.1f) * 7.1f, RandLerpColor(Color.White, Color.Silver), 120, Projectile.scale * Main.rand.NextFloat(0.9f, 1.1f) * 0.32f, RandRotTwoPi).Spawn();
                }
            }
            if (Projectile.ai[1] % 20 == 0)
            {
                new TurbulenceGlowOrb(Projectile.ToRandRec(), 7f, RandLerpColor(Color.White, Color.Silver), 120, Projectile.scale * Main.rand.NextFloat(0.9f, 1.1f) * 0.52f, RandRotTwoPi).Spawn();
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                        Projectile.Center + new Vector2(Main.rand.NextFloat(-120, 120), Main.rand.NextFloat(-120, 120)),
                        new Vector2(Main.rand.NextFloat(4f, 12f), 0).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)),
                        ProjectileType<SamsaraofDawnlightBolt>(), Projectile.damage / 4, Projectile.knockBack / 2, Projectile.owner);
                }
            }

            if (Projectile.ai[1] > 140)
            {
                for (int i = 0; i < 40; i++)
                {

                    Vector2 speed = (TwoPi / 40f * i).ToRotationVector2() * Main.rand.NextFloat(2, 12);
                    Vector2 pos = Projectile.Center - new Vector2(3, 3);
                    for (int j = 0; j < 3; j++)
                    {
                        ECSParticle.ShinyCrossStarECS(pos.ToRandCirclePos(3), speed.ToSafeNormalize() * Main.rand.NextFloat(0.8f, 13f), RandLerpColor(Color.WhiteSmoke, Color.Silver), Main.rand.Next(35, 45), 1, Main.rand.NextFloat(0.8f, 1.2f) * 0.60f, 0.2f);
                    }
                    ECSParticle.LightntingGlow(pos, speed, RandLerpColor(Color.WhiteSmoke, Color.Silver), 40, 1, 0.45f * Main.rand.NextFloat(0.9f, 1.1f), 6);
                }
                SoundEngine.PlaySound(HJScarletSounds.Frosthammer_SnowCharge with { MaxInstances = 0, Pitch = 0.5f });
                SoundEngine.PlaySound(HJScarletSounds.Hammer_ShootAlt with { MaxInstances = 0, Pitch = 0.5f });
                Projectile.Kill();
            }
        }
    }
    public class SamsaraofDawnlightBolt : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true; Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.netUpdate = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 1;
        }
        public override bool? CanCutTiles()
        {
            return true;
        }
        public override bool PreKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            int amount = 6;
            Color color = new Color(255, 255, 255, 0);
            for (int i = 0; i < amount; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustType<ContinentOfJourney.Dusts.StarDust>());
                Main.dust[d].scale = 2f;
                Main.dust[d].velocity = new Vector2(4, 0).RotatedBy(TwoPi / amount * i);
                Main.dust[d].color = color;
                Main.dust[d].noGravity = true;
            }
            return true;
        }
        //public override bool PreDraw(ref Color lightColor)
        //{
        //    SB.EnterShaderArea();
        //    DrawNebulaTrail(Color.MediumPurple, 14f);
        //    DrawNebulaTrail(Color.LightPink with { A = 50 }, 12.2f);
        //    DrawNebulaTrail(Color.White with { A = 100 }, 10.8f);
        //    SB.EnterShaderArea();
        //    if (Projectile.oldPos.Length > 12)
        //    {
        //        for (int k = 0; k < 12; k += 3)
        //        {
        //            Vector2 dir = Projectile.oldPos[k] - Projectile.oldPos[k + 1];
        //            DrawStar(Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition, dir.ToRotation(), Color.Lerp(BinaryStarsProj.TrailColor, Color.Purple, (float)k / 16));
        //        }
        //    }
        //    SB.EndShaderArea();

        //    return false;
        //}

        public void DrawStar(Vector2 drawPos, float rot, Color starColor)
        {
            Texture2D sharpTears = HJScarletTexture.Particle_HRStar.Value;
            Vector2 targetSize = 0.36f * Projectile.scale * new Vector2(1.2f, 0.25f);
            SB.Draw(sharpTears, drawPos, null, starColor, rot, sharpTears.Size() / 2, targetSize, SpriteEffects.None, 0);
            SB.Draw(sharpTears, drawPos, null, Color.White with { A = 150 }, rot, sharpTears.Size() / 2, targetSize * 0.5f, SpriteEffects.None, 0);
        }
        public void DrawNebulaTrail(Color trailColor, float height)
        {
            float laserLength = 50;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_ManaStreak.Size);
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_ManaStreak.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -150);
            shader.Parameters["uColor"].SetValue(trailColor.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(1.74f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();
            DrawSetting sets = new(HJScarletTexture.Trail_ManaStreak.Value);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2;
                date.Add(new(listPos, Color.White, new(0, height * 0.75f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Projectile.scale = (Projectile.ai[1] < 10) ? Projectile.ai[1] * 0.1f : 1;
            Vector2 drawOrigin = Vector2.Zero;
            Color color = new Color(255, 255, 255, 0) * 0.5f;

            Color color1;
            Color color2;
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = Vector2.Lerp(Projectile.oldPos[k], Projectile.oldPos[0], 0.2f) - Main.screenPosition + Projectile.Size / 2;
                color1 = color * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color2 = new Color(255, 255, 255, 0) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                for (int i = 0; i < 2; i++)
                {
                    SB.Draw(HJScarletTexture.Particle_SharpTear, drawPos - Projectile.SafeDir() * i * 10, null,
                        color1 * .5f, Projectile.oldRot[k] + (float)Math.PI / 2, new Vector2(36, 36), Projectile.scale * 0.5f, SpriteEffects.None, 0);
                    SB.Draw(HJScarletTexture.Particle_SharpTear, drawPos - Projectile.SafeDir() * i * 10, null,
                        color2 * .5f, Projectile.oldRot[k] + (float)Math.PI / 2, new Vector2(36, 36), Projectile.scale * 0.3f, SpriteEffects.None, 0);
                }
            }

            color1 = color;
            color2 = new Color(255, 255, 255, 0);
            Vector2 scl = new Vector2(1f, 1f + 0.1f * (float)Math.Sin(Main.timeForVisualEffects * 2));
            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center - Main.screenPosition, null,
                color, Projectile.rotation + (float)Math.PI / 2, new Vector2(36, 36), scl * Projectile.scale * 0.7f, SpriteEffects.None);

            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center - Main.screenPosition, null,
                color2, Projectile.rotation + (float)Math.PI / 2, new Vector2(36, 36), scl * Projectile.scale * 0.5f, SpriteEffects.None);
            Texture2D tex = HJScarletTexture.Particle_KiraStarGlow.Value;
            SB.EnterShaderArea();
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.ToOrigin(), Projectile.scale * 0.10f, 0, 0);
            DrawNebulaTrail(Color.DarkGray, 14f);
            DrawNebulaTrail(Color.Silver with { A = 150 }, 12.2f);
            DrawNebulaTrail(Color.White with { A = 100 }, 10.8f);

            SB.EndShaderArea();

            return false;
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[1] > 10;
        }
        public override void AI()
        {
            Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 0.6f, 0.6f, 0.6f);
            Vector2 targetPos = Projectile.Center;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy() && (Vector2.Distance(npc.Center, Projectile.Center) < 2000f))
                {
                    if ((targetPos == Projectile.Center) || (Vector2.Distance(targetPos, Projectile.Center)
                        >
                        Vector2.Distance(npc.Center, Projectile.Center)))
                    {
                        targetPos = npc.Center;
                    }
                }
            }
            Projectile.ai[1] += 1f / Projectile.MaxUpdates;
            if (Projectile.velocity.LengthSquared() > 0)
                Projectile.rotation = Projectile.velocity.ToRotation();
            if (targetPos != Projectile.Center)
            {
                Vector2 targetVel = Vector2.Normalize(targetPos - Projectile.Center) * 18;
                Projectile.velocity += 0.5f * Vector2.Normalize(targetVel - Projectile.velocity);
            }
            else
            {
                if (Projectile.velocity.Length() > 1f)
                    Projectile.velocity *= 0.95f;

                Projectile.timeLeft -= 2;
            }
        }
    }
    public class SamsaraofDawnlightSun : SamsaraofDawnlightMoon
    {
        public override Color trailColor => new Color(255, 165, 84, 0);
        public override bool AlterType => true;
    }
    public class SamsaraofDawnlightMoon : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true; Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
        }
        public virtual Color trailColor => new(212, 187, 255, 0);
        public virtual bool AlterType => false;
        public override bool? CanCutTiles()
        {
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.GetTexture();
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2;
                float tA = ((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length);
                Color color = trailColor * tA;
                SB.Draw(texture, drawPos, null, color, Projectile.oldRot[k] + PiOver4, texture.ToOrigin(), Projectile.scale * 0.85f, SpriteEffects.None, 0);
            }

            Vector2 pos = Projectile.Center - Main.screenPosition;
            float lerpValue = (float)Math.Abs(Math.Sin(Main.timeForVisualEffects / 16f)) * .2f + 1f;
            //SB.Draw(Request<Texture2D>("ContinentOfJourney/Images/SunlightDisciple_2").Value, pos, null, trailColor * 0.66f, 0f, new Vector2(40, 40), 2f, SpriteEffects.None, 0);

            SB.EnterShaderArea();
            Texture2D tex = HJScarletTexture.Particle_CrossGlow.Value;
            SB.Draw(tex, pos + Projectile.SafeDir() * 10f, null, trailColor with { A = 255 }, 0, tex.ToOrigin(), Projectile.scale * 0.35f, 0, 0);
            SB.Draw(tex, pos + Projectile.SafeDir() * 10f, null, trailColor with { A = 255 }, Pi, tex.ToOrigin(), Projectile.scale * 0.35f, 0, 0);
            tex = HJScarletTexture.Particle_KiraStarGlow.Value;
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 offset = Projectile.SafeDir().RotatedBy(PiOver2) * i * 15 + Projectile.SafeDir() * 10f;
                SB.Draw(tex, pos + offset, null, trailColor with { A = 255 } * 1f, Projectile.rotation + PiOver2, tex.ToOrigin(), Projectile.scale * 0.14f * lerpValue, 0, 0);
            }
            for (int i = -1; i < 2; i += 2)
            {
                ApplyTrailAlt(HJScarletTexture.Trail_ManaStreakTiny.Value, Color.Lerp(trailColor, Color.DarkGray, 0.23f) with { A = 200 }, 30, 15f * i);
                ApplyTrailAlt(HJScarletTexture.Trail_FadedStreak.Value, Color.Lerp(trailColor, Color.Gray, .23f) with { A = 200 }, 12, 15f * i);
                ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.Lerp(trailColor, Color.WhiteSmoke, .23f) with { A = 200 }, 28, 15f * i);
            }
            ApplyTrailAlt(HJScarletTexture.Trail_ManaStreakTiny.Value, Color.Lerp(trailColor, Color.DarkGray, 0.23f) with { A = 80 }, 56);
            ApplyTrailAlt(HJScarletTexture.Trail_FadedStreak.Value, Color.Lerp(trailColor, Color.Gray, .23f) with { A = 80 }, 40);
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.Lerp(trailColor, Color.WhiteSmoke, .23f) with { A = 80 }, 76);

            SB.EndShaderArea();
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(texture, pos + (TwoPi / 16f).ToRotationVector2() * 2f, null, Color.White.ToAddColor(), Projectile.rotation + PiOver4, texture.ToOrigin(), Projectile.scale, SpriteEffects.None, 0);
            }
            SB.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + PiOver4, texture.ToOrigin(), Projectile.scale, SpriteEffects.None, 0);
            SB.EnterShaderArea();
            SB.EndShaderArea();
            return false;
        }
        public void ApplyTrailAlt(Texture2D tex, Color color, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            float laserLength = 120;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -210);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(0.60f);
            shader.Parameters["uFadeinLength"].SetValue(0.052f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight * 1.32f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }

        public void ApplyTrailRenderer(Texture2D tex, Color color, float mult)
        {
            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2;
                date.Add(new(listPos, color, new(0, mult * 13f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);

        }
        public ref float Timer => ref Projectile.ai[0];
        public override void AI()
        {
            Timer++;
            if (Timer < 177)
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / 120);
            if (Timer < 120)
            {
                if (Timer > 20)
                    if (Projectile.velocity.Length() < 12f)
                    {
                        Vector2 unit = Vector2.Normalize(Projectile.velocity);
                        if (!unit.HasNaNs())
                            Projectile.velocity += unit * 0.2f;
                    }
            }
            else
            {
                Vector2 targetVel = new Vector2(Projectile.ai[1], Projectile.ai[2]) - Projectile.Center;
                if (targetVel.Length() < 240 && Projectile.timeLeft > 20) Projectile.timeLeft = 20;
                targetVel = Vector2.Normalize(targetVel);
                Projectile.velocity += targetVel * 0.4f;
                if (Projectile.velocity.Length() > 12f)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 12f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Color color = AlterType ? RandLerpColor(Color.Lerp(Color.Orange, Color.White, 0.5f), Color.Lerp(Color.OrangeRed, Color.White, 0.5f)) : RandLerpColor(Color.Lerp(Color.Violet, Color.White, 0.5f), Color.Lerp(Color.Purple, Color.White, 0.5f));
            if (Main.rand.NextBool(4))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePos(40), Projectile.velocity / 8f, color, 40, 1, 0.25f);
            color = AlterType ? RandLerpColor(Color.Lerp(Color.Orange, Color.White, 0.5f), Color.Lerp(Color.OrangeRed, Color.White, 0.5f)) : RandLerpColor(Color.Lerp(Color.Violet, Color.White, 0.5f), Color.Lerp(Color.Purple, Color.White, 0.5f));
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(40), Projectile.velocity / 8f, color, 40, 1, 0.55f, 0.2f);
        }
    }
}
