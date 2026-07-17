using HJScarletRework.Globals.Graphics.ParticleECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Core.ParticleECS
{
    public static class ECSParticle
    {
        public static int ShinyCrossStarECS(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float smallDrawMult = 0.1f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<ShinyCrossStareECS>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs, ai0: smallDrawMult);
        }
        public static int HRShinyOrb(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float glowMult = 0.1f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<HRShinyOrbECS>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs, ai0: glowMult);
        }
        /// <summary>
        /// 弹壳。这个没处理碰撞
        /// </summary>
        public static int BulletShellECS(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, bool fullBright = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            return ECSMethod.NewParticle(GetInstance<BulletShellECS>().Type, timeLeft, pos, vel, color, opacity, rotation: RandRotTwoPi, scale: scale, blendstate: bs, ai0: fullBright.ToInt());
        }

        public static int SnowCloud(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rotation, float opacity, float scale, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<SnowCloudECS>().Type, timeLeft, pos, vel, color, opacity, rotation: rotation, scale: scale, blendstate: bs);
        }
        public static int LightntingGlow(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, int drawTime = 6, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<LightningGlowECS>().Type, timeLeft, pos, vel, color, opacity, 0, scale: scale, blendstate: bs, ai0: drawTime);

        }
        /// <summary>
        /// ECS的StarShape粒子，默认以velocity的角度
        /// </summary>

        public static int StarShape(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float glowMult = 0.5f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<StarShapeECS>().Type, timeLeft, pos, vel, color, opacity, 0, scale: scale, blendstate: bs, ai0: glowMult);
        }
        /// <summary>
        /// ECS的StarShape粒子，重载自定义转向方法
        /// </summary>
        public static int StarShape(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rotation, float opacity, float scale, float glowMult = 0.5f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<StarShapeECS>().Type, timeLeft, pos, vel, color, opacity, rotation, scale: scale, blendstate: bs, ai0: glowMult, ai1: 9);
        }
        public static int SmokeParticle(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rot, float opacity, float scale, bool alt = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            float Ai0 = alt ? 1 : 0;
            return ECSMethod.NewParticle(GetInstance<SmokeParticleECS>().Type, timeLeft, pos, vel, color, opacity, rot, scale, blendstate: bs, ai0: Ai0);
        }
        public static int BloodDrop(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, int edgeMultTime = 0, bool fullBright = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            return ECSMethod.NewParticle(GetInstance<BloodDrop>().Type, timeLeft, pos, vel, color, opacity, 0, scale, bs, fullBright.ToInt(), edgeMultTime);
        }
    }
}
