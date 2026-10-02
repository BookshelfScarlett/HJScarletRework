using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Systems.Conditions
{
    public class ScarletDownedBoss : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
        }
        internal static bool _downedEOW = false;
        internal static bool _downedBOC = false;
        internal static bool _downedBloodMoon = false;
        public static bool DownedEvilBosses => DownedBOC || DownedEOW;
        public static bool DownedEOW
        {
            get => _downedEOW;
            set
            {
                if (!value)
                    _downedEOW = false;
                else
                    NPC.SetEventFlagCleared(ref _downedEOW, -1);
            }
        }
        public static bool DownedBOC
        {
            get => _downedBOC;
            set
            {
                if (!value)
                    _downedBOC = false;
                else
                    NPC.SetEventFlagCleared(ref _downedBOC, -1);
            }
        }
        public static bool DownedBloodMoon
        {
            get => _downedBloodMoon;
            set
            {
                if (!value)
                    _downedBloodMoon = false;
                else
                    NPC.SetEventFlagCleared(ref _downedBloodMoon, -1);
            }
        }
        public static void ResetAllFlags()
        {
            DownedEOW = false;
            DownedBOC = false;
            DownedBloodMoon = false;
        }
        public override void OnWorldLoad() => ResetAllFlags();

        public override void OnWorldUnload() => ResetAllFlags();
        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();
            // 肉前击败的boss
            if (DownedEOW)
                downed.Add("ScarletEater");
            if (DownedBOC)
                downed.Add("ScarletBrain");
            if (DownedBloodMoon)
                downed.Add("ScarletBloodMoon");
            tag["ScarletDownedFlags"] = downed;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("ScarletDownedFlags");
            DownedEOW = downed.Contains("ScarletEater");
            DownedBOC = downed.Contains("ScarletBrain");
            DownedBloodMoon = downed.Contains("ScarletBloodMoon");
        }
        #region 网络同步
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte net1 = new BitsByte();
            // 一个比特=8个字节，如果有部分字节暂时用不上，这些字节是一定得用各种方法占用掉让其形成一个完整的比特的
            // 不然发送的时候会有点问题
            net1[0] = DownedEOW;
            net1[1] = DownedBOC;
            net1[2] = DownedBloodMoon;
            net1[3] = false;
            net1[4] = false;
            net1[5] = false;
            net1[6] = false;
            net1[7] = false;
            writer.Write(net1);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte net1 = reader.ReadByte();
            DownedEOW = net1[0];
            DownedBOC = net1[1];
            DownedBloodMoon = net1[2];
            _ = net1[3];
            _ = net1[4];
            _ = net1[5];
            _ = net1[6];
            _ = net1[7];
        }
        #endregion
    }
}
