using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Core.NetSync
{
    public static class HJNetOwnerBound
    {
        private static readonly HashSet<int> _ownerBoundTypes = [];
        private static readonly HashSet<int> _keepAfterDeath = [];

        public static void Register(params int[] projTypes)
        {
            for (int i = 0; i < projTypes.Length; i++)
            {
                if (projTypes[i] > 0)
                    _ownerBoundTypes.Add(projTypes[i]);
            }
        }

        public static void KeepAfterDeath(params int[] projTypes)
        {
            for (int i = 0; i < projTypes.Length; i++)
            {
                if (projTypes[i] > 0)
                    _keepAfterDeath.Add(projTypes[i]);
            }
        }

        public static bool IsOwnerBound(int projType) => _ownerBoundTypes.Contains(projType);

        public static bool CanKillAfterDeath(int projType) => !_keepAfterDeath.Contains(projType);

        public static bool CanSpawnChild(this Projectile proj)
        {
            if (!proj.IsOwnerSide())
                return false;
            Player owner = proj.OwnerOrNull();
            return owner is not null && !owner.dead;
        }

        public static bool IsStale(this Projectile proj)
        {
            if (!IsOwnerBound(proj.type))
                return false;
            Player owner = proj.OwnerOrNull();
            if (owner is null)
                return true;
            return owner.dead && CanKillAfterDeath(proj.type);
        }

        internal static void Clear()
        {
            _ownerBoundTypes.Clear();
            _keepAfterDeath.Clear();
        }
    }
}
