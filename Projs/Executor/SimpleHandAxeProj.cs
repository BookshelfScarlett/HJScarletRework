using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SimpleHandAxeProj : HJScarletProj
    {
        public override string Texture => GetInstance<SimpleHandAxe>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 44;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 0;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 2;
        }
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(HJScarletSounds.Misc_KnifeToss[1] with { MaxInstances = 0, Pitch = -0.412f, Volume = 0.825f, PitchVariance = 0.15f }, Projectile.Center);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }
        public override void ProjAI()
        {
            Timer++;
            float maxTime = 15f;
            if (Timer > maxTime)
            {
                if (Timer > maxTime + 10f)
                {
                    Projectile.AffactedByGrav(0.93f, yMult: 1f, yAdd: 1.7f, maxGravSpeed: 57f);
                }
                else
                {
                    Projectile.AffactedByGrav(0.92f, yMult: 1f, yAdd: 0.57f, maxGravSpeed: 57f);
                }
                Projectile.rotation += Projectile.SpeedAffectRotation() / 12f;
            }
            else
            {
                Projectile.rotation += 0.21f;
            }
            UpdatePartilce();
        }

        public void UpdatePartilce()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f) / 4f;
            }
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(4f), DustID.SilverCoin, Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f));
                d.noGravity = true;
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f) / 4f;
                d.scale *= 1.1f;
            }
            if (Main.rand.NextBool(4))
            {
                new StarShape(Projectile.Center.ToRandCirclePosEdge(8f), Projectile.velocity / 8f, RandLerpColor(Color.White, Color.DarkGoldenrod), 0.45f, 40).Spawn();
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HJScarletSounds.GalvanizedHand_Hit with { Variants = [1], MaxInstances = 0, Pitch = 0.412f + Projectile.numHits * 0.1f, Volume = 0.925f }, Projectile.Center);
            Projectile.AddExecutionTimeImmediate(ItemType<SimpleHandAxe>());
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(15f), 0, 4.2f);
            }
            for (int i = 0; i < 16; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(4f), DustID.SilverCoin, -Projectile.velocity.ToRandVelocity(ToRadians(5f), 2f));
                d.noGravity = true;
                d.velocity = Projectile.velocity.ToRandVelocity(ToRadians(15f), 0, 4.2f);
                d.scale *= 1.1f;
            }

        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 17; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(4f), DustID.PlatinumCoin, RandVelTwoPi(0.4f, 7.7f));
                d.scale *= 1.294f;
                d.noGravity = true;
            }
            for (int i = 0; i < 24; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePosEdge(4f), DustID.GoldCoin, RandVelTwoPi(0.4f, 7.7f));
                d.scale *= 1.294f;
                d.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, posMove: 1.2f);
            Projectile.DrawProj(Color.White, 2);
            return false;
        }
    }
}
