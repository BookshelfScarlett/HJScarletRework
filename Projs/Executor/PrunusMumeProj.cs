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
    public class PrunusMumeProj : HJScarletProj
    {
        public override string Texture => GetInstance<PrunusMume>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            Return
        }
        public State AttackState
        {
            get => (State)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = -1;
            Projectile.scale = 0.8f;
            Projectile.noEnchantmentVisuals = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void OnFirstFrame()
        {
            if (Projectile.HJScarlet().ExecutionStrike)
            {
                SoundEngine.PlaySound(HJScarletSounds.Misc_Spell with { MaxInstances = 0 });
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * -4.5f, ProjectileType<PrunusMumeFlower>(), Projectile.damage, 0, Owner.whoAmI);
                proj.rotation = RandRotTwoPi;
                proj.timeLeft = GetSeconds(2);
                proj.HJScarlet().ExecutionStrike = true;
            }
        }
        public override void ProjAI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Jungle);
            switch(AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
            SpawnParticle();
        }

        public void SpawnParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.JunglePlants);
            d.velocity = Projectile.velocity / 8f;
            d.noGravity = true;
            if(Main.rand.NextBool(4))
            new SnowCloud(Projectile.Center.ToRandCirclePos(60), Projectile.velocity / 8f, RandLerpColor(Color.Green, Color.IndianRed), 40, 0, 0.5f, Main.rand.NextFloat(0.1f, 0.3f) * .8f).Spawn();

        }

        public void DoShoot()
        {
            Projectile.rotation += .12f;
            Timer++;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 10f * Projectile.MaxUpdates)
            {
                Projectile.frameCounter = 0;

            }
            if (Projectile.MeetMaxUpdatesFrame(Timer, 11))
            {
                AttackState = State.Return;
                Projectile.netUpdate = true;
                Projectile.velocity = Projectile.velocity * -.1f;
                Timer = 0;
            }
        }

        public void SpawnPetalFlower(Vector2 center)
        {
            int length = 16;
            for(int i =0;i<16;i++)
            {
                float rad = Projectile.rotation + ToRadians(360f / length * i);
                new SmokeParticle(center.ToRandCirclePos(5f), rad.ToRotationVector2() * 10f * Main.rand.NextFloat(), RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 40, RandRotTwoPi, 1, 0.49f,true).SpawnToPriority();
            }
            for(int i =0;i<30;i++)
            {
                new TurbulenceGlowOrb(center.ToRandCirclePos(60f), 2.2f, RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 120, .21f, RandRotTwoPi).Spawn();
                new TurbulenceGlowOrb(center.ToRandCirclePos(60f), 2.2f, RandLerpColor(Color.HotPink, Color.IndianRed), 120, .2f, RandRotTwoPi).Spawn();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackState == State.Shoot)
            {
                AttackState = State.Return;
                Projectile.netUpdate = true;
                Timer = 0;
                Projectile.BounceOnTile(oldVelocity);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { MaxInstances = 2, Pitch = 0.3f }, Projectile.Center);
                SpawnPetalFlower(Projectile.Center);
            }
            return false;
        }
        public void DoReturn()
        {
            Projectile.HomingTarget(Owner.Center, -1, 18f, 20f);
            Projectile.rotation += .12f;
            Projectile.tileCollide = false;
            if(Projectile.IntersectOwnerByDistance())
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<PrunusMume>());
            target.AddBuff(BuffID.Poisoned, GetSeconds(1));
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { MaxInstances = 2, Pitch = 0.3f }, Projectile.Center);
            if (Projectile.numHits < 1)
            {
                if (!Projectile.HJScarlet().ExecutionStrike)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * -9f, ProjectileType<PrunusMumeFlower>(), Projectile.damage, 0, Owner.whoAmI);
                    proj.rotation = RandRotTwoPi;
                    proj.timeLeft = GetSeconds(2);
                }
                SpawnPetalFlower(target.Center);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.Green);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
