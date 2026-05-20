using ReLogic.Threading;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace HJScarletRework.Core.ParticleScarlet
{
    public partial class ScarletParticleManager : ModSystem
    {
        #region 更新主要粒子
        public static void UpdatePriorityParticles()
        {
            //UpdateAlphaPriority();
            //UpdateNonPrePriority();
            //UpdateAdditivePriority();
            MetadataUpdateHandlerAttribute(ParticleAlphaPriority);
            MetadataUpdateHandlerAttribute(ParticleAddPriority);
            MetadataUpdateHandlerAttribute(ParticleNonPrePriority);
        }
        public static void MetadataUpdateHandlerAttribute(List<ScarletParticle> list)
        {
            if (list.Count == 0)
                return;
            FastParallel.For(0, list.Count, (j, k, callback) =>
            {
                for (int i = j; i < k; i++)
                {
                    ScarletParticle scarletParticle = list[i];
                    scarletParticle.Update();
                    scarletParticle.Position += scarletParticle.Velocity;
                    scarletParticle.Time++;
                }
            });
            list.RemoveAll(p =>
            {
                if (p.Time > p.Lifetime)
                {
                    p.OnKill();
                    p.ReturnToPool();
                    return true;
                }
                return false;
            });
        }
        public static void UpdateAlphaPriority()
        {
            if (ParticleAlphaPriority.Count == 0)
                return;
            for (int i = 0; i < ParticleAlphaPriority.Count; i++)
            {
                ParticleAlphaPriority[i].Update();
                ParticleAlphaPriority[i].Position += ParticleAlphaPriority[i].Velocity;
                ParticleAlphaPriority[i].Time++;
            }
            ParticleAlphaPriority.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    //记得归还。
                    particle.ReturnToPool();
                    return true;
                }
                return false;
            });
        }
        public static void UpdateNonPrePriority()
        {
            if (ParticleNonPrePriority.Count == 0)
                return;
            for (int i = 0; i < ParticleNonPrePriority.Count; i++)
            {
                ParticleNonPrePriority[i].Update();
                ParticleNonPrePriority[i].Position += ParticleNonPrePriority[i].Velocity;
                ParticleNonPrePriority[i].Time++;
            }
            ParticleNonPrePriority.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    particle.ReturnToPool();
                    return true;
                }
                return false;
            });
        }
        public static void UpdateAdditivePriority()
        {
            if (ParticleAddPriority.Count == 0)
                return;
            for (int i = 0; i < ParticleAddPriority.Count; i++)
            {
                ParticleAddPriority[i].Update();
                ParticleAddPriority[i].Position += ParticleAddPriority[i].Velocity;
                ParticleAddPriority[i].Time++;
            }
            ParticleAddPriority.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    particle.ReturnToPool();
                    return true;
                }
                return false;
            });
        }
        #endregion
        #region 更新常规粒子
        public static void UpdateParticles()
        {
            MetadataUpdateHandlerAttribute(ParticleAlpha);
            MetadataUpdateHandlerAttribute(ParticleAdditive);
            MetadataUpdateHandlerAttribute(ParticleNonPre);
            //UpdateAlpha();
            //UpdateNonPre();
            //UpdateAdditive();
        }
        public static void UpdateAlpha()
        {
            if (ParticleAlpha.Count == 0)
                return;
            for (int i = 0; i < ParticleAlpha.Count; i++)
            {
                ParticleAlpha[i].Update();
                ParticleAlpha[i].Position += ParticleAlpha[i].Velocity;
                ParticleAlpha[i].Time++;
            }
            ParticleAlpha.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    particle.ReturnToPool();
                    return true;
                }
                return false;
            });
        }
        public static void UpdateNonPre()
        {
            if (ParticleNonPre.Count == 0)
                return;
            for (int i = 0; i < ParticleNonPre.Count; i++)
            {
                ParticleNonPre[i].Update();
                ParticleNonPre[i].Position += ParticleNonPre[i].Velocity;
                ParticleNonPre[i].Time++;
            }
            ParticleNonPre.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    particle.ReturnToPool();
                    return true;
                }
                return false;
            });
        }
        public static void UpdateAdditive()
        {
            if (ParticleAdditive.Count == 0)
                return;
            for (int i = 0; i < ParticleAdditive.Count; i++)
            {
                ParticleAdditive[i].Update();
                ParticleAdditive[i].Position += ParticleAdditive[i].Velocity;
                ParticleAdditive[i].Time++;
            }
            ParticleAdditive.RemoveAll(particle =>
            {
                if (particle.Time >= particle.Lifetime)
                {
                    particle.OnKill();
                    particle.ReturnToPool();
                    return true;
                }
                return false;
            });
        }
        #endregion
    }
}
