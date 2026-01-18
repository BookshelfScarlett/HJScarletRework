using HJScarletRework.Core.Packets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        internal Vector2 oldSyncedMouseWorld;
        public Vector2 SyncedMouseWorld;

        internal bool OldMouseLeft;
        public bool MouseLeft;

        internal bool OldMouseRight;
        public bool MouseRight;

        public bool JustPressedWeaponSKill;
        public void UpdateNetPacket()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                SyncedMouseWorld = Main.MouseWorld;
                MouseLeft = Main.mouseLeft;
                MouseRight = Main.mouseRight;
            }
            if (SyncedMouseWorld != oldSyncedMouseWorld)
            {
                // 只在多人模式的客户端执行
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == Player.whoAmI)
                {
                    // 创建一个新的网络数据包
                    ModPacket packet = Mod.GetPacket();
                    // 写入一个自定义的消息类型，以便HandlePacket能识别
                    packet.Write((byte)HJScarletNetCode.MessageType.SyncMousePosition);
                    // 写入是哪个玩家发送的
                    packet.Write((byte)Player.whoAmI);
                    // 写入鼠标坐标
                    packet.WriteVector2(Main.MouseWorld);
                    // 发送给服务器
                    packet.Send();
                }
            }
            if (MouseLeft != OldMouseLeft)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == Player.whoAmI)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)HJScarletNetCode.MessageType.SyncMouseLeft);
                    packet.Write((byte)Player.whoAmI);
                    packet.Write(Main.mouseLeft);
                    packet.Send();
                }
            }
            if (MouseRight != OldMouseRight)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == Player.whoAmI)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)HJScarletNetCode.MessageType.SyncMouseRight);
                    packet.Write((byte)Player.whoAmI);
                    packet.Write(Main.mouseRight);
                    packet.Send();
                }
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                OldMouseLeft = Main.mouseLeft;
                OldMouseRight = Main.mouseRight;
                oldSyncedMouseWorld = Main.MouseWorld;
            }
        }

    }
}
