using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class TerraFlamethrowerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.HJScarlet().terraFlamethrowerDebuff = true;
            Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.TerraBlade);
            d.velocity = Vector2.UnitY * Main.rand.NextFloat(0.8f, 1.3f);
            d.noGravity = true;
            ECSParticle.HRShinyOrb(npc.ToRandRec(), Vector2.UnitY * Main.rand.NextFloat(.8f, 1.4f), RandLerpColor(Color.DarkGreen, Color.LightGreen), 40, 1f, 0.105f * .4f, 0.45f);
            base.Update(npc, ref buffIndex);
        }
    }
}
