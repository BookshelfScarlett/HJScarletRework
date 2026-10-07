using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.ParryShield
{
    public class CobaltParryShield : BaseParryShield
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.CobaltShield);
        protected override int TrailCounts => 14;
        protected override float ParryShieldScale => 1.5f;
        protected override void OnActuallyGoingParry(float progress, Vector2 tarPos, Vector2 offset)
        {
            if (Main.rand.NextFloat() < progress)
                return;
            float maxRange = ApplyParryShieldHorizonalRangeScale() * 90;
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + offset + tarPos.RotatedBy(TargetRotation) * maxRange, Main.rand.NextFloat(0.31f, .6f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                ECSParticle.HRShinyOrb(pos, vel, Color.White, 40, 1, 0.05f);
            }
            if (Main.rand.NextBool())
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + offset + tarPos.RotatedBy(TargetRotation) * maxRange, Main.rand.NextFloat(0.41f, .50f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.RoyalBlue, Color.LightBlue), 40, 1, Main.rand.NextFloat(.9f, 1.15f) * .2f, .2f);
            }
        }
        protected override void PostOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone, Vector2 parryDirection)
        {
            for (int i = 0; i < 26; i++)
            {
                Vector2 vel = parryDirection.ToRandVelocity(ToRadians(35f), .1f, 19f);
                ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * 10f, RandVelTwoPi(.3f, 14f), RandLerpColor(Color.WhiteSmoke, Color.White), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 20; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(6) + parryDirection * 4f, RandVelTwoPi(0.3f, 10.1f), RandLerpColor(Color.White, Color.WhiteSmoke), 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                Dust d = Dust.NewDustPerfect(target.Center, DustID.GemDiamond);
                d.velocity = RandVelTwoPi(1.2f, 6.2f) + parryDirection * 3f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.2f, 1.61f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 rotationPoint = tex.Size() / 2f;
            int length = OldParryShieldPos.Count;
            float lerp = ApplyParryShieldHorizonalRangeScale();

            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = i / (float)length;
                Vector2 pos = OldParryShieldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                float rot = OldParryShieldRot[i];
                float opac = Lerp(0f, 1f, ratios) * .954f;
                Color c = Color.Lerp(Color.WhiteSmoke, Color.RoyalBlue, ratios).ToAddColor(10);
                SB.FastDraw(tex, pos, c * opac * lerp, rot, rotationPoint, Projectile.scale, 0);

                /*
                主盾必须放在循环的最后一轮里绘制，
                才能保证它是最后一个被画上去的对象，从而压在所有残影之上。
                如果把它挪到循环外面，它会被先画，然后被所有残影覆盖，
                视觉上就会出现主盾好像退到了残影后面的错觉——
                位置本身没动，只是被盖住了。
                老实说，我也不知道为什么这样，丢到外面的话就会出现残影在主盾位置前方的问题。
                我仍然估计的是可能和取点位置有关系，但是格挡盾的运作速度本身足够快，判定也非常宽松，所以或许，并不需要进一步考虑细化了。
                */
                if (i == 0)
                {
                    pos = OldParryShieldPos[length - 1] - Main.screenPosition + Projectile.Size / 2f;
                    rot = OldParryShieldRot[length - 1];
                    for (int j = 0; j < 8; j++)
                    {
                        SB.FastDraw(tex, pos + (TwoPi / 8f * i).ToRotationVector2() * 1.1f, Color.White.ToAddColor() * lerp * lerp, rot, rotationPoint, Projectile.scale, 0);
                    }
                    SB.FastDraw(tex, pos, Color.White * lerp, rot, rotationPoint, Projectile.scale, 0);
                    SB.EnterShaderArea();
                    Texture2D tex2 = HJScarletTexture.Particle_Smear.Value;
                    float rot2 = rot + PiOver4 + (Projectile.direction < 0).ToInt() * -Pi;
                    float scale = Projectile.scale * .3f;
                    SB.FastDraw(tex2, pos, Color.RoyalBlue * ApplyParryShieldHorizonalRangeScale() * 1.5f, rot2, tex2.Size() / 2f, scale, 0);
                    SB.FastDraw(tex2, pos, Color.RoyalBlue * ApplyParryShieldHorizonalRangeScale() * 1.5f, rot2, tex2.Size() / 2f, scale*.9f, 0);
                    SB.FastDraw(tex2, pos, Color.CornflowerBlue * ApplyParryShieldHorizonalRangeScale() * 1.5f, rot2, tex2.Size() / 2f, scale * .8f, 0);
                    SB.FastDraw(tex2, pos, Color.AliceBlue * ApplyParryShieldHorizonalRangeScale() * 1.5f, rot2, tex2.Size() / 2f, scale * .7f, 0);
                    SB.EndShaderArea();
                }
            }
            return false;
        }
    }
}
