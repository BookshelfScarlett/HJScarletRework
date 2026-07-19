using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerBloodyBullet : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public ref float Timer => ref Projectile.ai[0];
        public enum State
        {
            Shoot,
            Homing
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public bool IntersectOwner = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 6;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.timeLeft = 300;
        }
        private Color GaiaStrikerBulletDataColor => Color.Lerp(Color.Red, Color.White, .21f) with { A = 255 };
        Vector2 RandVectorX = Vector2.Zero;
        public override void OnFirstFrame()
        {
            if (Projectile.ai[2] == 1)
            {
                RandVectorX = Main.rand.NextFloat(-10f, 10f) * Vector2.UnitY;
                Projectile.timeLeft = Main.rand.Next(30, 60) / 2 * Projectile.MaxUpdates;
            }
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (AttackState == State.Shoot)
            {
                float ratios = Clamp(Timer / 15f, 0, 1);
                for (int i = 0; i < 2; i++)
                    BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(7), Projectile.SafeDir() * ratios, Lerp(0.21f, 0.12f, (ratios)), Projectile.velocity.ToRotation(), false);
                BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(2), Projectile.SafeDir(), 0.10f * (EaseOutBack(ratios)), Projectile.rotation, true);
                Projectile.velocity *= .98f;
                if (Projectile.FinalUpdate())
                    Timer++;
                if (Timer > 20f)
                {
                    Timer = 0;
                    AttackState = State.Homing;
                    Projectile.timeLeft = 600;
                    Projectile.netUpdate = true;
                }
            }
            else if (AttackState == State.Homing)
            {
                float ratios = Clamp(Timer / 30f, 0f, 1f);
                float angle = Lerp(1f, 90f, ratios);
                float st = Lerp(0f, 35f, ratios);

                if (Projectile.FinalUpdate())
                    Timer++;
                for (int i = 0; i < 2; i++)
                    BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(7), Projectile.SafeDir(), 0.12f, Projectile.velocity.ToRotation(), false);

                BloodyMetaball.SpawnParticle(Projectile.Center, Projectile.SafeDir(), Lerp(0.12f, 0.17f, ratios), Projectile.rotation, true);
                if (Projectile.HJScarlet().HasExecutionMechanic && Projectile.ai[2] != 1)
                {
                    if (Projectile.GetTargetSafe(out NPC target, true, 1800f, true))
                    {
                        Projectile.HomingTarget(target.Center, -1, Lerp(6, 9, ratios), st, angle);
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.HomingTarget(Owner.Center, -1, Lerp(6, 9, ratios), st, angle);
                    if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                    {
                        switch (Projectile.ai[2])
                        {
                            //盖亚重锤普攻爆开时的治疗
                            case 0f:
                                Owner.ScarletHeal(GaiaStriker.BloodBulletHealNormal * 2, GaiaStrikerBulletDataColor);
                                break;
                            //盖亚重锤处决爆开时的治疗
                            case 1f:
                                Owner.ScarletHeal(GaiaStriker.BloodBulletHealNormal, GaiaStrikerBulletDataColor);
                                break;
                            //盖亚宝箱爆开时的治疗
                            case 2f:
                                Owner.ScarletHeal(Main.rand.Next(GaiaStriker.BloodBulletHealLootChest - 10, GaiaStriker.BloodBulletHealLootChest + 12), GaiaStrikerBulletDataColor);
                                break;
                            //盖亚重锤的仆从在提前爆裂时提供的治疗
                            case 3f:
                                Owner.ScarletHeal(Main.rand.Next(GaiaStriker.BloodBulletHealMinionDeadEarly - 3, GaiaStriker.BloodBulletHealMinionDeadEarly + 2), GaiaStrikerBulletDataColor);
                                break;
                            //盖亚重锤的仆从在自然爆裂时提供的治疗
                            case 4f:
                                Owner.ScarletHeal(Main.rand.Next(GaiaStriker.BloodBulletHealMinionDead - 3, GaiaStriker.BloodBulletHealMinionDead + 2), GaiaStrikerBulletDataColor);
                                break;
                        }
                        Projectile.Kill();
                    }
                }
            }
        }
        public override bool? CanDamage() => AttackState == State.Homing && Projectile.HJScarlet().HasExecutionMechanic;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 spawnVec = (Projectile.velocity).RotateRandom(0.3f) * Main.rand.NextFloat(0.1f, 1.6f) * 1.5f;
                BloodyMetaball.SpawnParticle(Projectile.Center, spawnVec, Main.rand.NextFloat(0.15f, 0.23f) * 2f, spawnVec.ToRotation(), false);
            }
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.HasProj<GaiaStrikerHeldProj>() && !Owner.HasProj<GaiaStrikerMountedProj>())
                Projectile.AddExecutionTimeImmediate(ItemType<GaiaStriker>());
        }
    }
}
