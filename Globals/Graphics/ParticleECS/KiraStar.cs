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
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Graphics.ParticleECS
{
    public class KiraStar : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            data.aifloat1 = data.Scale;
            if (data.aifloat0 > 0)
            {
                data.Scale = 0;
            }
        }
        public override void Update(ref ECSParticleData data)
        {
            float fadeInTime = data.aifloat0;
            float beginScale = data.aifloat1;
            fadeInTime = Clamp(fadeInTime, 0.0f, 1.0f);
            if (data.LifetimeRatio < fadeInTime && fadeInTime > 0)
            {
                //归一化
                float progress = data.LifetimeRatio / fadeInTime;
                data.Scale = Lerp(0f, beginScale, EasingHelper.EaseOutCubic(progress));
            }
            else
            {
                //归一化进度=(当前比例 - 淡入比例) / (1 - 淡入比例)
                float remaining = (data.LifetimeRatio - fadeInTime) / (1f - fadeInTime);
                //防止为0
                remaining = Clamp(remaining, 0f, 1f);
                float progress = EasingHelper.EaseInBack(remaining);
                data.Scale = Lerp(beginScale, 0, progress);
            }
            data.Velocity *= .96f;
        }
        public override void Draw(ref ECSParticleData data)
        {
            bool useAltTex = data.aifloat2 != 0;
            Texture2D tex = useAltTex ? HJScarletTexture.Particle_KiraStarGlow.Value : HJScarletTexture.Particle_KiraStar.Value;
            Main.spriteBatch.Draw(tex, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, tex.Size() / 2, data.Scale, 0, 0);
        }
    }
}
