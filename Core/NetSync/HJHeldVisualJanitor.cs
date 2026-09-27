using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetSync
{
    public class HJHeldVisualJanitor : ModSystem
    {
        public override void PostSetupContent()
        {
            HJHeldVisual.RebuildIndex();
        }

        public override void Unload()
        {
            HJHeldVisual.Clear();
            HJNetOwnerBound.Clear();
        }

        public override void OnWorldLoad() => HJHeldVisual.RebuildIndex();

        public override void PostUpdateProjectiles()
        {
            List<Projectile> doomed = null;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.IsStale())
                {
                    (doomed ??= []).Add(proj);
                    continue;
                }
                if (!HJHeldVisual.IsHeldVisual(proj.type))
                    continue;
                Player owner = proj.OwnerOrNull();
                if (owner is null)
                {
                    (doomed ??= []).Add(proj);
                    continue;
                }
                if (owner.dead && HJHeldVisual.CanKillAfterDeath(proj.type))
                    (doomed ??= []).Add(proj);
            }
            if (doomed is null)
                return;
            for (int i = 0; i < doomed.Count; i++)
            {
                Projectile proj = doomed[i];
                if (!proj.active)
                    continue;
                proj.Kill();
            }
        }
    }
}
