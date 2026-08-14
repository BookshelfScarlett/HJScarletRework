using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class FruitofEternityBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.HJScarlet().fruitofEthernity = true;
        }
    }
}
