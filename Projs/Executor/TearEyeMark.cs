using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Graphics.Particles;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    /// <summary>
    /// 这个标记是隐形标记，没有任何作用
    /// <br>鞭类的效果后续再进行修改</br>
    /// </summary>
    public class TearEyeMark : KnifeMarkClass
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
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
            Owner.AddBuff(BuffType<TearEyeBuff>(), 2);
            Projectile.Center = Owner.MountedCenter;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    /// <summary>
    /// 这个标记是隐形标记，没有任何作用
    /// <br>鞭类的效果后续再进行修改</br>
    /// </summary>
    public class StarofHoperMark : KnifeMarkClass
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExtraFirstFrame()
        {
            Vector2 spawnPos = Projectile.Center;
            for (int i = 0; i < 6; i++)
            {
                Color color = RandLerpColor(Color.Gold, Color.White);
                new NoiseShockRing(spawnPos, Vector2.Zero, color, 45, 1f, .13f + i * 0.2f, -1, Vector2.Zero, false).Spawn();
            }
            for (int i = 0; i < 50; i++)
                ECSParticle.TurbulenceShinyOrb(spawnPos.ToRandCirclePosEdge(30), Main.rand.NextFloat(1.2f, 2.4f) * 2, RandLerpColor(Color.Gold, Color.White), 120, 1, Main.rand.NextFloat(.9f, 1.15f) * .13f);
            ScarletSound(HJScarletSounds.Misc_Spell, Projectile.Center, 0.45f);
        }
        public override void ExProjAI()
        {
            Owner.AddBuff(BuffType<StarofHopeBuff>(), 2);
            Projectile.Center = Owner.MountedCenter;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

}
