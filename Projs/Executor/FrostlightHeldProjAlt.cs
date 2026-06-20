using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class FrostlightHeldProjAlt : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<Frostlight>();
        public override string Texture => GetInstance<FrostlightHeldProj>().Texture;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime, 20);
        public ref float Timer => ref Projectile.ai[0];
        public ref float ShootTimer => ref Projectile.ai[1];
        public ref float HeldAnimationHelper => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.SetUpHeldProj(10);
            Projectile.netImportant = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public float EdgeValue = 0;
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            HandleProjAttack();
            HandleParticle();
            HandlePlayerState();
            HandleProjStatement();
        }

        public void HandleProjStatement()
        {
        }

        public void HandlePlayerState()
        {
            Owner.ControlPlayerArm(Projectile.rotation);
            int dir = (Main.MouseWorld.X > Owner.Center.X).ToDirectionInt();
            Owner.ChangeDir(dir);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemAnimation = Owner.itemTime = 2;
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
            bool ifStillInUse = (Main.mouseLeft || Owner.controlUseTile) && !Owner.noItems && !Owner.CCed;
            if (ifStillInUse)
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();

        }

        public void HandleParticle()
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            if (Main.rand.NextBool(6))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                posBase += Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-30f, 30f);
                if (Main.rand.NextBool(3))
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    new Fire(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 50), RandRotTwoPi, 0.57f, Main.rand.NextFloat(.9f, 1.1f) * .25f).SpawnToPriority();
                }
                else
                {
                    Color fireColor = Color.Lerp(Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), Color.WhiteSmoke, Main.rand.NextFloat());
                    new SmokeParticle(posBase, -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f), fireColor, Main.rand.Next(35, 50), RandRotTwoPi, 0.57f, Main.rand.NextFloat(.9f, 1.1f) * .45f, Main.rand.NextBool()).SpawnToPriority();
                }
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                posBase += Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-35f, 37f) + dir * Main.rand.NextFloat(-60f, 60f);
                Dust d = Dust.NewDustPerfect(posBase, DustID.IceTorch);
                d.scale *= Main.rand.NextFloat(1.1f, 1.4f);
                d.velocity = -Vector2.UnitY * Main.rand.NextFloat(0.4f, 24f);
                d.noGravity = true;
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 posBase = Projectile.Center + dir * 65f * Projectile.scale + dir.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, 2.4f);
                Vector2 setPos = posBase + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * Main.rand.NextFloat(-20f, 21f) + dir * Main.rand.NextFloat(-60f, 60f);
                Vector2 vel = -Vector2.UnitY.RotatedByRandom(ToRadians(15)) * Main.rand.NextFloat(4f, 12f);
                float offsetX = setPos.X - posBase.X;

                bool needMirror = (offsetX > 0 && vel.X < 0) || (offsetX < 0 && vel.X > 0);
                if (needMirror)
                    setPos.X = posBase.X - offsetX;
                new ShinyCrossStar(setPos, vel, Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat()), 40, vel.ToRotation(), 1f, Main.rand.NextFloat(0.8f, 1.2f) * 0.48f, false).SpawnToPriority();
            }
        }

        public void HandleProjAttack()
        {
            float mirroredX = Owner.Center.X * 2 - Owner.ToClampMouseVector2().X;
            float finalX = Lerp(mirroredX, Owner.Center.X, .70f);
            Vector2 ownerToSky = new Vector2(finalX, Owner.Center.Y - 500) - Owner.MountedCenter;
            Vector2 skyDir = ownerToSky.ToSafeNormalize();
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, skyDir, HeldAnimationHelper);
            float targetRotaiton = Projectile.velocity.ToRotation();
            float currentRotation = Projectile.rotation;
            float value = WrapAngle(targetRotaiton - currentRotation);
            Projectile.rotation = currentRotation + value;
            if(Projectile.FinalUpdate())
            {
                HeldAnimationHelper += .01f;
                if (HeldAnimationHelper >= .05f)
                    HeldAnimationHelper = .05f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + PiOver4 + (Projectile.spriteDirection == -1 ? PiOver2 : 0);
            Vector2 origin = new Vector2(Projectile.spriteDirection == -1 ? tex.Width : 0, tex.Height);
            Vector2 realDrawPos = drawPos + Vector2.UnitX.RotatedBy(Projectile.rotation) * -25f + Vector2.UnitX.RotatedBy(Projectile.rotation).RotatedBy(PiOver2) * 0;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for(int i =0;i<12;i++)
            SB.Draw(tex, realDrawPos + (TwoPi / 12f * i).ToRotationVector2() * 1.2f, null, Color.White.ToAddColor(), rotation, origin, Projectile.scale, se, 0);
            SB.Draw(tex, realDrawPos, null, Color.White, rotation, origin, Projectile.scale, se, 0);
            SB.EnterShaderArea();
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Texture2D star = HJScarletTexture.Particle_CrossGlow.Value;
            Vector2 pos = drawPos + dir * 55f * Projectile.scale;
            float scale = Projectile.scale * .285f;
            SB.Draw(star, pos, null, Color.RoyalBlue * .9f, 0, star.ToOrigin(), scale * 0.95f, 0, 0);
            SB.Draw(star, pos, null, Color.SkyBlue* .9f, 0, star.ToOrigin(), scale * .90f, 0, 0);
            SB.Draw(star, pos, null, Color.LightBlue * .85f, 0, star.ToOrigin(), scale * 0.85f, 0, 0);
            star = HJScarletTexture.Particle_RingShiny.Value;
            SB.Draw(star, pos, null, Color.LightBlue * .35f, 0, star.ToOrigin(), scale * 0.35f, 0, 0);
            //出于我不清楚的原因，这里得重置两次批次，才能确保正常
            SB.EndShaderArea();
            SB.EndShaderArea();
            return false;
        }
    }
}
