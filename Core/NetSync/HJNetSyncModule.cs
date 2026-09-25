using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Core.NetSync
{
    public abstract class HJNetSyncModule : ModSystem
    {
        public abstract byte MessageType { get; }

        public abstract void Handle(BinaryReader reader, int sender);

        public sealed override void Load()
        {
            HJNetRoute.Register(this);
            OnModuleLoad();
        }

        public sealed override void Unload()
        {
            HJNetRoute.Unregister(this);
            OnModuleUnload();
        }

        protected virtual void OnModuleLoad() { }
        protected virtual void OnModuleUnload() { }

        protected ModPacket Start() => HJNetRoute.Start(MessageType);

        protected void RelayToOthers(BinaryReader reader, int sender)
        {
            if (Main.netMode != NetmodeID.Server)
                return;
            long pos = reader.BaseStream.Position;
            int len = (int)(reader.BaseStream.Length - pos);
            byte[] body = reader.ReadBytes(len);
            reader.BaseStream.Position = pos;
            ModPacket pack = HJNetRoute.Start(MessageType);
            pack.Write(body, 0, body.Length);
            pack.Send(-1, sender);
        }

        protected static void SendToServer(ModPacket pack)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;
            pack.Send();
        }

        protected static void SendToClients(ModPacket pack, int to = -1, int except = -1)
        {
            if (Main.netMode != NetmodeID.Server)
                return;
            pack.Send(to, except);
        }
    }

    public static class HJNetRoute
    {
        public const byte RouteTag = 0x68;

        private static readonly Dictionary<byte, HJNetSyncModule> _modules = [];

        internal static void Register(HJNetSyncModule module) => _modules[module.MessageType] = module;
        internal static void Unregister(HJNetSyncModule module) => _modules.Remove(module.MessageType);
        internal static void Clear() => _modules.Clear();

        public static ModPacket Start(byte messageType)
        {
            ModPacket pack = HJScarletRework.Instance.GetPacket();
            pack.Write(RouteTag);
            pack.Write(messageType);
            return pack;
        }

        public static bool TryDispatch(BinaryReader reader, int sender)
        {
            if (reader is null)
                return false;
            long start = reader.BaseStream.Position;
            if (reader.ReadByte() != RouteTag)
            {
                reader.BaseStream.Position = start;
                return false;
            }
            byte messageType = reader.ReadByte();
            if (_modules.TryGetValue(messageType, out HJNetSyncModule module))
                module.Handle(reader, sender);
            return true;
        }
    }

    public class HJNetPlayerVar : HJNetSyncModule
    {
        public const byte VarMessage = 1;

        internal static readonly Dictionary<ushort, Action<BinaryReader, int>> Appliers = [];

        public override byte MessageType => VarMessage;

        protected override void OnModuleUnload() => Appliers.Clear();

        public static void RegisterApplier(ushort key, Action<BinaryReader, int> apply) => Appliers[key] = apply;

        public static void Push(ushort key, int owner, Action<ModPacket> writeBody)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket pack = HJNetRoute.Start(VarMessage);
            pack.Write(key);
            pack.Write((byte)owner);
            writeBody?.Invoke(pack);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                pack.Send();
            else
                pack.Send(-1);
        }

        public override void Handle(BinaryReader reader, int sender)
        {
            if (Main.netMode == NetmodeID.Server)
                RelayToOthers(reader, sender);
            ushort key = reader.ReadUInt16();
            byte owner = reader.ReadByte();
            if (Appliers.TryGetValue(key, out Action<BinaryReader, int> apply))
                apply(reader, owner);
        }
    }
}
