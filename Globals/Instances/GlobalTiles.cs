using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Useables;
using HJScarletRework.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletGlobalTiles : GlobalTile
    {
        public int LastTile = -1;
        public int CurChance = 0;
        //熔炼去把孩子
        public void SmeltOres(int i, int j, int chance, int type)
        {
            Vector2 tileSoundPos = new(i * 16, j * 16);
            Rectangle tileRec = new(i * 16, j * 16, 16, 16);
            //玩家上一个挖掘的矿石不是同类矿，刷新熔炼矿石的可能性
            if (type != LastTile)
                CurChance = 0;
            for (int k = 0; k < SmeltList.OreType.Count; k++)
            {
                //不是我想要的，直接跳过
                if (type == SmeltList.OreType[k])
                {
                    if (Main.rand.Next(chance) - CurChance <= 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item35 with { Volume = 0.7f }, tileSoundPos);
                        Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), tileRec, SmeltList.BarType[k]);
                        //触发熔炼效果则刷新概率次数
                        CurChance = 0;
                    }
                    //如果没触发熔炼效果，增加一次保底次数
                    else
                    {
                        CurChance++;
                    }
                    break;
                }
            }
        }
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                return;

            bool dummy = true;
            if (Main.gameMenu || fail || !CanKillTile(i, j, type, ref dummy))
                return;
            //抄的，我其实看不懂物块更新
            Player usPlayer = Main.LocalPlayer;
            var soulPlayer = usPlayer.HJScarlet();
            if (Main.netMode != NetmodeID.Server && Main.LocalPlayer == usPlayer && !usPlayer.CCed && !usPlayer.noBuilding && !usPlayer.HasBuff(BuffID.DrillMount) && !usPlayer.noItems)
            {
                if (usPlayer.HeldItem.type == ItemType<CorePickaxe>() && usPlayer.HeldItem.ModItem is CorePickaxe)
                {
                    for (int k = 0; k < SmeltList.OreType.Count; k++)
                    {
                        if (type == SmeltList.OreType[k])
                        {
                            noItem = true;
                            PacketOres(i, j, 3, type);
                            LastTile = type;
                            break;
                        }
                    }
                }
            }
        }
        public void PacketOres(int i, int j, int chance, int type)
        {
            //处于多人服务器的情况下得手动发送数据包了
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                int id = 20260221;
                ModPacket pack = GetInstance<HJScarletRework>().GetPacket();
                pack.Write(id);
                pack.Write((ushort)i);
                pack.Write((ushort)j);
                pack.Write((ushort)chance);
                pack.Write((ushort)type);
                pack.Send();
            }
            //其他情况就，随便吧
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                SmeltOres(i, j, chance, type);
            }
        }
    }
}
