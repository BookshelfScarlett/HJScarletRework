using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Magic
{
    public class LivingBoomerangMagicLeaf : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Projectile, ProjectileID.Leaf);
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(10);
        }
        public override void ExSD()
        {
            Projectile.MaxUpdates = 2;
            Projectile.SetupImmnuity(Projectile.MaxUpdates * 15);
            Projectile.penetrate = 6;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void OnFirstFrame()
        {
            Projectile.frame = Main.rand.Next(0, 5);
            //记录中心点（初始位置）
            Projectile.localAI[1] = Projectile.Center.X;
            Projectile.localAI[2] = Projectile.Center.Y;
            //起始角度取当前速度方向
            Projectile.ai[0] = Projectile.velocity.ToRotation();
            //起始半径
            Projectile.ai[1] = 20f;

            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.localAI[0]++;

            Vector2 center = new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
            Projectile.ai[1] += 1.2f;
            float radius = Projectile.ai[1];

            float targetLinearSpeed = 8f;
            float angularSpeed = targetLinearSpeed / Max(radius, 1f);
            Projectile.ai[0] += angularSpeed;
            float angle = Projectile.ai[0];

            Vector2 targetPos = center + angle.ToRotationVector2() * radius;

            Vector2 finalSpeed = targetPos - Projectile.Center;
            Projectile.velocity = targetPos - Projectile.Center;

            if (Projectile.velocity.LengthSquared() > 13f * 13f)
                Projectile.velocity = finalSpeed.ToSafeNormalize() * 13f;

            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.IsOutScreen())
                return;
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.JungleGrass);
            d.scale = Main.rand.NextFloat(0.8f, 1.15f) * .9f;
            d.velocity *= .9f;
            d.noGravity = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Projectile.ApplyDirectionOverride(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(4), 0.9f, Color.Lime, 40, 1, 0.1f, glowMult: .1f);
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle frames = Projectile.GetTexture().Frame(1, 5, 0, Projectile.frame);
            Vector2 origin = frames.Size() / 2;
            Texture2D tex = Projectile.GetTexture();
            int length = Projectile.oldPos.Length - 1;


            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .85f, (progress));
                float yMult = Lerp(1f, .85f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.LimeGreen, Color.Lerp(Color.White, Color.Green, .93f), EaseInOutQuad(progress));
                Color glowColor = Color.Lerp(Color.LimeGreen, Color.Green, EaseInOutQuad(progress));
                float opac = Lerp(1f, .96f, EaseInOutExpo(progress));
                SB.Draw(tex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), frames, glowColor.ToAddColor(10) * opac * 0.821f, oldRot, origin, scale * 1.05f, 0, 0);
                SB.Draw(tex, oldPos, frames, c.ToAddColor(150) * opac * 1.1f, oldRot, origin, scale, 0, 0);
            }

            Vector2 pos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 8; i++)
                SB.Draw(tex, pos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 2f, frames, Color.White.ToAddColor(), Projectile.rotation, origin, Projectile.scale, 0, 0);
            SB.Draw(tex, pos, frames, Color.White, Projectile.rotation, origin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
