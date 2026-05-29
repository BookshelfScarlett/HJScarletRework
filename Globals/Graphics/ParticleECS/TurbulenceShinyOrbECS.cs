using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Items.Weapons.Requirement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Globals.Graphics.ParticleECS
{
    public class TurbulenceShinyOrbECS : ECSParticleBehavior
    {
        /// <summary>
        /// AI0是湍流的速度
        /// </summary>
        public override void OnSpawn(ref ECSParticleData data)
        {
            data.aifloat2 = data.Scale;
            data.aiint0 = Main.rand.Next(0, 100000);
        }
        public override void Update(ref ECSParticleData data)
        {
            float Speed = data.aifloat0;
            if (Speed != 0)
            {
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(Lerp(-TwoPi, TwoPi, (float)Math.Sin(data.Time / 36f + data.aiint0) * 0.5f + 0.5f)) * Speed;
                float movementInterpolant = Lerp(0.01f, 0.25f, Utils.GetLerpValue(0, data.Lifetime / 2, data.Time, true));
                data.Velocity = Vector2.Lerp(data.Velocity, idealVelocity, movementInterpolant);
                data.Velocity = data.Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            }
            data.Velocity *= 0.9f;
            data.Scale = Lerp(data.aifloat2, 0, EasingHelper.EaseOutCubic(data.LifetimeRatio));
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D texture = HJScarletTexture.Particle_HRShinyOrb.Value;
            Main.spriteBatch.Draw(texture, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, texture.Size() / 2, data.Scale, SpriteEffects.None, 0);
        }
    }
}
