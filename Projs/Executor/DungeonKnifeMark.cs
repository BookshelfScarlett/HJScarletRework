using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Graphics.Particles;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DungeonKnifeMark : KnifeMarkClass
    {
        public override Color BackgroundColor => Color.White;
        public override void ExtraFirstFrame()
        {
            Vector2 spawnPos = Projectile.Center;
            for (int i = 0; i < 6; i++)
            {
                Color color = RandLerpColor(Color.SkyBlue, Color.White);
                new NoiseShockRing(spawnPos, Vector2.Zero, color, 45, 1f, .13f + i * 0.2f, -1, Vector2.Zero, false).Spawn();
            }
            for (int i = 0; i < 50; i++)
                ECSParticle.TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(30), Main.rand.NextFloat(1.2f, 2.4f) * 2, RandLerpColor(Color.SkyBlue, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
            ScarletSound(HJScarletSounds.Misc_Spell, Projectile.Center, 0.45f);

        }
        public override void ExProjAI()
        {
        }
    }
}
