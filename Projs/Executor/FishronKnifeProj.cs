using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class FishronKnifeProj : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<FishronKnife>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            base.ProjAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.HasProj<FishronKnifeMark>())
                Projectile.AddExecutionTimeImmediate<FishronKnife>();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Vector2 offset = Vector2.Zero;
            drawPos -= offset;
            Texture2D tex = Projectile.GetTexture();
            Vector2 orig = tex.Size() / 2;
            float disapperRatios = 1;
            int length = (int)((Projectile.oldPos.Length - 2));
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = EaseInOutExpo(1 - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter() - offset;
                float rot = Projectile.oldRot[i];
                int addLerp = (int)(Lerp(0, 105, ratios));
                Color c = Color.Lerp(Color.LightGreen, Color.LimeGreen, ratios).ToAddColor((byte)addLerp) * (Lerp(0.14f, 1f, ratios));
                float scale = Projectile.scale * Lerp(.35f, 1f, ratios);
                SB.Draw(projTex, pos, null, c, rot + PiOver4, orig, scale, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + (TwoPi / 8 * i).ToRotationVector2() * 1.5f, null, Color.Green.ToAddColor(), Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            Color mainC = Color.Lerp(Color.White, Color.WhiteSmoke, disapperRatios);
            SB.Draw(projTex, drawPos, null, mainC, Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            return false;
        }

    }
}
