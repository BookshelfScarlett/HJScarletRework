using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Misc;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class TheSevenStarHeldProj: ExecutorHeldProj, IPixelatedRenderer
    {
        public override int OriginalItemID => ItemType<TheSevenStar>();
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override string Texture => GetInstance<TheSevenStar>().Texture;
        public AnimationStruct Helper = new AnimationStruct(3);
        public float SwordLength = 60;
        public float BeginTargetRotation = 0;
        public float TargetRotation = 0;
        public bool Flip = false;
        public float Height = 1f;
        public float SwordScale = 1f;
        public List<Vector2> OldAimPos = [];

        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.SetUpHeldProj(10);
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;

        }
        public override void OnFirstFrame()
        {
            ScarletSound(HJScarletSounds.TheSevenStar_Swing, Projectile.Center, 0.5f, 1, 0.4f, 0.1f);
            Projectile.originalDamage = Projectile.damage;
            Helper.MaxProgress[0] = (int)(AttackSpeed * .45f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .55f);
            Helper.MaxProgress[2] = (int)(AttackSpeed * 1.20f);
            BeginTargetRotation = Owner.Center.ToMouseVector2().ToRotation();
            TargetRotation = BeginTargetRotation;

        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
            UpdatePlayerState();
            HandleExecution();
            if (OldAimPos.Count > 25)
                OldAimPos.RemoveAt(0);
        }
        public void UpdatePlayerState()
        {
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ControlPlayerArm(Projectile.rotation);

        }

        public void UpdateHeldState()
        {
            Projectile.Center = Owner.MountedCenter;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 2;
        }
        public void UpdateAnimation()
        {
            if (!Helper.IsDone[0])
            {
                if(Helper.OnAnimationBegin(0))
                {
                    if(Projectile.IsMe())
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.Center.GetNormalVector2(Main.MouseWorld) * 16f, ProjectileType<TheSevenStarStar>(), Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
                        proj.HJScarlet().HasExecutionMechanic = true;
                    }
                }
                UpdateBeginAnimation();

            }
            else if (!Helper.IsDone[1])
            {
                if (OldAimPos.Count > 0)
                    OldAimPos.RemoveAt(0);

                UpdateEndAnimation();
            }
            else if (!Helper.IsDone[2] && !Main.mouseLeft)
            {
                if (Main.mouseLeft)
                {
                    Projectile.Kill();
                }
                UpdateFinalAnimation();
            }
            else
                Projectile.Kill();
        }

        public void UpdateFinalAnimation()
        {
            Helper.UpdateAniState(2);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseInOutSin(Helper.GetAniProgress(2));
            float beginAngle = 180f * Flip.ToDirectionInt();
            float endAngle = 185 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);

        }

        public void UpdateBeginAnimation()
        {
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1;
            Helper.UpdateAniState(0);
            float easedProgress = EaseInOutExpo(Helper.GetAniProgress(0));
            float beginAngle = -185f * Flip.ToDirectionInt();
            float endAngle = 175f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .5f);
            else
            {
                //下面基本上是粒子生成了。
                float slashTrailRotation = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
                Matrix tFormSlash = Matrix.CreateRotationZ(slashTrailRotation) * Matrix.CreateScale(1f, Height, 1f);
                Vector2 slashTargetPos = Vector2.Transform(Vector2.UnitX, tFormSlash) * SwordScale * heldScale;
                Vector2 slashPosFinal = slashTargetPos.RotatedBy(TargetRotation) * 80;
                OldAimPos.Add(slashPosFinal);
                if (Main.rand.NextBool(8))
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 85, Main.rand.NextFloat(0.51f, 1.08f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy((PiOver2 + ToRadians(10)) * Owner.direction * (Flip.ToDirectionInt())) * Main.rand.NextFloat(1.2f, 1.5f)*2f;
                    ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.LightSkyBlue, Color.DarkGray), 40, 1f, .04f * Projectile.scale * Main.rand.NextFloat(.8f, 1.1f), glowMult: .51f);
                }
                if (Main.rand.NextBool(6))

                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 85, Main.rand.NextFloat(.51f, .98f));
                    Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                    Vector2 vel = Owner.velocity * 0.5f + dir.RotatedBy(PiOver2 * Owner.direction * Flip.ToDirectionInt()) * Main.rand.NextFloat(1.5f, 1.9f)*2f;
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.LightSkyBlue, Color.DarkGray), 20, 1f, Main.rand.NextFloat(.7f, 1.01f) * Projectile.scale * .55f, 0.2f);
                }
            }
        }
        public void UpdateEndAnimation()
        {
            Helper.UpdateAniState(1);
            float heldScale = HJScarletMethods.HasFuckingCalamity ? Owner.HeldItem.scale : 1f;
            float easedProgress = EaseInOutExpo(Helper.GetAniProgress(1));
            float beginAngle = 175f * Flip.ToDirectionInt();
            float endAngle = 180 * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Matrix tForm = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(1f, Height, 1);
            Vector2 tarPos = Vector2.Transform(Vector2.UnitX, tForm) * SwordScale * heldScale;
            Projectile.scale = tarPos.Length();
            Projectile.rotation = tarPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Owner.GetToMouseVector2(Projectile.Center).ToRotation(), .05f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 1.72f;
            if (!Projectile.IsMe())
                return;
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                proj.HJScarlet().HasExecutionMechanic = true;
                ((TheSevenStarHeldProj)proj.ModProjectile).Flip = !Flip;
                ((TheSevenStarHeldProj)proj.ModProjectile).BeginTargetRotation = TargetRotation;
            }
            else
            {
                ScarletSound(HJScarletSounds.Misc_KnifeExpired, Owner.Center, 0.955f, 1, -0.4f, 0.2f);
                for (int i = 0; i < 26; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f, Main.rand.NextFloat(.1f, .98f));
                    ECSParticle.TurbulenceShinyOrb(pos, 1.2f, Color.White, 60, 1, 0.12f);
                }
                for (int i = 0; i < 26; i++)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f, Main.rand.NextFloat(.1f, .98f));
                    ECSParticle.ShinyCrossStarECS(pos, RandVelTwoPi(0.7f, 1.2f), Color.White, 60, 1, 0.5f);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Projectile.AddExecutionTimeImmediate(OriginalItemID);
            ScarletSound(HJScarletSounds.TheSevenStar_Hit, target.Center, .75f);
            for (int i = 0; i < 34; i++)
            {
                ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1.2f, 4.2f), Color.RoyalBlue, 40, 1, 0.6f);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            float easedProgress = EaseOutCubic(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 98;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public BlendState BlendState => BlendState.Additive;

        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 topPos = drawPosition + (Vector2.UnitX).RotatedBy(Projectile.rotation) * 23f * Projectile.scale;
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            float scale = Projectile.scale * .32f * (1 - EaseOutExpo(Helper.GetAniProgress(1)));
            SB.Draw(glow, topPos, null, Color.LightGoldenrodYellow, PiOver4, glow.Size() / 2, scale, 0, 0);
            Texture2D texture = HJScarletTexture.Texture_StandardGradient.Value;
            HJScarletMethods.ApplyAlphaCut(new Vector4(0.31f, 0.1f, 0, 0), Vector2.Zero, Vector2.One);
            DrawSlash(texture, Color.SkyBlue* 0.90f, 0.95f);
            DrawSlash(texture, Color.LightSkyBlue* 0.60f, 0.55f);

            HJScarletMethods.ApplyAlphaCut(new Vector4(0.42f, 0.2f, 0, 0), new Vector2(-Main.GlobalTimeWrappedHourly * 1.195f, 0), new Vector2(3.2f,2.1f), Color.SkyBlue);
            Texture2D texture2 = HJScarletTexture.Noise_Misc.Value;
            DrawSlash(texture2, Color.SkyBlue* .95f, 0.90f);
            texture2 = HJScarletTexture.Noise_Aura.Value;
            DrawSlash(texture2, Color.LightSkyBlue* .80f, 0.55f);

            texture = HJScarletTexture.Texture_SwordSlash.Value;
            HJScarletMethods.ApplyAlphaCut(new Vector4(0.41f, 0.1f, 0, 0), Vector2.Zero, Vector2.One);
            DrawSlash(texture, Color.SkyBlue* 0.95f, 0.95f);
            DrawSlash(texture, Color.LightSkyBlue* 0.60f, 0.50f);
            HJScarletMethods.EndShaderAreaPixel();
        }
        private List<ScarletVertex> _vertexCache = new List<ScarletVertex>(); // 类级别缓存
        public void DrawSlash(Texture2D texture, Color drawcolor, float mult = 0.8f)
        {
            if (OldAimPos.Count < 3)
                return;
            _vertexCache.Clear();
            List<ScarletVertex> Vertexlist = new List<ScarletVertex>();
            for (int i = 0; i < OldAimPos.Count; i++)
            {
                float progress = (float)i / OldAimPos.Count;
                Vector2 DrawPos_Head = OldAimPos[i] + Projectile.Center - Main.screenPosition;
                Vector2 DrawPos_Source = OldAimPos[i] * mult + Projectile.Center - Main.screenPosition;
                _vertexCache.Add(new ScarletVertex(DrawPos_Head, drawcolor, new Vector3(progress, 0, 0)));
                _vertexCache.Add(new ScarletVertex(DrawPos_Source, drawcolor, new Vector3(progress, 1, 0)));
            }
            GD.Textures[0] = texture;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertexCache.ToArray(), 0, _vertexCache.Count - 2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawInfo_Melee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            PixelatedRenderManager.BeginDrawProj = true;
            float progress = (1 - Helper.GetAniProgress(1));
            float easePro = Helper.GetAniProgress(2);
            Color c = Color.Lerp(Color.White, Color.White with { A = 30 }, easePro);
            SB.Draw(tex, drawPosition, null, c, drawRotation, rotationPoint, Projectile.scale, flipSprite, 0);
            SB.EnterShaderArea(BlendState.NonPremultiplied);
            Texture2D texture = HJScarletTexture.Texture_SwordSlashWhite.Value;
            HJScarletMethods.ApplyAlphaCut(new Vector4(0.41f, 0.53f, 0.12f, 0.12f), Vector2.One, Vector2.One);
            DrawSlash(texture, Color.Black * 0.136f, 0.45f);
            DrawSlash(texture, Color.DeepSkyBlue* 0.150f, 0.20f);
            SB.EndShaderArea();
            return false;
        }
    }
}
