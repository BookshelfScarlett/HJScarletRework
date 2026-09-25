using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Buffs
{
    public class SaintChurchBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.moonLeech = true;
            if (player.velocity.Length() < 5f)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 pos = player.ToRandRec();
                    Vector2 dir = Vector2.UnitY.ToSafeNormalize() * -1f;
                    Vector2 vel = dir * Main.rand.NextFloat(0.3f, 1.7f);
                    int lifeTime = Main.rand.Next(60, 70);
                    float rot = RandRotTwoPi;
                    ECSParticle.SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, .48f, .35f, true, BlendState.NonPremultiplied);
                    ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.White, .1f)), lifeTime, rot, .6f, .3f, true, BlendState.NonPremultiplied);
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = player.ToRandRec();
                    Vector2 dir = player.velocity.ToSafeNormalize() * -1f;
                    Vector2 vel = dir * Main.rand.NextFloat(0.3f, 1.7f);
                    int lifeTime = Main.rand.Next(30, 70);
                    float rot = RandRotTwoPi;
                    {
                        ECSParticle.SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, .8f, .35f, true, BlendState.NonPremultiplied);
                        ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.White, .1f)), lifeTime, rot, 1f, .3f, true, BlendState.NonPremultiplied);
                    }
                    ECSParticle.LiliesFire(pos, vel.RotatedBy(ToRadians(15f) * i), Color.Black, lifeTime, RandRotTwoPi, 1, 0.3f, true);
                }
            }
        }
    }
}
