using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class EmeraldYoyoEmerald : HJScarletProj
    {
        public override string Texture => GetVanillaAssetPath(VanillaAsset.Item, ItemID.Emerald);
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);

        }
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.MaxUpdates = 4;
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(60);
            Projectile.timeLeft = 300 * Projectile.MaxUpdates;
            Projectile.penetrate = 3;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft -= 50 * Projectile.MaxUpdates;
            ScarletSound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), RandVelTwoPi(1f, 4f), RandLerpColor(Color.DarkGreen, Color.Green), 45, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .6f, .2f);
            }
            ECSParticle.HRShinyOrb(Projectile.Center, Vector2.Zero, Color.Lime, 40, 1, 0.2f, 0.4f);

            //Projectile.BounceOnTile(oldVelocity);
            return true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void ProjAI()
        {
            float maxTime = Projectile.MaxUpdates * 60f;
            float progress = Clamp(Timer / maxTime, 0f, 1f);
            float lerpRotSpeed = Lerp(.15f, .01f, progress);
            //Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDir() * .01f, progress);
            Projectile.rotation += lerpRotSpeed;
            float speedMult = 10f;
            speedMult = 16f;
            Vector2 vecToPlayer = Owner.Center - Projectile.Center + new Vector2(0f, -180f);
            float playerDist = vecToPlayer.Length();
            //Slow down if near the player
            if (playerDist < 200f && speedMult > 8f)
            {
                speedMult = 2f;
            }
            //Move toward player if more than 70 pixels away
            if (playerDist > 70f)
            {
                vecToPlayer.Normalize();
                vecToPlayer *= speedMult;
                Projectile.velocity = (Projectile.velocity * 30f + vecToPlayer) / 31f;
            }
            //Move if still
            else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
            Timer++;
            if (Timer > maxTime)
            {
                Projectile.Kill();
            }
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(28))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(6), RandVelTwoPi(4), RandLerpColor(Color.LimeGreen, Color.DarkGreen), 45, 1, Projectile.scale * Main.rand.NextFloat(.75f, 1.15f) * .6f, .2f);
            if (Main.rand.NextBool(28))
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), RandVelTwoPi(4), Color.LimeGreen, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
            base.ProjAI();
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePosEdge(6), RandVelTwoPi(0, 2f), Color.LightGreen, 40, 1, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .75f, 0, 0.1f);
            }
            ECSParticle.HRShinyOrb(Projectile.Center, Vector2.Zero, Color.LimeGreen, 40, 1, 0.2f, 0.4f);
            int coinSpawn = -1;
            float chance = Main.rand.NextFloat();
            int stack = 1;
            if (chance < .5f)
            {
                coinSpawn = ItemID.SilverCoin;
                stack = 5;
            }
            else if (chance >= .5f && chance < .9f)
            {
                coinSpawn = ItemID.GoldCoin;
                stack = 3;
            }
            else
                coinSpawn = ItemID.PlatinumCoin;
            HJScarletMethods.ScarletSpawnItem(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Hitbox, coinSpawn, stack);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            int length = Projectile.oldPos.Length;
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.DarkGreen, 1f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.LimeGreen, 0.9f, 1f, 1.1f);
            DrawTrails(HJScarletTexture.Trail_BloomDualLine.Texture, Color.White, 1f, 1f, 1.1f);
            SB.EndShaderArea();
            SB.EndShaderArea();

            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .55f, (progress));
                float yMult = Lerp(1f, .55f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.DarkGreen, Color.LimeGreen, .63f), EaseInOutQuad(progress));
                float opac = Lerp(1f, .3f, EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.DarkGreen, Color.LimeGreen, .63f), EaseInOutQuad(progress));
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor(50) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .95f, 0);
                SB.FastDraw(projTex, oldPos, c.ToAddColor(0) * opac * .315f, oldRot, projTex.Size() / 2f, scale * .98f, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.White.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -0f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * .85f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.05f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = Main.rand.NextVector2Circular(1.5f, 1.5f);
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 9 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
