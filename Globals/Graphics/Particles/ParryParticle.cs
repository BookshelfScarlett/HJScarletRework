using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    public class ParryParticle : BaseParticle
    {
        public int MountedPlayer = -1;
        public float TargetScale = 0;
        public float TargetOpac = 0;
        public override string Texture => HJScarletTexture.Particle_Parry.Path;
        public override int UseBlendStateID => BlendStateID.Alpha;
        public ParryParticle(Vector2 pos, float scale, int lifeTime)
        {
            DrawColor = Color.White;
            Scale = 0;
            Opacity = 0;
            Scale = scale;
            Position = pos;
            TargetOpac = 1f;
            Rotation = 0;
            Lifetime = lifeTime;
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
        public override void Update()
        {

            if (LifetimeRatio > .9f)
            {
                //1->0
                float rat = (1 - LifetimeRatio) / .1f;
                Opacity = Lerp(0, Opacity, rat);
                Velocity -= Vector2.UnitY/2f;
                if (Velocity.Length() > .1f)
                    Velocity *= .9f;
            }
            else
            {
                Opacity = Lerp(Opacity, TargetOpac + .1f, .15f);
                Velocity -= Vector2.UnitY/100f;
                if (Velocity.Length() > 4f)
                    Velocity *= .9f;

            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D pingTexture = HJScarletTexture.Particle_Parry.Value;
            Vector2 origin = pingTexture.Size() * 0.5f;
            Vector2 pingDrawPosition;

            Vector2 finalPosition = Position + new Vector2(10f, -80f);
            float progress = EaseInOutQuad(LifetimeRatio);
            {
                float popOutProgress = (float)LifetimeRatio;
                pingDrawPosition = Vector2.Lerp(Position, finalPosition, EaseOutCubic(popOutProgress));
            }

            pingDrawPosition -= Main.screenPosition;

            float stretchFactorY = 0f;
            float firstStretchThreshold = .1f;
            float secondStretchThreshold= .6f;
            float thirdStretchThreshold = .8f;

            float firstTargetFactor = .8f;
            float secondTargetFactor = -.6f;
            float thridTargetFactor = 0;
            if (progress < firstStretchThreshold)
            {
                //挤压
                float stretchProgress = progress / firstStretchThreshold;
                stretchFactorY = Lerp(0f, firstTargetFactor, stretchProgress);
            }
            else if (progress < secondStretchThreshold)
            {
                //延展
                float squashProgress = EaseOutCubic(Utils.GetLerpValue(firstStretchThreshold, secondStretchThreshold, progress,true));
                stretchFactorY = Lerp(firstTargetFactor, secondTargetFactor, squashProgress * squashProgress);
            }
            else if (progress < thirdStretchThreshold)
            {
                //第三部分要让其略微形变回原本的样子
                float stretchProgress = EaseOutCubic(Utils.GetLerpValue(secondStretchThreshold, thirdStretchThreshold, progress, true));
                stretchFactorY = Lerp(secondTargetFactor, thridTargetFactor, stretchProgress * stretchProgress);
            }
            else
            {
                stretchFactorY = 0f;
            }

            // Draw without any additions
            Color drawColor = Color.White*Opacity;

            // Fade out over 30 frames
            if (LifetimeRatio >= .75f)
            {
                float fadeProgress = Utils.GetLerpValue(.75f, 1f, LifetimeRatio,true);
                drawColor = Color.White * Lerp(1f, 0f, fadeProgress);
            }

            float basePingDrawScale = Scale;
            float stretchScaleY = 1f + stretchFactorY;
            float stretchScaleX = 1f - (stretchFactorY * 0.75f);

            // Calculate scale to display with all factors combined
            Vector2 finalPingScale = new Vector2(basePingDrawScale * stretchScaleX, basePingDrawScale * stretchScaleY);
            spriteBatch.Draw(pingTexture, pingDrawPosition, null, drawColor, 0, origin, finalPingScale, 0, 0f);
        }
    }
}
