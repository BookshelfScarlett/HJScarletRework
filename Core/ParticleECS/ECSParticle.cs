using HJScarletRework.Globals.Graphics.ParticleECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return ECSMethod.NewParticle(GetInstance<HRShinyOrbECS>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs,ai0:glowMult);

        }
        public static int SnowCloud(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rotation,float opacity, float scale, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<SnowCloudECS>().Type, timeLeft, pos, vel, color, opacity,rotation:rotation, scale: scale, blendstate: bs);

        }
    }
}
