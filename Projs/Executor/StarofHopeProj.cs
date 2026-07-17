using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class StarofHopeProj : ExecutorWhipProj
    {
        public override int OriginalWhip => ItemType<StarofHope>();
        public override (Texture2D LineTexture, Color LineColor, int LineEndCut, bool FullBright) LineSetting => (
            TextureAssets.FishingLine.Value, Color.SkyBlue, HeadPosOffsetFactor, false);
        public override int HeadPosOffsetFactor => 2;
        public bool HasFireStar = false;
        public override void OnWhipActualSwinging(float swingProgress)
        {
            if (Projectile.IsMe() && !HasFireStar)
            {
                SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with { MaxInstances = 0, Pitch = 0.4f, Volume = 0.6f });
                for (int i = 0; i < 1; i++)
                {
                    Vector2 pos = Owner.Center - Vector2.UnitY * Main.rand.NextFloat(600, 700) + Main.rand.NextFloat(80, 120) * i * Vector2.UnitX * Main.rand.NextBool().ToDirectionInt() - ((Main.MouseWorld.X - Owner.Center.X) > 0).ToDirectionInt() * Vector2.UnitX * 100;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, pos.GetNormalVector2(Main.MouseWorld) * 30f, ProjectileType<StarofHopeStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                HasFireStar = true;
            }
        }
        public override void DrawWhipInPreDraw(List<Vector2> list, Texture2D texture, Rectangle frame, Vector2 originalOrigin, SpriteEffects flip)
        {
            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 origin = originalOrigin;
                int frameHeight = frame.Height + 4;
                float scale = 1f;

                //把手。
                if (i == 0)
                {
                    //原点略微上移，以确保抓到把手位置
                    origin.Y -= 4f;
                }
                else
                {
                    //这样。设置正确的帧图位置。
                    int segmentToDraw = 1;
                    frame.Y = frameHeight * segmentToDraw;
                }

                bool isHead = i == list.Count - 2;
                //如果是头结点（鞭末尾），准备设置一个从大到小的scale动画。
                if (isHead)
                {
                    //帧图中鞭末端的位置
                    frame.Y = frameHeight * 2 - 6;
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                    float t = Timer / timeToFlyOut;
                    scale = Lerp(0.5f, 1.35f, Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true));

                }
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;
                float rotation = diff.ToRotation() - PiOver2;
                //鞭末端才会无视亮度
                Color color = isHead ? Color.White : Lighting.GetColor(pos.ToTileCoordinates(), Color.White);
                //鞭末端是一个星星，这里会让其全局自转起来
                rotation += isHead ? (float)Main.timeForVisualEffects * .051f : 0;
                //不是把手，我们才描边
                if (i != 0)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        SB.Draw(texture, pos + (TwoPi / 16f * j).ToRotationVector2() * 1.5f - Main.screenPosition, frame, color.ToAddColor(), rotation, origin, scale, flip, 0);
                    }
                }
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
                pos += diff;
                if (isHead)
                {
                    SB.EnterShaderArea();
                    Texture2D tex2 = HJScarletTexture.Particle_KiraStar.Value;
                    //SB.Draw(tex, list[starIndex] - Main.screenPosition, null, Color.White, 0, tex.ToOrigin(), 0.92f * starScale,0,0);
                    SB.EndShaderArea();
                }
            }
        }
        public override void DrawMiscOnHead(Vector2 vector2, SpriteEffects flip)
        {

        }
        public override void ExOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.ExOnHitNPC(target, hit, damageDone);
        }
    }
    public class StarofHopeProj2 : HJScarletProj
    {
        public override string Texture => GetInstance<StarofHopeProj>().Texture;
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public List<Vector2> HeadPositionList = [];
        public override void ExSD()
        {
            Projectile.DefaultToWhip();
            Projectile.DamageType = ExecutorDamageClass.Instance;
            Projectile.WhipSettings.Segments = 12;
            Projectile.WhipSettings.RangeMultiplier = 0.6f;
            Projectile.extraUpdates = 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * .5f);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);
            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
            float t = Timer / timeToFlyOut;
            //获得这个hitbox。
            return HJScarletMethods.LineThroughRect(Projectile.Center, list[^2], targetHitbox);
        }
        public override void ProjAI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + PiOver2; // Without PiOver2, the rotation would be off by 90 degrees counterclockwise.

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            // Vanilla uses Vector2.Dot(Projectile.velocity, Vector2.UnitX) here. Dot Product returns the difference between two vectors, 0 meaning they are perpendicular.
            // However, the use of UnitX basically turns it into a more complicated way of checking if the projectile's velocity is above or equal to zero on the X axis.
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            Timer++;

            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
            if (Timer >= timeToFlyOut || Owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            Owner.heldProj = Projectile.whoAmI;
            Owner.MatchItemTimeToItemAnimation();
            if (Timer == timeToFlyOut / 2)
            {
                // Plays a whipcrack sound at the tip of the whip.
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }
            {

                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                HeadPositionList.Add(points[points.Count - 2]);

            }

            // Spawn Dust along the whip path
            // This is the dust code used by Durendal. Consult the Terraria source code for even more examples, found in Projectile.AI_165_Whip.
            float swingProgress = Timer / timeToFlyOut;
            // This code limits dust to only spawn during the the actual swing.
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f)
            {

                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                if (Main.rand.NextBool())
                {
                    int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                    Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                    int dustType = DustID.Enchanted_Gold;
                    if (Main.rand.NextBool(3))
                        dustType = DustID.TintableDustLighted;

                    // After choosing a randomized dust and a whip segment to spawn from, dust is spawned.
                    Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);
                    dust.position = points[pointIndex];
                    dust.fadeIn = 0.3f;
                    Vector2 spinningPoint = points[pointIndex] - points[pointIndex - 1];
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                    // This math causes these dust to spawn with a velocity perpendicular to the direction of the whip segments, giving the impression of the dust flying off like sparks.
                    dust.velocity += spinningPoint.RotatedBy(Owner.direction * ((float)Math.PI / 2f));
                    dust.velocity *= 0.5f;
                }
                for (int i = 0; i < 2; i++)
                {
                    int pointIndex2 = Main.rand.Next(points.Count - 2, points.Count);
                    Rectangle spawnArea2 = Utils.CenteredRectangle(points[pointIndex2], new Vector2(30f, 30f));

                    Vector2 dir = points[pointIndex2] - points[pointIndex2 - 1];
                    Vector2 vel = dir.RotatedBy(Owner.direction * PiOver2);
                    ECSParticle.ShinyCrossStarECS(Main.rand.NextVector2FromRectangle(spawnArea2), vel * Main.rand.NextFloat(0.5f, 1.1f) * 0.153f, RandLerpColor(Color.LightGoldenrodYellow, Color.Gold), 40, 1, Main.rand.NextFloat(.4f, .7f), 0.2f);
                }
                for (int i = 0; i < 2; i++)
                {
                    int pointIndex2 = Main.rand.Next(points.Count - 3, points.Count);
                    Rectangle spawnArea2 = Utils.CenteredRectangle(points[pointIndex2], new Vector2(30f, 30f));

                    Vector2 dir = points[pointIndex2] - points[pointIndex2 - 1];
                    Vector2 vel = dir.RotatedBy(Owner.direction * PiOver2);
                    ECSParticle.HRShinyOrb(Main.rand.NextVector2FromRectangle(spawnArea2), vel * Main.rand.NextFloat(0.5f, 1.1f) * 0.953f, RandLerpColor(Color.LightGoldenrodYellow, Color.Gold), 40, 1, Main.rand.NextFloat(.4f, .7f) * .12f, 0.65f);
                }
            }
        }
        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            // If your whip has a long range and this line is poking out of the front, use list.Count - 2 instead of list.Count - 1.
            for (int i = 0; i < list.Count - 2; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.SkyBlue);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            //Main.DrawWhip_WhipBland(Projectile, list);
            // The code below is for custom drawing.
            // If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
            // However, you must adhere to how they draw if you do.

            // This is a copy of Main.DrawWhip_WhipBland
            // This drawing assumes that each frame is projectile is equal size.
            // For more control over drawing each segment, see ExampleWhipProjectileAdvanced.

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int totalSegments = Projectile.WhipSettings.Segments; // The number of segments this whip has.

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, 3);
            Vector2 originalOrigin = frame.Size() / 2f;
            Vector2 pos = list[0];

            int starIndex = -1;
            float starScale = -1;
            for (int i = 0; i < list.Count - 1; i++)
            {
                int frameHeight = frame.Height + 4;
                Vector2 origin = originalOrigin;
                float scale = 1f;

                // Handle
                if (i == 0)
                {
                    origin.Y -= 4f; // This will move where the handle is drawn so it can be more in the player's hand.
                }
                // Divide the middle of the whip (after the handle and before the head) by approximately 3 and use the middle segments in each third.
                // ExampleWhipProjectile has 20 segments, so the following will result in 1 handle, 6 segment 1s, 6 segment 2s, 6 segment 3s, and 1 head.
                else
                {
                    // First Segment
                    // At the start of the whip after the handle, the first segment is used.
                    int segmentToDraw = 1;

                    frame.Y = frameHeight * segmentToDraw; // Set the frame to the correct segment.
                }

                if (i == list.Count - 2)
                {
                    // This is the head of the whip.
                    frame.Y = frameHeight * 2 - 6;
                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
                    float t = Timer / timeToFlyOut;
                    scale = Lerp(0.5f, 1.35f, Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true));
                    starIndex = i;
                    starScale = scale;

                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Color.White;


                rotation += i == list.Count - 2 ? (float)Main.timeForVisualEffects * .1f : 0;
                //if (i == list.Count - 2)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        SB.Draw(texture, pos + (TwoPi / 16f * j).ToRotationVector2() * 1.5f - Main.screenPosition, frame, color.ToAddColor(), rotation, origin, scale, flip, 0);
                    }
                }
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
                pos += diff;
                if (i == list.Count - 2)
                {
                    SB.EnterShaderArea();
                    Texture2D tex = HJScarletTexture.Particle_KiraStar.Value;
                    //SB.Draw(tex, list[starIndex] - Main.screenPosition, null, Color.White, 0, tex.ToOrigin(), 0.92f * starScale,0,0);
                    SB.EndShaderArea();
                }
            }
            //DrawNebulaTrail(Color.White, 5, 5f);

            return false;
        }
        public void DrawNebulaTrail(Color trailColor, float height, float DrawScale)
        {
            if (HeadPositionList.Count < 3)
                return;
            float laserLength = 50;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(HJScarletTexture.Trail_ManaStreak.Size);
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, HJScarletTexture.Trail_ManaStreak.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -50);
            shader.Parameters["uColor"].SetValue(trailColor.ToVector4() * DrawScale);
            shader.Parameters["uFadeoutLength"].SetValue(0.1f);
            shader.Parameters["uFadeinLength"].SetValue(0.05f);
            shader.CurrentTechnique.Passes[0].Apply();
            DrawSetting sets = new(HJScarletTexture.Trail_ManaStreak.Value);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < HeadPositionList.Count - 1; i++)
            {
                if (HeadPositionList[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = HeadPositionList[i];
                float rot = (HeadPositionList[i + 1] - HeadPositionList[i]).ToRotation();
                date.Add(new(listPos, Color.White, new(0, height * DrawScale), rot));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
    }
}
