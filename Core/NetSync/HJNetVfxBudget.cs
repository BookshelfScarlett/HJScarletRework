using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetSync
{
    public static class HJNetVfxBudget
    {
        public const int RemoteBudgetPerFrame = 40;
        public const float RemoteLifeScale = 0.5f;
        public const int RemoteMinLife = 10;
        public const float RemoteCountScale = 0.4f;

        private static int usedThisFrame;

        public static void ResetFrame() => usedThisFrame = 0;

        public static bool Allow(this Projectile proj, int cost = 1)
        {
            if (!proj.IsRemoteMirror())
                return true;
            if (usedThisFrame + cost > RemoteBudgetPerFrame)
                return false;
            usedThisFrame += cost;
            return true;
        }

        public static int Count(this Projectile proj, int count)
        {
            if (!proj.IsRemoteMirror() || count <= 1)
                return count;
            return (int)System.Math.Max(1, count * RemoteCountScale);
        }

        public static int Life(this Projectile proj, int lifeTime)
        {
            if (!proj.IsRemoteMirror())
                return lifeTime;
            int cut = (int)(lifeTime * RemoteLifeScale);
            return lifeTime < RemoteMinLife ? lifeTime : System.Math.Max(RemoteMinLife, cut);
        }

        public static bool AllowSound(this Projectile proj) => !proj.IsRemoteMirror();
    }

    public class HJNetVfxBudgetTicker : ModSystem
    {
        public override void PostUpdateDusts() => HJNetVfxBudget.ResetFrame();
    }
}
