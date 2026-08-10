using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class EndlessWarProj : HJScarletProj
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public enum State
        {
            Shoot,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(5);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.extraUpdates = 4;
            Projectile.SetupImmnuity(60);
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            if (AttackState == State.Shoot)
            {
                if (Timer > Projectile.MaxUpdates * 15f)
                {
                    AttackState = State.Return;
                    Projectile.netUpdate = true;
                    Timer = 0;
                }
            }
            else if (AttackState == State.Return)
            {
                Projectile.velocity *= .95f;
                float ratios = Clamp(Timer / 30f, 0, 1);
                Projectile.rotation += Lerp(ToRadians(5f), ToRadians(0f), ratios);
                if (ratios >= .99f)
                {
                    Projectile.Kill();
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            //爆开
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
