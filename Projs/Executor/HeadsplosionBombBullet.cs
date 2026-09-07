using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class HeadsplosionBombBullet : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = 3;
            Projectile.SetupImmnuity(-1);
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.AffactedByGrav(velMult: .975f, yMult: 1.02f, yAdd: 0.13f, maxGravSpeed: 45);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.HJScarlet().ExecutionStrikeManual)
            {
                if (Projectile.HJScarlet().CurStoredTarget.IsLegal() && Projectile.HJScarlet().CurStoredTarget.Equals(target))
                    return null;
                return false;
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 _, out SpriteEffects se);
            int length = Projectile.oldPos.Length / 4;

            for (int i = 0; i < 8; i++)
                SB.FastDraw(tex, drawPosition + (TwoPi / 8f * i).ToRotationVector2() * 2.5f, Color.Orange.ToAddColor(), drawRotation, tex.Size() / 2f, Projectile.scale, se);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, tex.Size() / 2f, Projectile.scale, se);
            return false;
        }
    }
}
