using HJScarletRework.Globals.Methods;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetCode.Content
{
    // 同步“武器技能键按下”那一帧，落地到 JustPressedWeaponSKill。
    // 发送端 HJNetUtils.SyncedWeaponSkill 已经写好，目前还没人调，要用时在按键那帧发一次。
    public class ReadWeaponSkill : BaseHJHandlePack
    {
        public override void Read(BinaryReader reader, int whoAmI)
        {
            byte playerIndex = reader.ReadByte();
            bool weaponSkill = reader.ReadBoolean();
            if (playerIndex < Main.maxPlayers && Main.player[playerIndex].active)
                Main.player[playerIndex].HJScarlet().JustPressedWeaponSKill = weaponSkill;
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = HJScarletRework.Instance.GetPacket();
                packet.Write(Type);
                packet.Write(playerIndex);
                packet.Write(weaponSkill);
                packet.Send(-1, whoAmI);
            }
        }
    }
}
