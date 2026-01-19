using HJScarletRework.Globals.ParticleSystem;
using Terraria;

namespace HJScarletRework.Core.ParticleSystem
{
    public static class ParticleUtilities
    {
        public static void AddToAlpha(this BaseParticle p)
        {
            if (!p.Important && BaseParticleManager.ActiveParticlesAlpha.Count > 10000)
                BaseParticleManager.ActiveParticlesAlpha.RemoveAt(0);
            BaseParticleManager.ActiveParticlesAlpha.Add(p);
        }

        public static void AddToADD(this BaseParticle p)
        {
            if (!p.Important && BaseParticleManager.ActiveParticlesAdditive.Count > 10000)
                BaseParticleManager.ActiveParticlesAdditive.RemoveAt(0);
            BaseParticleManager.ActiveParticlesAdditive.Add(p);
        }

        public static void AddToNP(this BaseParticle p)
        {
            if (!p.Important && BaseParticleManager.ActiveParticlesNonPremultiplied.Count > 10000)
                BaseParticleManager.ActiveParticlesNonPremultiplied.RemoveAt(0);
            BaseParticleManager.ActiveParticlesNonPremultiplied.Add(p);
        }

        public static void AddToPAlpha(this BaseParticle p)
        {
            if (!p.Important && BaseParticleManager.PriorityActiveParticlesAlpha.Count > 10000)
                BaseParticleManager.PriorityActiveParticlesAlpha.RemoveAt(0);
            BaseParticleManager.PriorityActiveParticlesAlpha.Add(p);
        }

        public static void AddToPADD(this BaseParticle p)
        {
            if (!p.Important && BaseParticleManager.PriorityActiveParticlesAdditive.Count > 10000)
                BaseParticleManager.PriorityActiveParticlesAdditive.RemoveAt(0);
            BaseParticleManager.PriorityActiveParticlesAdditive.Add(p);
        }

        public static void AddToPNP(this BaseParticle p)
        {
            if (!p.Important && BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Count > 10000)
                BaseParticleManager.PriorityActiveParticlesNonPremultiplied.RemoveAt(0);
            BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Add(p);
        }
        public static void ShowCurrentParticleCounts()
        {
            int totalCounts = BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Count + BaseParticleManager.PriorityActiveParticlesAdditive.Count + BaseParticleManager.PriorityActiveParticlesAlpha.Count +
                                BaseParticleManager.ActiveParticlesAdditive.Count + BaseParticleManager.ActiveParticlesNonPremultiplied.Count + BaseParticleManager.PriorityActiveParticlesAlpha.Count;
            Main.NewText("当前粒子数量：" + totalCounts);
        }
    }
}
