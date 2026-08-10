using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class BambooSwordSpin : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<BambooSwordHeldProj>().Texture;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(30);
        }
        public float TargetRotation = 0;
        public AnimationStruct Helper = new AnimationStruct(1);
        public ref float Timer => ref Projectile.ai[0];
        public enum State
        {
            Shoot,
            Spin
        }
        public bool SpinDone = false;
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public float OriginalSpeed = 0;
        public float ArmRotation = 0;
        public float RandRotation = 0;
        public override void ProjAI()
        {
            if (AttackState == State.Shoot)
            {
                if (Timer == 0)
                {
                    Helper.MaxProgress[0] = Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, 50 * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
                    OriginalSpeed = Projectile.velocity.Length();
                    RandRotation = RandRotTwoPi;
                    TargetRotation = Owner.Center.GetNormalVector2(Main.MouseWorld).ToRotation();
                    ArmRotation = TargetRotation;
                    ScarletSound(HJScarletSounds.Atom_StrikeAlt, Projectile.Center, pitch: -.264f, pitchVariance: .1f);
                    for (int i = 0; i < 16; i++)
                    {
                        ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(4f), Projectile.SafeDir().ToRandVelocity(ToRadians(5f), .3f, 34f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, RandRotTwoPi, 1, 0.25f, Main.rand.NextBool(), BlendState.Additive);
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), Projectile.SafeDir().ToRandVelocity(ToRadians(5f), 0.3f, 20.1f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, 1, 0.26f * Main.rand.NextFloat(.9f, 1.1f));
                    }


                }
                Projectile.rotation = Projectile.velocity.ToRotation();
                float maxtime = 24 * Projectile.MaxUpdates;
                float ratios = Utils.GetLerpValue(0, maxtime, Timer, true);
                Timer++;
                Projectile.velocity = Vector2.Lerp(Projectile.SafeDir() * OriginalSpeed, Projectile.SafeDir() * .2f, EaseOutCubic(ratios));
                if (Main.rand.NextBool(5))
                    ECSParticle.LiliesPetal(Projectile.Center.ToRandCirclePosEdge(3), Projectile.SafeDir(), RandLerpColor(Color.LimeGreen, Color.ForestGreen), 60, 1, RandRotTwoPi, 0.1f * Main.rand.NextFloat(.8f, 1.01f), 0.6f, false, fullBright: true, blendState: BlendState.AlphaBlend);
                if (Main.rand.NextBool(3))
                    ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(6), Projectile.SafeDir(), RandLerpColor(Color.LimeGreen, Color.ForestGreen), 40, 1, 0.34f);

                //Projectile.velocity *= .94f;
                if (Timer >= Projectile.MaxUpdates * 24f || Projectile.numHits > 0)
                {
                    Timer = 0;
                    AttackState = State.Spin;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (Projectile.velocity.LengthSquared() > 0.05f * .05f)
                {
                    Projectile.velocity *= .9f;
                }
                Vector2 rnd = RandDirTwoPi * Projectile.scale;

                if (!SpinDone)
                {

                    float maxtime = 30 * Projectile.MaxUpdates;
                    float ratios = Utils.GetLerpValue(0, maxtime, Timer, true);
                    Timer++;
                    Projectile.rotation += Lerp(ToRadians(0f), ToRadians(20f), ratios) * Projectile.direction;
                    Vector2 vRnd = -rnd * .5f + rnd.RotatedBy(PiOver2) * 2.5f * ratios;
                    if (Main.rand.NextBool(7))
                        ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(.3f, 9f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
                    Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(20).RotatedBy(Projectile.rotation), DustID.JungleTorch);
                    d.velocity = Projectile.rotation.ToRotationVector2();
                    d.noGravity = true;
                    if (Timer > maxtime + Projectile.MaxUpdates * 50f)
                    {
                        SpinDone = true;
                        Timer = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < 26; i++)
                    {
                        ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(.3f, 14f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(0.3f, 10.1f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.JungleTorch);
                        d.velocity = RandVelTwoPi(1.2f, 6.2f);
                        d.noGravity = true;
                        d.scale = Main.rand.NextFloat(1.2f, 1.61f);
                    }
                    ScarletSound(SoundID.Item101, Projectile.Center);

                    Projectile.Kill();
                }
            }

            if (Helper.IsDone[0])
                return;
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseOutExpo(Helper.GetAniProgress(0));
            float beginAngle = 195f;
            float endAngle = -195f;
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Projectile.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1, 1, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * 1.1f * heldScale;
            ArmRotation = tarPos.ToRotation() + TargetRotation;
            Owner.ControlPlayerArm(ArmRotation);
            Owner.itemAnimation = Owner.itemTime = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Shoot)
            {
                Timer = Projectile.MaxUpdates * 30f;
            }
            Projectile.velocity *= .90f;
            Vector2 finalDir = Owner.Center.GetNormalVector2(target.Center);
            ScreenShakeSystem.AddScreenShakes(target.Center, 4, 18, Owner.Center.GetNormalVector2(target.Center).ToRotation(), 0, easingFunc: EaseOutBack);
            if (Projectile.numHits < 1)
                ScarletSound(HJScarletSounds.Tlipoca_StoneBonk, target.Center, pitch: -.34f, pitchVariance: .1f, variantType: 1);
            else
                ScarletSound(HJScarletSounds.Tlipoca_StoneBonk, target.Center, pitch: -.64f, pitchVariance: .1f, variantType: 1);
            for (int i = 0; i < 26; i++)
            {
                Vector2 vel = finalDir.ToRandVelocity(ToRadians(35f), .1f, 19f);
                ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * 10f, RandVelTwoPi(.3f, 14f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, RandRotTwoPi, 1, 0.45f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 20; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center.ToRandCirclePos(6) + finalDir * 4f, RandVelTwoPi(0.3f, 10.1f), RandLerpColor(Color.ForestGreen, Color.LawnGreen), 40, 1, 0.46f * Main.rand.NextFloat(.9f, 1.1f));
                Dust d = Dust.NewDustPerfect(target.Center, DustID.JungleTorch);
                d.velocity = RandVelTwoPi(1.2f, 6.2f) + finalDir * 3f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.2f, 1.61f);
            }
            if (target.type == NPCID.DungeonGuardian)
            {
                target.HJScarlet().PostSpeed = target.velocity.ToSafeNormalize() * -8f;
                target.HJScarlet().StopNpcTime = 31;
            }
            else
            {
                target.HJScarlet().PostSpeed = target.velocity.ToSafeNormalize() * -1.8f;
                target.HJScarlet().StopNpcTime = 10;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            int length = Projectile.oldPos.Length - 2;
            rotationPoint = tex.Size() / 2f;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1 - i / (float)length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i] + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
                float opac = Lerp(0.05f, 1f, ratios) * .30f;
                Color c = Color.Lerp(Color.LimeGreen, Color.White, ratios).ToAddColor(75);
                SB.FastDraw(tex, pos, c * opac, rot, rotationPoint, Projectile.scale * 1.32f, flipSprite);
            }
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale * 1.32f, flipSprite);

            return false;
        }
    }
}
