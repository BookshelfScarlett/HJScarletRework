using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class TheBleachingBuff : ModBuff
    {
        public static float HitDamageMult = .5f;
        public static float HitDamageMultEnemy = .05f;
        public static int BadLifeRegenEnemy = 1000;
        public static int BadLifeRegen = 100;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {

            npc.HJScarlet().theBleachingBuffEnemy = true;
            Vector2 pos = npc.Center.ToRandCirclePos(32);
            Vector2 dir = -Vector2.UnitY * 1f;
            Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7.7f);
            int lifeTime = Main.rand.Next(30, 70);
            float rot = RandRotTwoPi;
            {
                ECSParticle.SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, 0.61f, .35f, true, BlendState.Additive);
            }
            ECSParticle.LiliesFire(pos, vel, Color.White, lifeTime, RandRotTwoPi, 1, 0.3f, true, BlendState.Additive);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.HJScarlet().theBleachingBuff = true;

            base.Update(player, ref buffIndex);
        }
    }
}
