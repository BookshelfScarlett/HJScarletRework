using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Core.ParticleECS
{
    public static class ECSMethod
    {
        public static int NewParticle(int Type, int timeLeft, Vector2 position, Vector2 velocity, Color drawColor, float rotation = 0, float scale = 0, BlendState blendstate = null, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            ECSParticleBehavior p = ECSParticleDataManager.PAICollection[Type];
            BlendState curState = blendstate is null ?  BlendState.AlphaBlend : blendstate;
            if (curState == BlendState.AlphaBlend)
            {
                if (ECSParticleDataManager.activePoint_alpha < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_alpha[ECSParticleDataManager.activePoint_alpha];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, rotation, scale, ai0, ai1, ai2);
                    ECSParticleDataManager.activePoint_alpha++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_alpha - 1;
                }
            }
            else if (curState == BlendState.Additive)
            {
                if (ECSParticleDataManager.activePoint_add < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_add[ECSParticleDataManager.activePoint_add];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, rotation, scale, ai0, ai1, ai2);
                    ECSParticleDataManager.activePoint_add++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_add - 1;
                }
            }
            else
            {
                if (ECSParticleDataManager.activePoint_Nonmult < ECSParticleDataManager.MaxParticle)
                {
                    ref ECSParticleData particleDate = ref ECSParticleDataManager.particleData_nopremult[ECSParticleDataManager.activePoint_Nonmult];
                    ReSetParticleInfo(ref particleDate, Type, timeLeft, position, velocity, drawColor, rotation, scale, ai0, ai1, ai2);
                    ECSParticleDataManager.activePoint_Nonmult++;
                    p.OnSpawn(ref particleDate);
                    return ECSParticleDataManager.activePoint_Nonmult - 1;
                }
            }
            return -1;
        }
        public static void ReSetParticleInfo(ref ECSParticleData particleDate, int Type, int timeLeft, Vector2 position, Vector2 velocity, Color drawColor, float rotation = 0, float scale = 0, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            particleDate = ECSParticleDataManager.TempleParticle;

            particleDate.Lifetime = timeLeft;
            particleDate.Active = true;
            particleDate.Type = Type;
            particleDate.whoAmI = ECSParticleDataManager.activePoint_Nonmult;
            particleDate.Position = position;
            particleDate.Velocity = velocity;
            particleDate.DrawColor = drawColor;
            particleDate.Rotation = rotation;
            particleDate.Scale = scale;
            particleDate.aifloat0 = ai0;
            particleDate.aifloat1 = ai1;
            particleDate.aifloat2 = ai2;
            particleDate.aibool0 = false;
            particleDate.aibool2 = false;
            particleDate.aibool2 = false;
            particleDate.aiint0 = 0;
            particleDate.aiint1 = 0;
            particleDate.aiint2 = 0;
        }
    }
}
