using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class GoldenAppleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.HJScarlet().goldenAppleDamageAbsorb = 20;
            player.lifeRegen += 2;
            player.statDefense += 8;
            if (player.miscCounter % 3 == 0)
            {
                Dust d = Dust.NewDustPerfect(player.ToRandRec() + Vector2.UnitY * 10, DustID.GoldCoin);
                d.velocity = -Vector2.UnitY * 0.5f;
                d.noGravity = true;
            }
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
