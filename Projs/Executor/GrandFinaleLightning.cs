using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Executor
{
    public class GrandFinaleLightning : HJScarletProj
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public List<Vector2> StoredCenter = [];
        public NPC CurTarget = null;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 50;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.width = Projectile.height = 30;
            Projectile.timeLeft = 50 * 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
            
        }
        public override void ProjAI()
        {
            if (CurTarget.IsLegal())
            {
                Projectile.HomingTarget(CurTarget.Center, -1, 20, 20);
            }
            StoredCenter.Add(Projectile.Center);
            if (StoredCenter.Count > 200)
                StoredCenter.RemoveAt(0);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            new LightningParticle(Projectile.Center.ToRandCirclePos(3f), Vector2.Zero, RandLerpColor(Color.RoyalBlue,Color.DodgerBlue), Main.rand.Next(25, 45), Projectile.rotation + PiOver2, Main.rand.NextFloat(0.3f, 0.44f),0).Spawn();
            new LightningGlow(Projectile.Center, Projectile.SafeDir(), Color.RoyalBlue, 40, 0.75f).Spawn();
            if (Main.rand.NextBool(20))
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(10), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.White), 40, 0, 1, 0.60f).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            new LightningParticle(Projectile.Center, Vector2.Zero, Color.RoyalBlue, 40, RandRotTwoPi, 0.5f * Main.rand.NextFloat(0.75f,0.95f), 2).Spawn();
            Vector2 spawnPos = Projectile.Center + Projectile.SafeDir() * 10f;
            //new CrossGlow(spawnPos, Color.RoyalBlue, 40, 1f, 0.25f,false).Spawn();
            new OpticalLineGlow(spawnPos, Color.RoyalBlue, 40, 1f, 0.20f).Spawn();
            new BloomShockwave(spawnPos, Color.Lerp(Color.RoyalBlue,Color.White,0.2f), 40, 1f, 0.18f).Spawn();
            //new BloomShockwave(spawnPos, Color.Lerp(Color.RoyalBlue,Color.White,0.2f), 40, 1f, 0.18f,false).Spawn();
            for (int i = 0;i<6;i++)
            {
                new LightningParticle(spawnPos.ToRandCirclePos(64f), Vector2.Zero, RandLerpColor(Color.RoyalBlue, Color.DodgerBlue), Main.rand.Next(30, 40), RandRotTwoPi, 0.2f).Spawn();
            }
            for (int j = 0; j < 30; j++)
            {
                Vector2 vel = RandVelTwoPi(0f, 8f);
                //ShinyStardust.SpawnCircle(spawnPos.ToRandCirclePos(30f), vel, Main.rand.NextFloat(0.5f, .8f), 40);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //SB.EnterShaderArea();
            //DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.RoyalBlue);
            //DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.RoyalBlue);
            //DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.RoyalBlue);
            //SB.EndShaderArea();
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (StoredCenter.Count < 3)
                return;
            //做掉可能存在的零向量
            List<Vector2> validPosition = StoredCenter;
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j], drawColor, new Vector2(0, 5 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
