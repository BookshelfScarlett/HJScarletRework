using ContinentOfJourney;
using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (ScarletItemIDSets.GrantsBoosterAfterSon[item.type] && item.IsExecutorWeapon() && DownedBossSystem.downedSon && player.HJScarlet().weaponUpgradePostSon)
                damage *= 10f;
            if (HJScarletConfigServer.Instance.ModWeaponDamageMult != 1f && item.damage > 0)
            {
                damage *= HJScarletConfigServer.Instance.ModWeaponDamageMult;
            }
        }
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(item, player, ref reduce, ref mult);
        }

    }
}
