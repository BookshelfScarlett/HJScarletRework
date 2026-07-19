using HJScarletRework.Globals.Graphics.ParticleECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

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
        /// <summary>
        ///<para><paramref name="fullBright"/>用于表示粒子是否应该无视光照</para>
        ///<para>尽管如此，这个粒子非常暗，无视光照的情况下可能也很难在夜间看清其行为</para>
        /// </summary>
        /// <returns></returns>
        public static int LiliesFire(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rot, float opacity, float scale, bool fullBright = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            float Ai0 = fullBright ? 1 : 0;
            return ECSMethod.NewParticle(GetInstance<LiliesFire>().Type, timeLeft, pos, vel, color, opacity, rot, scale, blendstate: bs, ai0: Ai0);
        }
        /// <summary>
        /// <para><paramref name="glowMult"/>表示中心辉光的大小，<paramref name="direction"/>用于表示湍流粒子的旋转偏移量，默认为TwoPi的弧度</para>
        /// </summary>
        /// <returns></returns>
        public static int TurbulenceShinyOrb(Vector2 pos, float speed, Color color, int lifeTime, float opacity, float scale, float direction = TwoPi, float glowMult = 0, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<TurbulenceShinyOrbECS>().Type, lifeTime, pos, Vector2.UnitX, color, opacity, direction, scale, bs, speed, glowMult);
        }
        /// <summary>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int KiraStar(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, bool altVersion = false, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<KiraStar>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, fadinTime, scale, altVersion.ToInt());
        }
        /// <summary>
        /// <para>一个重载方案，跳过了<paramref name="rotation"/>（设定为0）和<paramref name="vel"/>（设定为0）的传参</para>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int KiraStar(Vector2 pos, Color color, int timeLeft, float opacity, float scale, bool altVersion = false, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<KiraStar>().Type, timeLeft, pos, Vector2.Zero, color, opacity, 0, scale, bs, fadinTime, scale, altVersion.ToInt());
        }
        /// <summary>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int CrossGlow(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<CrossGlow>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, ai0: fadinTime);
        }
        /// <summary>
        /// <para>一个重载方案，跳过了<paramref name="rotation"/>（设定为0）和<paramref name="vel"/>（设定为0）的传参</para>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int CrossGlow(Vector2 pos, Color color, int timeLeft, float opacity, float scale, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<CrossGlow>().Type, timeLeft, pos, Vector2.Zero, color, opacity, 0, scale, bs, ai0: fadinTime);
        }
        /// <summary>
        /// <para>超级复杂的传参</para>
        /// <para><paramref name="floatSpeed"/>为花瓣/落叶的漂浮速度大小，一般值越大，代表其速度越快</para>
        /// <para><paramref name="alterTexture"/>True时启用另一个不同的贴图</para>
        /// <para><paramref name="fullBright"/>是否无视光照，默认为false，即受环境光影响</para>
        /// <para><paramref name="noCollision"/>是否无视物块，在原AI中，花瓣会有物块碰撞判定</para>
        /// <para><paramref name="glowMult"/>是否绘制描边且重复多少次绘制，默认为0，即不描边，取1时，绘制8次</para>
        /// </summary>
        /// <returns></returns>
        public static int LiliesPetal(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, float floatSpeed, bool noCollision = false, int glowMult = 0, bool fullBright = false, bool alterTexture = false, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            float aifloat2 = alterTexture.ToInt();
            return ECSMethod.NewParticle(GetInstance<LiliesPetal>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, ai0: floatSpeed, ai1: scale, ai2: aifloat2, aibool1: noCollision, aibool2: fullBright, aiint2: glowMult);
        }
    }
}
