using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class EnchantedSwordfishJavelin : HJScarletProj
    {
        public override string Texture => GetInstance<EnchantedSwordfishProj>().Texture;
        public override ClassCategory Category => ClassCategory.Melee;
        public enum State
        {
            Slow,
            Laser
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public float OriginalSpeed = -1;
        public float TimerRatios = 0;
        public AnimationStruct Helper = new(3);
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = 7;
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 120;
            OriginalSpeed = Projectile.velocity.Length();
        }
        public override void ProjAI()
        {
            HandleProjAI();
        }

        public void HandleProjAI()
        {
            switch (AttackState)
            {
                case State.Slow:
                    DoSlow();
                    break;
                case State.Laser:
                    DoLaser();
                    break;
            }
        }
        public float LaserScaleX = 0;
        public void DoSlow()
        {
            float maxTime = 40f;
            TimerRatios = Clamp(Timer / maxTime, 0f, 1f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Lerp(Projectile.SafeDir() * OriginalSpeed, Projectile.SafeDir() * 0.1f, TimerRatios);
            Timer++;
            if (TimerRatios == 1f)
            {
                AttackState = State.Laser;
                Projectile.netUpdate = true;
                BeginTargetRotation = Projectile.rotation;
                TargetRotation = BeginTargetRotation;
            }
        }

        public void DoLaser()
        {
            Projectile.timeLeft = 2;
            if (Helper.IsDone[0])
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.velocity = Projectile.velocity.ToSafeNormalize();
                Projectile.velocity = TargetRotation.ToRotationVector2();
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return AttackState == State.Slow;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }
        public override Vector2 TileHitbox => new Vector2(12);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public int BeamLength = 4200;
        public Vector2 EyeOffset => Projectile.rotation.ToRotationVector2() * 10f;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);

            SB.Draw(projTex, drawPos, null, Color.White, Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            SB.EnterShaderArea();
            Texture2D crossGlow = HJScarletTexture.Particle_CrossGlow.Value;
            float crossRotation = Lerp(0, ToRadians(45), TimerRatios);
            float glowScale = Projectile.scale * .27f;
            float glowRot = Projectile.rotation + crossRotation;
            SB.Draw(crossGlow, drawPos - EyeOffset, null, Color.DarkRed, glowRot, crossGlow.ToOrigin(), glowScale, 0, 0);
            SB.Draw(crossGlow, drawPos - EyeOffset, null, Color.Red, glowRot, crossGlow.ToOrigin(), glowScale * .95f, 0, 0);
            SB.Draw(crossGlow, drawPos - EyeOffset, null, Color.White, glowRot, crossGlow.ToOrigin(), glowScale * .90f, 0, 0);
            //if (LaserScaleX > .02f)
            //{

            //    DrawBeam(Color.DarkRed, 0.550f);
            //    DrawBeam(Color.Red, 0.5f);
            //    DrawBeam(Color.IndianRed, 0.45f);
            //    DrawBeam(Color.White, 0.40f);
            //}
            SB.EndShaderArea();
            return false;
        }
        public void DrawBeam(Color color, float height)
        {

            Asset<Texture2D> value = HJScarletTexture.Trail_ManaStreak.Texture;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -70);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.0f);
            shader.CurrentTechnique.Passes[0].Apply();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = (int)(BeamLength * LaserScaleX) / value.Width();
            SB.Draw(value.Value, Projectile.Center - Main.screenPosition - EyeOffset, null, Color.White, Projectile.rotation, orig, new Vector2(xScale * Clamp(Projectile.scale, 0.02f, 1f), height * Projectile.scale * 0.20f * LaserScaleX), 0, 0);
        }

    }
}
