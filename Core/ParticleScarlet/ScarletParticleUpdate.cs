using ReLogic.Threading;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace HJScarletRework.Core.ParticleScarlet
{
    public partial class ScarletParticleManager : ModSystem
    {
        public static void UpdatePriorityParticles()
        {
            UpdateParticleList(ParticleAlphaPriority);
            UpdateParticleList(ParticleAddPriority);
            UpdateParticleList(ParticleNonPrePriority);
        }
        public static void UpdateParticles()
        {
            UpdateParticleList(ParticleAlpha);
            UpdateParticleList(ParticleAdditive);
            UpdateParticleList(ParticleNonPre);
        }

        public static void UpdateParticleList(List<ScarletParticle> list)
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
    }
}
