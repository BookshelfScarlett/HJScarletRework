using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Projectile.ToTrailSetting(20, 2);
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
        public int TotalSpawnTime = 3;
        public int CurSpawnTime = 0;
        public bool GoSpawn = false;
        public int ArrowSpawnTime = 0;
        public ref float Timer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 5;
            Projectile.ownerHitCheck = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 150;
        }
        public override void FirstFrameAI()
        {
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
            if (Timer % 16f == 0 && AttackType == State.Striker)
                SpawnArrow();
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.penetrate == -1 && Projectile.damage == 0)
            {
                DisapperAI();
                return;
            }

        }

        private void UpdateParticles()
        {
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;

            bool spawnTrail = GoSpawn && AttackType == State.Striker || AttackType == State.Skyfallen;
            if (Timer % (3f) == 0 && spawnTrail)
                SpawnPortals();

            if (Main.rand.NextBool(2) && AttackType == State.Striker)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(6f, 6f) - Projectile.SafeDir() * 20f;
                    //new StarShape(pos, Projectile.velocity / 6f, RandLerpColor(Color.Green, Color.LimeGreen), 0.94f * Projectile.Opacity, 60).Spawn();
                }
            }
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
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, vel, ProjectileType<TerraSpearPortal>(), Projectile.damage / 2, 1, Owner.whoAmI);
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
                Vector2 dir = (spawnProjPos - target.Center).ToSafeNormalize() * -22f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnProjPos, dir, Type, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                ((TerraSpearProj)proj.ModProjectile).AttackType = State.Skyfallen;
                if (AttackType == State.Skyfallen)
                {
                    ((TerraSpearProj)proj.ModProjectile).CurSpawnTime = CurSpawnTime + 1;
                    proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                }
                else
                    proj.ai[2] = randRot;
            }

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
            DrawStarShapeTrail();
            Vector2 drawOffset = Projectile.SafeDirByRot() * 100f;
            SB.Draw(projTex, drawPos - drawOffset, null, Color.White * Projectile.Opacity, Projectile.rotation + rotFixer, ori, Projectile.scale, 0, 0);
            return false;
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
