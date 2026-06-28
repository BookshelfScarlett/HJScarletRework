using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class FleshtumorHeldProj : ExecutorHeldProj
    {
        public override int OriginalItemID => ItemType<Fleshtumor>();
        public override string Texture => GetInstance<Fleshtumor>().Texture;
        public AnimationStruct Helper = new(2);
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(5);
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = (int)(AttackSpeed * .75f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .35f);
        }
        public bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public override void ProjAI()
        {
            if (Owner.HeldItem.type != OriginalItemID)
            {
                Projectile.Kill();
                return;
            }
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                
            }
            else if (!Helper.IsDone[1])
            {
                if(Helper.OnAnimationBegin(1))
                {

            SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { Pitch = 0.71f});
                }
                Helper.UpdateAniState(1);
            }
            Vector2 targetMountedPosition = Owner.GetToMouseVector2(Projectile.Center) * 150f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetMountedPosition.ToSafeNormalize(), .05f);
            float tarRot = Projectile.velocity.ToRotation();
            float beginRot = Projectile.rotation;
            float value = WrapAngle(tarRot - beginRot);
            Projectile.rotation = beginRot + value;
            Vector2 tarPos = Owner.MountedCenter + Owner.Center.GetNormalVector2(Main.MouseWorld) .ToSafeNormalize() * 60;
            Projectile.Center = Vector2.Lerp(Projectile.Center, tarPos, .05f);
            Projectile.position.Y += (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1.1f) * 0.5f);
            Projectile.spriteDirection = Projectile.direction = ((Owner.LocalMouseWorld().X - Projectile.Center.X) > 0).ToDirectionInt();
            Projectile.timeLeft = 2;
            if (!IsUsing)
                return;

            Owner.ChangeDir(Projectile.direction);
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.ControlPlayerArm(Projectile.rotation);

        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float progress1 = EaseOutBack(Helper.GetAniProgress(0));
            float progress2 = EaseOutBack( Helper.GetAniProgress(1));
            float globalTimeProgress = Lerp(0.95f, 1.05f, (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f)));
            Texture2D tex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation - PiOver4 + (Projectile.spriteDirection == -1 ? -PiOver2 : 0);
            Vector2 origin = tex.Size() / 2;
            Vector2 realDrawPos = drawPos;
            SpriteEffects se = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, realDrawPos + (TwoPi / 16f * i).ToRotationVector2() * 1.2f * progress2, null, Color.White.ToAddColor() * progress2, rotation, origin, Projectile.scale * progress2, se, 0);
            //SB.End();
            //SB.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //GD.Textures[0] = tex;
            //GD.SamplerStates[0] = SamplerState.PointClamp;
            //GD.Textures[1] = HJScarletTexture.Noise_Misc.Value;
            //GD.SamplerStates[1] = SamplerState.PointClamp;
            ////应用这个shader，我们正式开始画这把“喷火器”
            //Effect shader = HJScarletShader.EdgeMeltsShader;
            //shader.Parameters["progress"].SetValue((1 - EaseOutBack(Helper.GetAniProgress(0))));
            //shader.Parameters["InPutTextureSize"].SetValue(tex.Size());
            //shader.Parameters["EdgeColor"].SetValue(Color.Crimson.ToVector4());
            //shader.Parameters["EdgeWidth"].SetValue(.02f);
            //shader.CurrentTechnique.Passes[0].Apply();

            SB.Draw(tex, realDrawPos, null, Color.Lerp(Color.Transparent, Color.White, progress1), rotation, origin, Projectile.scale * progress1, se, 0);
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            float ringScale = Projectile.scale * .15f * globalTimeProgress * progress2;
            Texture2D ring = HJScarletTexture.Particle_RingShiny.Value;
            SB.Draw(ring, realDrawPos, null, Color.IndianRed* .5f * progress2, Projectile.rotation, ring.ToOrigin(), ringScale, 0, 0);
            SB.Draw(ring, realDrawPos, null, Color.Red* .5f * progress2, Projectile.rotation + Pi, ring.ToOrigin(), ringScale, 0, 0);
            SB.EnterShaderArea();
            SB.Draw(ring, realDrawPos, null, Color.Crimson* .5f * progress2, Projectile.rotation, ring.ToOrigin(), ringScale, 0, 0);
            SB.EndShaderArea();
            return false;
        }
    }
}
