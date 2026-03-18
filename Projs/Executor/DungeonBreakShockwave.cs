using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public abstract class ExecutorHammerShockwave : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public virtual int Hitbox => 100;
        public virtual int ImmunityTime => 30;
        public virtual int TotalHitTime => -1;
        public virtual int TotalLifeTime => 30;
        public override void ExSD()
        {
            Projectile.height = Projectile.width = Hitbox;
            Projectile.SetupImmnuity(ImmunityTime);
            Projectile.penetrate = TotalHitTime;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = TotalLifeTime;
            Projectile.extraUpdates = 0;
        }
        public sealed override void OnFirstFrame()
        {
            PlayParticleOnFirstFrame();
        }
        public sealed override void ProjAI()
        {
            PlayParticleOnProjAI();
        }
        public virtual void PlayParticleOnFirstFrame() { }
        public virtual void PlayParticleOnProjAI() { }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public sealed override bool ShouldUpdatePosition() => false;
        public sealed override bool PreDraw(ref Color lightColor)
        {
            PreDrawShockwaveInPreDraw(ref lightColor);
            return false;
        }
        public virtual void PreDrawShockwaveInPreDraw(ref Color lightColor) { }
    }
    public class BlazingStrikerShockwave : ExecutorHammerShockwave
    {
        public override int Hitbox => 45;
        public override void PlayParticleOnFirstFrame()
        {
            base.PlayParticleOnFirstFrame();
        }
        public override void PlayParticleOnProjAI()
        {
            base.PlayParticleOnProjAI();
        }
    }
    public class DungeonBreakShockwave : ExecutorHammerShockwave
    {
        public override int Hitbox => 50;
        public override void PlayParticleOnFirstFrame()
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 dir = Vector2.UnitX.RotatedBy(ToRadians(360f / 32 * i));
                Dust d = Dust.NewDustPerfect(Projectile.Center + dir * 30f, DustID.UnusedWhiteBluePurple);
                d.velocity *= Vector2.Zero;
                d.scale = 1.41f;
                //d.noGravity = true;
            }
            //new BloomShockwave(Projectile.Center, Color.RoyalBlue, 40, 1f, .1f).Spawn();
            //new EmptyRing(Projectile.Center, Vector2.Zero, Color.RoyalBlue, 40, 0.7f, 1f, fadeIn:true,altRing: Main.rand.NextBool()).Spawn();
        }
        public override void PlayParticleOnProjAI()
        {
            base.PlayParticleOnProjAI();
        }
    }
}
