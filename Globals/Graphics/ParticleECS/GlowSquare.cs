using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using System;
using Terraria;

namespace HJScarletRework.Globals.Graphics.ParticleECS
{
    public class GlowSquare : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            data.aifloat2 = 1f;
            base.OnSpawn(ref data);
        }
        public override void Update(ref ECSParticleData data)
        {
            ref float rotSpeed = ref data.aifloat0;
            if (data.aifloat1 > 0)
                data.aifloat2 -= .1f;
            data.Scale *= .94f;
            data.DrawColor *= Lerp(1f, 0.15f, (float)Math.Pow(data.LifetimeRatio, 3D));
            data.Velocity *= .96f;
            if (rotSpeed != 0)
            {
                data.Rotation += rotSpeed;
                rotSpeed *= .992f;
            }
            else
                data.Rotation = data.Velocity.ToRotation() + PiOver2;
            base.Update(ref data);
        }
        public override void OnKill(ref ECSParticleData data)
        {
            base.OnKill(ref data);
        }
        public override void Draw(ref ECSParticleData data)
        {
            int type = data.aiint0;
            float glowMult = data.aifloat1;
            Vector2 scale = Vector2.One * data.Scale;
            Texture2D tex = type switch
            {
                1 => HJScarletTexture.Particle_GlowSquareBig.Value,
                2 => HJScarletTexture.Particle_GlowSquareThick.Value,
                _ => HJScarletTexture.Particle_GlowSquare.Value,
            };
            Vector2 pos = data.Position - Main.screenPosition;
            Main.spriteBatch.FastDraw(tex, pos, data.DrawColor, data.Rotation, tex.Size() / 2f, scale, 0);
            if (glowMult > 0)
                Main.spriteBatch.FastDraw(tex, pos, Color.White * data.aifloat2 * glowMult, data.Rotation, tex.Size() / 2f, scale * new Vector2(.9f), 0);
        }
    }
}
