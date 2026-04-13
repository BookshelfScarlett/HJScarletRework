using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class BlackKeyExecutionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage<ExecutorDamageClass>() += player.HJScarlet().blackKeyDefenseBuff;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.defense -= npc.HJScarlet().blackKeyDefensesReduces;
        }
    }
}
