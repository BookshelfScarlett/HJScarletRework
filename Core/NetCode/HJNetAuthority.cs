using Terraria;
using Terraria.ID;

namespace HJScarletRework.Core.NetCode
{
    // 谁在管这个射弹/玩家。约定：生成、伤害、特效、写玩家数据这些只在主人端（或单人）跑，
    // 队友端只负责表现同步过来的状态，专用服务器只转发不表现。
    public static class HJNetAuthority
    {
        // 玩家是不是本机玩家。专用服务器上没有本地玩家，所以这里一律 false，
        // 正好用来挡住服务器去跑客户端表现（粒子、音效、生成射弹）。
        public static bool IsOwnerSide(this Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            return player != null && player.whoAmI == Main.myPlayer;
        }
        // 射弹的 owner 槽位是不是本机玩家。挂载射弹、子弹、召唤物都用它做主人端闸门。
        public static bool IsOwnerSide(this Projectile proj)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            return proj.owner == Main.myPlayer;
        }
        // 队友端看到的同一个射弹（只读镜像），用来把“只该在主人端跑”的代码排除掉。
        public static bool IsRemoteMirror(this Projectile proj) => !proj.IsOwnerSide();
        // 取射弹的主人；owner 槽位无效、玩家已退出或槽位被回收时返回 null 而不是乱指人。
        public static Player OwnerOrNull(this Projectile proj)
        {
            int slot = proj.owner;
            if (slot < 0 || slot >= Main.maxPlayers)
                return null;
            Player owner = Main.player[slot];
            return owner != null && owner.active ? owner : null;
        }
        // 主人是否还存在且没死。召唤形态自己每帧查它，主人死了就自行 Kill，
        // 这样射弹不会带着旧 owner 索引活到另一个玩家身上。
        public static bool OwnerAlive(this Projectile proj)
        {
            Player owner = proj.OwnerOrNull();
            return owner != null && !owner.dead;
        }
        // 运行时再生成子射弹/召唤物（锤转召唤形态、挂载星、激光等）的统一条件：
        // 主人端 + 主人还活着。只写 IsOwnerSide 会导致主人刚死时空中父弹又把召唤物拉回来。
        public static bool CanSpawnChild(this Projectile proj) => proj.IsOwnerSide() && proj.OwnerAlive();
        // 显式问“现在是服务器吗”，比到处写 NetmodeID.Server 好检索。
        public static bool IsServerSide() => Main.netMode == NetmodeID.Server;
        // 单人模式（本地逻辑与特效正常全跑）。
        public static bool IsSingleSide() => Main.netMode == NetmodeID.SinglePlayer;
    }
}
