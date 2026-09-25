using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Core.NetSync
{
    public static class HJHeldVisual
    {
        private static readonly Dictionary<int, int> _projToItem = [];
        private static readonly HashSet<int> _keepAfterDeath = [];

        public static bool TrySpawnOwnerVisual(Player player, Item item, Vector2? spawnPos = null,
            float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, Action<Projectile> afterSpawn = null, int? damage = null)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            if (player is null || player.whoAmI != Main.myPlayer)
                return false;
            if (!player.active || player.dead)
                return false;
            int shoot = item.shoot;
            if (shoot <= 0)
                return false;
            if (player.HasProj(shoot))
            {
                Register(item.type, shoot);
                return false;
            }
            Register(item.type, shoot);
            int projDamage = (int)player.GetTotalDamage(item.DamageType).ApplyTo(item.damage);
            Vector2 position = spawnPos ?? player.Center;
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), position, Vector2.Zero,
                shoot, damage ?? projDamage, item.knockBack, player.whoAmI, ai0, ai1, ai2);
            proj.originalDamage = projDamage;
            proj.netUpdate = true;
            afterSpawn?.Invoke(proj);
            return true;
        }

        public static void Register(int itemType, int projType)
        {
            if (itemType <= 0 || projType <= 0)
                return;
            _projToItem[projType] = itemType;
        }

        public static void KeepAfterDeath(int projType) => _keepAfterDeath.Add(projType);

        public static bool IsHeldVisual(int projType) => _projToItem.ContainsKey(projType);

        public static int OwnerItemTypeOf(int projType) => _projToItem.TryGetValue(projType, out int item) ? item : 0;

        public static bool CanKillAfterDeath(int projType) => !_keepAfterDeath.Contains(projType);

        public static void RebuildIndex()
        {
            _projToItem.Clear();
            Item temp = new Item();
            bool[] flags = ScarletItemIDSets.IsHeldProjItem;
            for (int i = 0; i < flags.Length; i++)
            {
                if (!flags[i])
                    continue;
                temp.SetDefaults(i);
                if (temp.type != i || temp.shoot <= 0)
                    continue;
                _projToItem[temp.shoot] = i;
            }
        }

        internal static void Clear()
        {
            _projToItem.Clear();
            _keepAfterDeath.Clear();
        }
    }
}
