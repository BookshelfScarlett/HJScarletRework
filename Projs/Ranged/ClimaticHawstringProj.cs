using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class ClimaticHawstringProj : HJScarletProj, IPixelatedRenderer
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override string Texture => GetInstance<ClimaticHawstring>().Texture;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;
        public AnimationStruct Helper = new(2);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public int GetUseTime => Owner.ApplyWeaponAttackSpeed(GetInstance<ClimaticHawstring>().Item, GetInstance<ClimaticHawstring>().Item.useTime, 5);
        public ref float Timer => ref Projectile.ai[0];
        public bool CanLaser => Owner.HJScarlet().climaticHawstringLaserCounter >= 20;
        public override void ExSD()
        {
            Projectile.width = 28;
            Projectile.height = 60;
            Projectile.SetupImmnuity(-1);
            Projectile.SetUpHeldProj();
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = GetSeconds(5);
        }

        public void UpdateBowStatement()
        {
            Projectile.velocity = Owner.ToMouseVector2();
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void UpdateAttackAI()
        {
            Vector2 dir = Projectile.SafeDir();
            if (CanLaser)
            {
                Helper.UpdateAniState(0);
                if (Helper.Progress[0] % 3 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item158 with { MaxInstances = 0, Pitch = -0.38f, PitchVariance = 0.1f }, Projectile.Center);
                    Vector2 laserPos = Projectile.SafeDir().RotatedBy(PiOver2) * Main.rand.NextFloat(-15f, 16f);
                    Projectile proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + laserPos + dir * 17f, dir * 12f, ProjectileType<ClimaticHawstringBeam>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    proj2.rotation = dir.ToRotation();
                    new ShinyCrossStar(Projectile.Center + laserPos + dir * 27f, Vector2.Zero, RandLerpColor(Color.DarkGoldenrod, Color.Goldenrod), 40, 0, 1, 0.80f, false).Spawn();
                    for (int j = 0; j < 8; j++)
                        new StarShape(Projectile.Center.ToRandCirclePos(4f) + laserPos + dir * 17f, dir * Main.rand.NextFloat(0.1f, 6.2f), RandLerpColor(Color.Goldenrod, Color.DarkGoldenrod), Main.rand.NextFloat(0.35f, 0.45f) * 1.2f, 40).Spawn();
                }
                if (Helper.IsDone[0])
                {
                    Helper.IsDone[0] = false;
                    Helper.Progress[0] = 0;
                    Owner.HJScarlet().climaticHawstringLaserCounter = 0;
                    //喷一点粒子出来收尾
                    for (int i = 0; i < 8; i++)
                        new StarShape(Projectile.Center.ToRandCirclePos(4f) + dir * 17f, dir.ToRandVelocity(ToRadians(5f), 0.1f, 6.2f), RandLerpColor(Color.DarkGoldenrod, Color.Goldenrod), Main.rand.NextFloat(0.35f, 0.45f) * 2.0f, 40).Spawn();
                    for (int i = 0; i < 16; i++)
                        new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(4f) + dir * 17f, dir.ToRandVelocity(ToRadians(5f), 0.1f, 9.2f), RandLerpColor(Color.Goldenrod, Color.PaleGoldenrod), 40, Main.rand.NextFloat(0.24f, 0.35f) * Projectile.scale).Spawn();
                }
            }
            Timer++;
            if (Timer < GetUseTime)
                return;

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 pos = dir.RotatedBy(PiOver2) * i * 22f + Projectile.Center;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos + dir * 17f, dir * 12f, ProjectileType<ClimaticHawstringArrow>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                proj.rotation = dir.ToRotation();
                for (int j = 0; j < 16; j++)
                {
                    new StarShape(pos.ToRandCirclePos(4f) + dir * 17f, dir * Main.rand.NextFloat(0.1f, 6.2f), RandLerpColor(Color.Goldenrod, Color.DarkGoldenrod), Main.rand.NextFloat(0.35f, 0.45f) * 1.2f, 40).Spawn();
                }
                new ShinyCrossStar(pos + dir * 17f, Vector2.Zero, RandLerpColor(Color.DarkGoldenrod, Color.Goldenrod), 40, 0, 1, 1.2f, false).Spawn();
            }
            SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_Hit with { MaxInstances = 0 }, Owner.Center);
            Timer = 0;
        }
        public void UpdatePlayerState()
        {
            Projectile.spriteDirection = Projectile.direction = (Owner.LocalMouseWorld().X > Owner.Center.X).ToDirectionInt();
            Owner.ControlPlayerArm(Projectile.rotation);
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemAnimation = Owner.itemTime = 2;
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;
        }

        public override void ProjAI()
        {
            if (CheckOwnerState())
                return;
            //是的这里会全程检查是否拥有这个射弹
            if (!Owner.HasProj<ClimaticHawstringMinion>(out int projID))
            {
                SoundEngine.PlaySound(SoundID.Item44 with { MaxInstances = 0 }, Owner.Center);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.MountedCenter + Vector2.UnitX * 500f - Vector2.UnitY * 1000f, Vector2.Zero, projID, Projectile.damage, 0f, Owner.whoAmI);
                proj.rotation = (-Vector2.UnitX).ToRotation();
                ((ClimaticHawstringMinion)proj.ModProjectile).Reverse = true;
                proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.MountedCenter- Vector2.UnitX * 500f - Vector2.UnitY * 1000f, Vector2.Zero, projID, Projectile.damage, 0f, Owner.whoAmI);
                proj.rotation = (-Vector2.UnitX).ToRotation();
                ((ClimaticHawstringMinion)proj.ModProjectile).Reverse = false;
            }

            UpdateAttackAI();
            UpdateBowStatement();
            UpdatePlayerState();
            Projectile.netUpdate = true;

        }
        public bool CheckOwnerState()
        {
            bool ifStillUse = (Owner.channel || Owner.controlUseTile) && !Owner.noItems && !Owner.CCed;
            if (!ifStillUse)
            {
                Projectile.Kill();
                return true;
            }
            else
                Projectile.timeLeft = 2;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = new(12 * Owner.direction, 0);
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            Vector2 rotationPoint = tex.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection * Main.player[Projectile.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color endColor = Color.Lerp(Color.White, Color.DarkGoldenrod, 0.5f);
            for (int i = 0; i < 8; i++)
            {
                Color edgeColor = Color.Lerp(Color.Transparent, endColor.ToAddColor(), EaseInOutSin((float)Timer / GetUseTime));
                if (CanLaser)
                    edgeColor = endColor.ToAddColor();
                SB.Draw(tex, drawPos + offset.RotatedBy(drawRot) + ToRadians(60 * i).ToRotationVector2() * 2f, null, edgeColor, drawRot, rotationPoint, Projectile.scale, flipSprite, default);
            }
            SB.Draw(tex, drawPos + offset.RotatedBy(drawRot), null, lightColor, drawRot, rotationPoint, Projectile.scale, flipSprite, default);

            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //overWiresUI.Add(index);
        }
        public void RenderPixelated(SpriteBatch sb)
        {
            //HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            ////这里的放缩会被lerp进行一次总控。
            //float GeneralProgress = (float)Timer / GetUseTime;
            //Vector2 dynamicBackgroundScale = Vector2.Lerp(Vector2.Zero, new Vector2(1.0f, 1.0f), GeneralProgress) * Projectile.scale * 0.55f;
            //Vector2 dynamicBloomScale = Vector2.Lerp(Vector2.Zero, new Vector2(0.5f, 0.5f), GeneralProgress) * Projectile.scale *.55f;
            //float ringScale = Lerp(0, 1.28f, GeneralProgress) * Projectile.scale;
            //Texture2D tex = HJScarletTexture.Particle_HRStar.Value;
            //Texture2D ring = HJScarletTexture.Particle_Ring.Value;
            //Vector2 ori = tex.Size() / 2;
            //Vector2 offset = Projectile.SafeDir() * 20f - Main.screenPosition;
            ////最后我们实际绘制他。
            //sb.Draw(tex, Projectile.Center + offset + Projectile.SafeDir().RotatedBy(PiOver2) * 15f, null, Color.Gold, Projectile.rotation + PiOver4, ori, dynamicBackgroundScale, SpriteEffects.None, 0.1f);
            //sb.Draw(tex, Projectile.Center + offset + Projectile.SafeDir().RotatedBy(PiOver2) * 15f, null, Color.White, Projectile.rotation + PiOver4, ori, dynamicBloomScale, SpriteEffects.None, 0.1f);
            //HJScarletMethods.EndShaderAreaPixel();


        }

    }
}
