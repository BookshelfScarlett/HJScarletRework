using HJScarletRework.Globals.Database.IDSets;
using Terraria;

namespace HJScarletRework.Globals.Methods
{
    public static partial class HJScarletMethods
    {
        public static bool HasParryShieldProj(this Player player)
        {
            foreach (var p in Main.ActiveProjectiles)
            {
                if (p.owner != player.whoAmI)
                    continue;
                if (ScarletProjIDSets.IsParryShield[p.type])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
