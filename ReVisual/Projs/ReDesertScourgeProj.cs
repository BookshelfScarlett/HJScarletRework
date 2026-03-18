using ContinentOfJourney.Projectiles.Meelee;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Utilities.Terraria.Utilities;

namespace HJScarletRework.ReVisual.Projs
{
    public class ReDesertScourgeProj : ReVisualProjClass
    {
        public override int ApplyProj => ProjectileType<DesertScourge>();
        public override int TotalListCount => 8;
        public bool IsFirstFrame = false;
        public float DustTimer = 0;
        public override bool ShouldApplyRevisual(Projectile proj, ReVisualPlayer vp)
        {
            return proj.IsMe() && vp.reVisualDesertScourge;
        }
        public override void RevisualUpdate(Projectile proj)
        {
            
            AddOldPosRotList(proj, proj.Center, proj.velocity.ToRotation());
            new SmokeParticle(proj.Center.ToRandCirclePosEdge(5f) - proj.SafeDir() * 40f, RandVelTwoPi(3f), RandLerpColor(Color.Brown, Color.Orange), 40, RandRotTwoPi, 0.75f, Main.rand.NextFloat(0.4f, 0.6f) * 0.32f, Main.rand.NextBool()).SpawnToNonPreMult();
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 dir = proj.SafeDir().RotatedBy(ToRadians(30) * -i);
                float speed = Main.rand.NextFloat(2f, 6.75f);
                Vector2 spawnPos = proj.Center + proj.SafeDir() * 30f + proj.SafeDir().RotatedBy(PiOver2 * i) * 10f;
                new ShinyOrbHard(spawnPos, dir.RotatedByRandom(ToRadians(10)) * -speed, RandLerpColor(Color.Brown, Color.SandyBrown), 40, 0.35f).Spawn();
            }
        }
        public override void PreDrawRevisual(Projectile proj, ref Color lightColor)
        {
            if (PosList.Count < 1 && RotList.Count < 1)
                return;
            Texture2D tex = proj.GetTexture();
            float rotFixer = ToRadians(135);
            Vector2 drawPos = proj.Center - Main.screenPosition;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                float colorRads = 0.9f * proj.Opacity * (1 - rads);
                Color drawColor2 = Color.Lerp(Color.Brown, Color.White, rads).ToAddColor(100) * colorRads * ((float)lightColor.A / 255f);
                SB.Draw(tex, PosList[i] - Main.screenPosition, null, drawColor2, RotList[i] + rotFixer, tex.ToOrigin(), proj.scale * (1f - rads) * 0.99f, 0, 0);
            }

            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(), proj.velocity.ToRotation() + rotFixer, tex.ToOrigin(), proj.scale, 0, 0);
            SB.Draw(tex, drawPos, null, lightColor, proj.velocity.ToRotation() + rotFixer, tex.ToOrigin(), proj.scale, 0, 0);
        }

    }
}
