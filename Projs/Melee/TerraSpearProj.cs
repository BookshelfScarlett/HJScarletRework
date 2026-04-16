using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class TerraSpearProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath + $"Proj_{nameof(TerraSpear)}";
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(36, 2);
        }
        public enum State
        {
            Striker,
            Skyfallen,
            None
        }
        public State AttackType
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float RandRot => ref Projectile.ai[2];
        public List<Vector2> TrailPosList = [];
        public List<float> TrailRotList = [];
        public float TotalTrailCounts = 36;
        public int TotalSpawnTime = 4;
        public int CurSpawnTime = 0;
        public bool GoSpawn = false;
        public int ArrowSpawnTime = 0;
        public NPC CurTarget = null;
        public int ProjDamage = 0;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            //必须得大几圈
            Projectile.width = Projectile.height = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates =6;
            Projectile.ownerHitCheck = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 150;
        }
        public override void FirstFrameAI()
        {
            Projectile.originalDamage = Projectile.damage;
            InitPosList();
            SpawnParticles();
        }

        private void SpawnParticles()
        {
            for (int i = 0; i < 80; i++)
            {
                Vector2 spawnPos = Projectile.Center;
                Vector2 vel = -Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.0f, 36.4f);
                Dust d = Dust.NewDustPerfect(spawnPos.ToRandCirclePos(4f), DustID.Terra);
                d.velocity = vel;
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
                d.noGravity = true;
            }
            for (int i = 0; i < 30; i++)
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f);
                Vector2 vel = -Projectile.velocity.ToRandVelocity(ToRadians(5f), 1.0f, 16.4f);
                new StarShape(spawnPos, vel, RandLerpColor(Color.Lime, Color.Green), 0.75f, 80).Spawn();
            }
        }
        public void InitPosList()
        {
            for (int i = 0; i < TotalTrailCounts; i++)
            {
                TrailPosList.Add(Vector2.Zero);
                TrailRotList.Add(0);
            }
        }


        public override void SpearAI()
        {
            UpdatePosList();
            UpdateProjAttack();
            UpdateParticles();
        }
        public void UpdatePosList()
        {
            TrailPosList.Add(Projectile.Center);
            TrailRotList.Add(Projectile.rotation);
            if (TrailPosList.Count > TotalTrailCounts)
                TrailPosList.RemoveAt(0);
            if (TrailRotList.Count > TotalTrailCounts)
                TrailRotList.RemoveAt(0);
        }

        private void UpdateProjAttack()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (AttackType == State.Skyfallen)
            {
                
                if(Projectile.GetTargetSafe(out NPC target, false, canPassWall:true) && Projectile.numHits < 1)
                {
                    Projectile.HomingTarget(target.Center, -1, 22f, 5f, 10f);
                }
            }
            else
            {
                if (Projectile.TooAwayFromOwner())
                    Projectile.Kill();
            }
            if (Projectile.penetrate == -1 && Projectile.damage == 0 && AttackType == State.Skyfallen)
            {
                DisapperAI();
                return;
            }
            if (Timer % 16f == 0 && AttackType == State.Striker)
                SpawnArrow();


        }

        private void UpdateParticles()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;

            bool spawnTrail = GoSpawn && AttackType == State.Striker || AttackType == State.Skyfallen;
            if (Timer % (3f) == 0 && spawnTrail)
                SpawnPortals();

            if (Main.rand.NextBool())
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePos(10f) - Projectile.SafeDir() * 20f;
                new ShinyOrbParticle(spawnPos, Projectile.velocity / 4f, RandLerpColor(Color.Lime, Color.LightGreen), 130, 0.846f).Spawn();
            }
            if (Main.rand.NextBool())
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePos(5f) - Projectile.SafeDir() * 20f, Projectile.velocity / 4f, RandLerpColor(Color.Green, Color.Lime), 60, Projectile.rotation, 1f, 0.54f, 0.2f).Spawn();
            }

        }

        private void SpawnArrow()
        {
            if (ArrowSpawnTime > 5)
                return;
                SoundEngine.PlaySound(HJScarletSounds.HymnFireball_Release with { MaxInstances = 0, Volume = 0.6f }, Projectile.Center);
            for (int i = -1; i < 2; i+=2)
            {
                Vector2 vel = -Projectile.velocity.ToSafeNormalize().RotatedBy(ToRadians(10 * i)) * 40f * Main.rand.NextFloat(0.8f, 1.4f);
                Vector2 pos = Projectile.Center - Projectile.SafeDir() * 20f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, vel, ProjectileType<TerraSpearPortal>(), Projectile.originalDamage / 2, 1, Owner.whoAmI);
                ((TerraSpearPortal)proj.ModProjectile).PortalType = TerraSpearPortal.State.InitSpawnState;
            }
            ArrowSpawnTime += 1;
            
        }

        private void DisapperAI()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.2f);
            if (Projectile.Opacity <= 0.15f)
                Projectile.Kill();
        }

        private void SpawnPortals()
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 vel = -Projectile.velocity.ToSafeNormalize().RotatedBy(ToRadians(10 * i)) * 15f;
                    Vector2 pos = Projectile.Center - Projectile.SafeDir() * 40f;
                    new StarShape(pos + vel.ToSafeNormalize() * j * 6f, vel, RandLerpColor(Color.Green, Color.LimeGreen), 1 * Projectile.Opacity, 30).Spawn();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!GoSpawn)
            {
                GoSpawn = true;
                if (AttackType == State.Striker)
                {
                    SoundEngine.PlaySound(HJScarletSounds.SpearofEscape_Boom with { Pitch = -0.5f, MaxInstances = 0, Volume = 0.5f }, Projectile.Center);
                    ScreenShakeSystem.AddScreenShakes(Projectile.Center, 25f, 40, Projectile.rotation, ToRadians(20f));
                }
                else
                    SoundEngine.PlaySound(HJScarletSounds.Smash_AirHeavy[0] with { Pitch = -0.2f, MaxInstances = 2, Volume = 0.8f }, Projectile.Center);
            }
            SpawnDirectionParticle();
            SpawnDust(Projectile.Center, Projectile.SafeDir());
            
            if (CurSpawnTime > TotalSpawnTime)
                return;
            if (Projectile.HJScarlet().GlobalTargetIndex == -1 || Projectile.HJScarlet().GlobalTargetIndex == target.whoAmI)
            {
                float randRot = RandRotTwoPi;
                Vector2 spawnProjPos = target.Center - Vector2.UnitX.RotatedBy(randRot + (ToRadians(10 * CurSpawnTime))) * Main.rand.NextFloat(1000f, 1300f);
                Vector2 dir = HJScarletMethods.PredictAimToTarget(spawnProjPos, target.Center, target.velocity, 22f, 0);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnProjPos, dir, Type, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                ((TerraSpearProj)proj.ModProjectile).AttackType = State.Skyfallen;
                    proj.penetrate = 4;
                proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                if (AttackType == State.Skyfallen)
                {
                    ((TerraSpearProj)proj.ModProjectile).CurSpawnTime = CurSpawnTime + 1;
                }
                else
                    proj.ai[2] = randRot;
            }

        }
        public override bool? CanHitNPC(NPC target)
        {
                return null;
        }
        private void SpawnDirectionParticle()
        {
            for (int i = 0; i < 80; i++)
            {
                Vector2 spawnPos = Projectile.Center;
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(10f), 1.0f, 36.4f);
                Dust d = Dust.NewDustPerfect(spawnPos.ToRandCirclePos(4f), DustID.Terra);
                d.velocity = vel;
                d.scale = Main.rand.NextFloat(0.8f, 1.2f);
                d.noGravity = true;
            }

        }
        private static void SpawnDust(Vector2 spawnPos, Vector2 dir)
        {
            float numberOfDusts = 36f;
            for (int i = 0; i < numberOfDusts; i++)
            {
                new ShinyOrbParticle(spawnPos.ToRandCirclePos(16f), RandVelTwoPi(6f), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, 0.8f).Spawn();
            }
            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustPerfect(spawnPos + Main.rand.NextVector2CircularEdge(10f, 10f), DustID.Terra);
                d.velocity = RandVelTwoPi(6f);
                d.scale = Main.rand.NextFloat(1.4f, 1.8f);
                d.noGravity = true;
            }
            for (int i = 0; i < 15; i++)
            {
                Color Firecolor = RandLerpColor(Color.LimeGreen, Color.DarkGreen);
                new SmokeParticle(spawnPos, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.3f).SpawnToPriorityNonPreMult();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float rotFixer = PiOver4;
            //DrawStarShapeTrail();
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkGreen, 1.26f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.GreenYellow, 0.8f, 1f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.58f);
            SB.EndShaderArea();
            Vector2 drawOffset = Projectile.SafeDirByRot() * 100f;
            

            SB.Draw(projTex, drawPos - drawOffset, null, Color.White * Projectile.Opacity, Projectile.rotation + rotFixer, ori, Projectile.scale, 0, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -5.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(0.8f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            Vector2 drawOffset = Projectile.SafeDirByRot() * 50f;
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset - drawOffset , drawColor, new Vector2(0, 20 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
        public void DrawStarShapeTrail()
        {
            Texture2D starShape = HJScarletTexture.Particle_ShinyOrb.Value;
            Vector2 scale = new Vector2(.65f, 1.4f);
            List<Vector2> PosList = TrailPosList;
            List<float> RotList = TrailRotList;
            for (int i = 0; i < PosList.Count; i++)
            {
                if (PosList[i] == Vector2.Zero)
                    continue;
                float rads = 1f - i / (float)PosList.Count;
                Color drawColor = (Color.Lerp(Color.LimeGreen, Color.Green, rads) with { A = 50 }) * 0.9f * Projectile.Opacity * (1 - rads);
                Vector2 shapeScale = scale * Clamp(i / (PosList.Count - 4f), 0f, 1f);
                Vector2 lerpPos = PosList[i] - Main.screenPosition - Projectile.SafeDir() * 20f;
                if (shapeScale.X > 0.3f && shapeScale.Y > 0.5f)
                {
                    SB.Draw(starShape, lerpPos, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                    SB.Draw(starShape, lerpPos - Projectile.SafeDir() * 10f, null, drawColor, RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale, 0, 0);
                    SB.Draw(starShape, lerpPos - Projectile.SafeDir() * 10f, null, Color.White.ToAddColor(50), RotList[i] + PiOver2, starShape.ToOrigin(), shapeScale * 0.35f, 0, 0);
                }
            }
        }
    }
}
