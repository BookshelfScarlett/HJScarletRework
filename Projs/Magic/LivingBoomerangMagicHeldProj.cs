using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Magic;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Magic
{
    public class LivingBoomerangMagicHeldProj : HJScarletProj
    {
        public override string Texture => GetInstance<LivingBoomerangMagic>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public int AttackSpeed => Owner.ApplyWeaponAttackSpeed(GetInstance<LivingBoomerangMagic>().Item, GetInstance<LivingBoomerangMagic>().Item.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        public AnimationStruct Helper = new AnimationStruct(2);
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(3);
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 100;
            Projectile.SetupImmnuity(Projectile.MaxUpdates * 30);
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public bool IsUsing => (Owner.channel) && !Owner.dead && !Owner.CCed && !Owner.noItems;
        public override void OnFirstFrame()
        {
            ScarletSound(HJScarletSounds.Misc_ManaClearUse, Projectile.Center);
            Helper.Progress[0] = (int)(AttackSpeed * .75f);
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            if (IsUsing)
            {
                HoldIdleState();
                Projectile.rotation += .035f * Projectile.direction;
                Owner.ChangeDir(Projectile.direction);
                Owner.itemTime = Owner.itemAnimation = 2;
                Owner.ControlPlayerArm((Projectile.Center - Owner.Center).ToRotation(), 2);
                Projectile.timeLeft = 2;
            }
            else
                return;
            Owner.AddBuff(BuffID.ManaRegeneration, 60);
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                float progress = Helper.GetAniProgress(0);
                Projectile.scale = Lerp(0.15f, 1f, progress);
                Projectile.Opacity = Projectile.scale;
            }
            else
            {
                AttackMode();
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(13))
                ECSParticle.LiliesPetal(Projectile.Center.ToRandCirclePos(36), Vector2.UnitY, RandLerpColor(Color.LimeGreen, Color.Green), 60, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .1f, 0.6f, glowMult: 1, fullBright: true);
            if (Main.rand.NextBool(18))
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(36), 1.2f, RandLerpColor(Color.Pink, Color.HotPink), 45, 1, .1f, RandRotTwoPi, .40f);
            }

        }

        public void AttackMode()
        {
            Timer++;
            if (Timer < AttackSpeed)
                return;
            if (!Projectile.IsMe())
                return;
            if (!Owner.CheckMana(Owner.HeldItem, (int)(Owner.HeldItem.mana * Owner.manaCost), true, false))
                return;

            Timer = 0;
            ScarletSound(HJScarletSounds.TheSevenStar_Charge, Projectile.Center, .35f, 0, pitch: -0.14f, pitchVariance: .05f);
            Vector2 spawnPos = Projectile.Center;
            float glowScale = .15f;
            ECSParticle.CrossGlow(spawnPos, Color.HotPink, 45, 1, glowScale);
            ECSParticle.CrossGlow(spawnPos, Color.Pink, 45, 1, glowScale * .95f);
            ECSParticle.CrossGlow(spawnPos, Color.White, 45, 1, glowScale * .90f);
            //特效相关
            for (int i = 0; i < 10; i++)
                ECSParticle.TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(30), Main.rand.NextFloat(1.2f, 2.4f) * 2, RandLerpColor(Color.Pink, Color.LightPink), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
            for (int i = 0; i < 4; i++)
            {
                Vector2 vel = (Projectile.rotation + PiOver2 * i).ToRotationVector2() * -13f;
                Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), Projectile.Center, vel, ProjectileType<LivingBoomerangMagicLeaf>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);

            }
            for (int i = 0; i < 4; i++)
            {
                Vector2 vel = (Projectile.rotation + PiOver2 * i).ToRotationVector2() * 13f;
                Projectile proj = Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(Owner.HeldItem), Projectile.Center, vel, ProjectileType<LivingBoomerangMagicPetal>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);

                for (int j = 0; j < 8; j++)
                {
                    ECSParticle.SmokeParticle(Projectile.Center, vel.ToRandVelocity(ToRadians(15f), 1f, 12f), RandLerpColor(Color.Pink, Color.HotPink), 80, RandRotTwoPi, 1, 0.3f, true, BlendState.Additive);
                }
            }
        }

        public void HoldIdleState()
        {
            Vector2 targetMountedPosition = Owner.GetToMouseVector2(Projectile.Center) * 220f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetMountedPosition.ToSafeNormalize(), .05f);
            Vector2 curLength = (Owner.LocalMouseWorld() - Owner.MountedCenter);
            float targetLength = Lerp(0, curLength.Length(), .5f);
            Vector2 tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld).ToSafeNormalize() * targetLength;
            float lerpValue = .05f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, tarPos, lerpValue);
            Projectile.position.Y += (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f);
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X - Projectile.Center.X > 0).ToDirectionInt();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Owner.statMana < Owner.statManaMax2)
            {
                Owner.statMana += 2;
                Owner.ManaEffect(2);
            }
            modifiers.HitDirectionOverride = Projectile.ApplyDirectionOverride(target);

            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            DrawString();
            DrawBoomerangTrail(tex);
            DrawBoomerang(tex);
            DrawGlowMask();

            return false;
        }

        public void DrawGlowMask()
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D mask = Request<Texture2D>("HJScarletRework/Assets/Texture/Projs/LivingBoomerangGlowMask").Value;
            SB.FastDraw(mask, drawPos, Color.White, Projectile.rotation, mask.Size() / 2f, Projectile.scale, 0);
        }

        public void DrawBoomerang(Texture2D tex)
        {
            float beginProgress = Helper.GetAniProgress(0);
            Color mainColor = Color.Lerp(Color.Transparent, Color.White, beginProgress);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation;

            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPos + Main.rand.NextVector2Circular(5, 5) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.LimeGreen.ToAddColor(), rot, tex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(tex, drawPos, mainColor, rot, tex.Size() / 2f, Projectile.scale, 0);
        }

        public void DrawBoomerangTrail(Texture2D tex)
        {
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .55f, (progress));
                float yMult = Lerp(1f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.Pink, Color.Lerp(Color.White, Color.HotPink, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .59f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.LimeGreen, Color.Lerp(Color.Lime, Color.Green, .63f), EaseInOutQuad(progress));
                SB.FastDraw(tex, oldPos + Main.rand.NextVector2Circular(5, 5), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, tex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(tex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, tex.Size() / 2f, scale * .98f, 0);
            }
        }
        public void DrawString()
        {
            Asset<Texture2D> value = HJScarletTexture.Trail_Lightning4.Texture;
            float BeamLength = (Projectile.Center - Owner.MountedCenter).Length();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = BeamLength / value.Width();
            //轨迹
            SB.EnterShaderArea();
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 60);
            shader.Parameters["uColor"].SetValue(Color.LimeGreen.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition, null, Color.LimeGreen, (Owner.MountedCenter - Projectile.Center).ToRotation(), orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), 0.25f * Projectile.scale), 0, 0);
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition, null, Color.White * 0.85f, (Owner.MountedCenter - Projectile.Center).ToRotation(), orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), 0.20f * Projectile.scale), 0, 0);
            SB.EndShaderArea();
        }
    }
}
