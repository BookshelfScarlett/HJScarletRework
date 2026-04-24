using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class GrandFinaleProj : HJScarletProj
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            Homing,
            Return
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public AnimationStruct Helper = new(3);
        public ref float Timer => ref Projectile.ai[0];
        public NPC CurTarget = null;
        public int TotalHitCounter = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 4;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 16;
            Projectile.scale = 0.70f;
            Projectile.Opacity = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            Vector2 vel = Owner.ToMouseVector2();
            if (Projectile.HJScarlet().ExecutionStrike && !Owner.HasProj<GrandFinaleExecution>(out int projID))
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, -vel * 24f, projID, Projectile.damage, 1f, Owner.whoAmI);
            }
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }
        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }
        public void DoShoot()
        {
            Projectile.rotation += 0.18f;
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 10f))
            {
                State id = State.Return;
                Projectile.scale = 0.85f;
                Projectile.Opacity = 1f;
                UpdateNextAI(id);
            }

        }
        public void DoReturn()
        {
            Projectile.HomingTarget(Owner.Center, -1, 20, 20);
            Projectile.rotation += 0.18f;
            if (Projectile.IntersectOwnerByDistance(150))
            {
                Projectile.AddExecutionTime(ItemType<GrandFinale>());
                Projectile.Kill();
            }
        }
        public void UpdateNextAI(State id)
        {
            Timer = 0;
            AttackState = id;
            Projectile.netUpdate = true;
        }
        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(40);
                new HRShinyOrb(pos, Projectile.velocity / 4f, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 40, 0.1f).Spawn();
            }
            if (Main.rand.NextBool(8))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(40);
                new LightningParticle(pos, Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.Blue), 40, Projectile.velocity.ToRotation() + PiOver2 + Main.rand.NextFloat(-PiOver4, PiOver4), 0.4f).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 spawnPos = new Vector2(target.Center.X + Main.rand.NextFloat(50f, 200f) * Main.rand.NextBool().ToDirectionInt(), target.Center.Y - Main.rand.NextFloat(1200f, 1600f));
            Vector2 vel = (target.Center - spawnPos).ToSafeNormalize() * 18f;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, vel, ProjectileType<GrandFinaleLightning>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            SoundStyle pickSound2 = Utils.SelectRandom(Main.rand, HJScarletSounds.Smash_AirHeavy);
            SoundEngine.PlaySound(pickSound2 with { Pitch = Main.rand.NextFloat(0.3f, 0.5f), Volume = 0.7f, MaxInstances = 1 }, target.Center);
            if (target.IsLegal())
                ((GrandFinaleLightning)proj.ModProjectile).CurTarget = target;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White, rotFix: PiOver4);
            Projectile.DrawProj(Color.White, rotFix: PiOver4);
            return false;
        }
    }
}
