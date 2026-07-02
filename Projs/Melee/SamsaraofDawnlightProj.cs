using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
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
    public class SamsaraofDawnlightProj : HJScarletProj
    {
        public override string Texture => GetInstance<SamsaraofDawnlightAlter>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
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
            if (ai2 == 1)
                hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(320, 320));


        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public virtual Color trailColor => new(255, 255, 255, 0);
        public override bool? CanCutTiles()
        {
            return true;
        }
        public void drawStar(Vector2 off, float scale, float starScale, Color c)
        {
            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center + off * scale - Main.screenPosition, null,
                c, 0, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f), SpriteEffects.None);
            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center + off * scale - Main.screenPosition, null,
                c, (float)Math.PI / 2, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f), SpriteEffects.None);

            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center + off * scale - Main.screenPosition, null,
                new Color(255, 255, 255, 0), 0, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f) * 0.66f, SpriteEffects.None);
            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center + off * scale - Main.screenPosition, null,
                new Color(255, 255, 255, 0), (float)Math.PI / 2, new Vector2(36, 36), new Vector2(starScale / 2, starScale * 2f) * 0.66f, SpriteEffects.None);
        }
        public void DrawSunMoonTrail()
        {
            float laserLength = 50;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_ManaStreak.Size);
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_ManaStreak.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            shader.Parameters["uColor"].SetValue(Color.Pink.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(0.1f);
            shader.Parameters["uFadeinLength"].SetValue(0.05f);
            shader.CurrentTechnique.Passes[0].Apply();
        }
        public void ApplyTrail(Texture2D tex, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            DrawSetting sets = new(HJScarletTexture.Trail_ManaStreak.Value);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.SafeDir() * 10f + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
                float ratios = i / (float)Projectile.oldPos.Length;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (ai2 == 1)
            {
                Vector2 drawPos = Projectile.Center;
                float time = Projectile.ai[1];
                float fac1 = Utils.Clamp(time / 90, 0f, 1f);
                fac1 = (float)(0.5f - 0.5f * Math.Cos(fac1 * Math.PI));
                if (time > 120)
                    fac1 = 1f - (time - 120) / 20;

                float colorLerp = Utils.Clamp(time / 180, 0f, 1f);
                colorLerp = 0.5f - 0.5f * (float)Math.Cos(colorLerp * Math.PI);
                Color showColor = Color.Lerp(new Color(255, 208, 133, 0), new Color(212, 187, 255, 0), colorLerp);

                Main.EntitySpriteDraw(Request<Texture2D>("ContinentOfJourney/Images/Glow1").Value,
                    drawPos - Main.screenPosition, null,
                    showColor * Utils.Clamp(time / 10, 0f, 1f) * 0.7f, Projectile.rotation,
                    new Vector2(100, 100), fac1 * 2.5f + (float)Math.Sin(Main.timeForVisualEffects) * 0.3f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(Request<Texture2D>("ContinentOfJourney/Images/GiantLight_Alt").Value,
                    drawPos - Main.screenPosition, null,
                    showColor * Utils.Clamp(time / 10, 0f, 1f) * 0.3f, 0f,
                    new Vector2(500, 500), fac1, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(Request<Texture2D>("ContinentOfJourney/Images/GiantLight_Alt").Value,
                    drawPos - Main.screenPosition, null,
                    Color.Black * Utils.Clamp(time / 10, 0f, 1f), 0f,
                    new Vector2(500, 500), fac1 * 0.4f, SpriteEffects.None, 0);
                float vtime = (float)Main.timeForVisualEffects;

                for (int i = 0; i < 10; i++)
                {
                    drawStar(
                        new Vector2(71, 13).RotatedBy((vtime + i * 233) / 57) + new Vector2(51, 57).RotatedBy((vtime + i * 71) / 37),
                        fac1 * 0.66f, fac1 * (0.5f + 0.5f * (float)Math.Sin((Main.timeForVisualEffects + i * 131) / 311)) * 1.2f,
                        showColor);
                }


                return false;
            }
            Projectile.localAI[0]++;
            float totalA = Utils.Clamp(Projectile.localAI[0] / 20, 0f, 1f);
            Projectile.rotation += 0.2f;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(Projectile.width / 2, Projectile.height / 2);
            //for (int k = 0; k < Projectile.oldPos.Length; k++)
            //{
            //    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
            //    float tA = ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            //    Color color = trailColor * tA * tA;

            //    Main.EntitySpriteDraw(Request<Texture2D>(Texture).Value, drawPos, null, color * totalA, Projectile.oldRot[k], new Vector2(texture.Width / 2, texture.Height / 2),
            //        tA, SpriteEffects.None, 0);
            //}
            //Main.EntitySpriteDraw(Request<Texture2D>("ContinentOfJourney/Images/SunlightDisciple_2").Value,
            //   Projectile.Center - Main.screenPosition, null,
            //     new Color(255, 255, 255, 0) * totalA, 0f,
            //    new Vector2(40, 40), Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw
            (texture, Projectile.Center - Main.screenPosition, null, Color.White * totalA,
            Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, SpriteEffects.None, 0);
            SB.EnterShaderArea();
            DrawSunMoonTrail();
            SB.EndShaderArea();
            return false;
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
            if (ai2 > 0) return;
            if (ai2 == 0 && Projectile.ai[0] < 62)
            {
                Projectile.ai[0] = 62;
                Projectile.netUpdate = true;
            }
        }
        public override void AI()
        {
            switch(AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.TurnintoSignarity:
                    DoTurnintoSignarity();
                    break;
            }
        }

        public void DoShoot()
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight);
            d.velocity *= 0.5f;
            d.scale = Main.rand.NextFloat(1f, 2f);
            d.noGravity = true;

            Projectile.rotation += 0.2f;
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
            SoundEngine.PlaySound(HJScarletSounds.Misc_AirCharge, Projectile.Center);
            AttackState = State.TurnintoSignarity;
            Projectile.netUpdate = true;
        }

        public void DoTurnintoSignarity()
        {
            Projectile.velocity *= 0.2f;
            Projectile.ai[1]++;
            Projectile.hide = true;

            if (Projectile.ai[1] == 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight);
                    d.velocity *= 12f;
                    d.scale = Main.rand.NextFloat(1f, 3f);
                    d.noGravity = true;
                }
            }
            if (Projectile.ai[1] % 20 == 0)
            {
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
                for (int i = 0; i < 30; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(3, 3), 0, 0, DustID.AncientLight);
                    d.velocity = new Vector2(24f, 0).RotatedBy(TwoPi / 30 * i);
                    d.velocity.Y *= 0.8769f;
                    d.scale = Main.rand.NextFloat(2f, 3f);
                    d.noGravity = true;
                    d = Dust.NewDustDirect(Projectile.Center - new Vector2(3, 3), 0, 0, DustID.AncientLight);
                    d.velocity = new Vector2(18f, 0).RotatedBy(TwoPi / 30 * i);
                    d.velocity.Y *= 0.8769f;
                    d.scale = Main.rand.NextFloat(1f, 3f);
                    d.noGravity = true;
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight);
                    d.velocity *= 12f;
                    d.scale = Main.rand.NextFloat(1f, 3f);
                    d.noGravity = true;
                }

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
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.scale = (Projectile.ai[1] < 10) ? Projectile.ai[1] * 0.1f : 1;
            Vector2 drawOrigin = new Vector2(12, 12);
            Color color = new Color(255, 255, 255, 0) * 0.5f;

            Color color1;
            Color color2;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                color1 = color * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color2 = new Color(255, 255, 255, 0) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, drawPos, null,
                    color1, Projectile.oldRot[k] + (float)Math.PI / 2, new Vector2(36, 36), Projectile.scale * 0.5f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, drawPos, null,
                    color2, Projectile.oldRot[k] + (float)Math.PI / 2, new Vector2(36, 36), Projectile.scale * 0.3f, SpriteEffects.None, 0);
            }
            color1 = color;
            color2 = new Color(255, 255, 255, 0);
            Vector2 scl = new Vector2(1f, 1f + 0.1f * (float)Math.Sin(Main.timeForVisualEffects * 2));
            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center - Main.screenPosition, null,
                color, Projectile.rotation + (float)Math.PI / 2, new Vector2(36, 36), scl * Projectile.scale * 0.7f, SpriteEffects.None);

            Main.EntitySpriteDraw(HJScarletTexture.Particle_SharpTear, Projectile.Center - Main.screenPosition, null,
                color2, Projectile.rotation + (float)Math.PI / 2, new Vector2(36, 36), scl * Projectile.scale * 0.5f, SpriteEffects.None);

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
            Projectile.ai[1] += 1;
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
        public override Color trailColor => new Color(255,165,84,0);
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
        public override bool? CanCutTiles()
        {
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 4;
            Texture2D texture = Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(Projectile.width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                float tA = ((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length);
                Color color = trailColor * tA;

                Main.EntitySpriteDraw(Request<Texture2D>(Texture).Value, drawPos, null, color, Projectile.oldRot[k], new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(Request<Texture2D>("ContinentOfJourney/Images/SunlightDisciple_2").Value,
                Projectile.Center - Main.screenPosition, null,
                trailColor * 0.66f, 0f,
                new Vector2(40, 40), 2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw
            (texture, Projectile.Center - Main.screenPosition, null, Color.White,
            Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, SpriteEffects.None, 0);
            return false;

        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 177)
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / 120);
            if (Projectile.ai[0] < 120)
            {
                if (Projectile.ai[0] > 20)
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
        }
    }
}
