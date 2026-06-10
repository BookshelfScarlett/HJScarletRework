using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class GoldenAppleBuffEnchanted : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[BuffType<GoldenAppleBuff>()] = true;
            player.HJScarlet().goldenAppleDamageAbsorb = 50;
            player.lavaImmune = true;
            player.lifeRegen += 4;
            player.statDefense += 10;
            if (player.miscCounter % 3 == 0)
            {
                Dust d = Dust.NewDustPerfect(player.ToRandRec() + Vector2.UnitY * 10, DustID.GoldCoin);
                d.velocity = -Vector2.UnitY * 0.5f;
                d.noGravity = true;
                d = Dust.NewDustPerfect(player.ToRandRec() + Vector2.UnitY * 10, DustID.HallowedTorch);
                d.velocity = -Vector2.UnitY * 0.5f;
                d.noGravity = true;
            }

        }
        public override void Update(NPC npc, ref int buffIndex)
        {
        }
    }
}
