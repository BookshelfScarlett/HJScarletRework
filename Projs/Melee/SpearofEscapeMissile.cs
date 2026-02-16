using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class SpearofEscapeMissile : HJScarletFriendlyProj
    {
        public override string Texture => ProjPath + nameof(SpearofEscapeMissile);
        public enum Style
        {
            Spawn,
            Attack,
            Direct
        }
        public ref float Timer => ref Projectile.ai[0];
        public bool DontUseMouseHoming = false;
        public NPC HomingTarget = null;
        public int BounceTime = 0;
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16, 2);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 60;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch(AttackType)
            {
                case Style.Spawn:
                    DoSpawn();
                    break;
                case Style.Attack:
                    DoAttack();
                    break;
                case Style.Direct:
                    DoDirect();
                    break;
            }
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            float GeneralScaleMul = 1.1f * RandZeroToOne;
            int GetLifeTime() => Main.rand.Next(8, 16);
            new TurbulenceGlowOrb(Projectile.Center.ToRandCirclePosEdge(4f), 0.5f, RandLerpColor(Color.Orange, Color.OrangeRed), GetLifeTime(), Main.rand.NextFloat(0.1f, 0.12f) * GeneralScaleMul, RandRotTwoPi).Spawn();
            //烟雾除了需要更多，也要更黑。
            for (int i = 0; i < 2; i++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(8f) + Projectile.SafeDirByRot() * i * 10f, -Projectile.velocity / 8f, RandLerpColor(Color.OrangeRed, Color.Black), GetLifeTime(), RandRotTwoPi, 1f, Main.rand.NextFloat(0.12f, 0.16f) * 1.1f * GeneralScaleMul).SpawnToPriorityNonPreMult();
            }
            Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(10f), 0.8f, 1.4f);
            new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f), vel, RandLerpColor(Color.DarkOrange, Color.OrangeRed), GetLifeTime(), RandRotTwoPi, 1f, 0.3f * GeneralScaleMul, ToRadians(10f)).Spawn();
        }

        private void DoDirect()
        {
            Timer++;
            if(Timer > 25f * Projectile.MaxUpdates)
            {
                if (Projectile.GetTargetSafe(out NPC target))
                    Projectile.HomingTarget(target.Center, -1f, 12f, 10f, 20f);
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-ToRadians(5f),ToRadians(6f)));
                Projectile.velocity *= 0.96f;
            }
        }

        private void DoAttack()
        {
            //原本预定的目标不可用，我们才跑追踪方法全局遍历可用单位。
            if (HomingTarget != null && HomingTarget.CanBeChasedBy())
            {
                Projectile.HomingTarget(HomingTarget.Center, -1f, 12f, 10f, 20f);
                //Main.NewText(11);
            }
            else if (GetTargetOnNeed(out NPC target, false))
            {
                Projectile.HomingTarget(target.Center, -1, 12f, 10f, 20f);
            }
            else
            {
                if (Projectile.velocity.LengthSquared() < 8f * 8f)
                    Projectile.velocity *= 1.1f;
            }
        }
        private void DoSpawn()
        {
            //在这个过程中搜索玩家指针最近的那个敌人
            //如果我们搜不到了，我们才让射弹继续自然下落并让射弹自己找
            if (!DontUseMouseHoming)
            {
                if (GetTargetOnNeed(out NPC target, true))
                {
                    HomingTarget = target;
                }
            }
            Timer++;
            if (Timer > 30f * Projectile.MaxUpdates)
            {
                AttackType = Style.Attack;
                Projectile.netUpdate = true;
                Timer *= 0;
            }
            else
            {
               //Spawn的时候默认受到重力影响让火箭落下。
                Projectile.velocity *= 0.97f;
                Projectile.velocity.X *= 1f;
                if (Projectile.velocity.Y < 30f)
                    Projectile.velocity.Y += 0.25f;
            }
        }
        public bool GetTargetOnNeed(out NPC target, bool mouse)
        {
            target = null;
            //事实上这里是跟随鼠标，取一个很小的距离。因为这里本质上是为了做修正而不是为了强锁定
            if (mouse)
            {
                float searchDist = 300f;
                List<NPC> availableTarget = [];
                foreach (NPC needTar in Main.ActiveNPCs)
                {
                    bool legalTarget = needTar != target && needTar.CanBeChasedBy();
                    float distPerTar = Vector2.Distance(needTar.Center, Owner.LocalMouseWorld());
                    if (legalTarget && distPerTar < searchDist)
                    {
                        searchDist = distPerTar;
                        //把可用单位甩进去，因为我们需要最后使用一个最靠近的单位
                        target = needTar;
                    }
                }
                //没有正确搜索敌人直接返回了
                if (target == null)
                {
                    return false;
                }
                //最后返回为真值。
                return true;
            }
            else
            {
                float searchDist = 300f;
                List<NPC> availableTarget = [];
                foreach (NPC needTar in Main.ActiveNPCs)
                {
                    bool legalTarget = needTar != target && needTar.CanBeChasedBy();
                    float distPerTar = Vector2.Distance(needTar.Center, Projectile.Center);
                    if (legalTarget && distPerTar < searchDist)
                    {
                        searchDist = distPerTar;
                        //把可用单位甩进去，因为我们需要最后使用一个最靠近的单位
                        target = needTar;
                    }
                }
                //没有正确搜索敌人直接返回了
                if (target == null)
                {
                    return false;
                }
                return true;
            }
        }

        //复用了兽灾矛的爆炸，但是做出了一些修改
        //事实上，因为橙色本身就已经接近白天的颜色，因此这里是不应该过度使用的
        //这里的处理方式是直接提供命中之后的速度让他们尽快散开
        public override bool PreKill(int timeLeft)
        {
            int dustCount = 10;
            for (int i = 0; i < dustCount; ++i)
            {
                Vector2 dir = Projectile.SafeDirByRot();
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(10f, 10f) + dir * Main.rand.NextFloat(10f);
                new ShinyCrossStar(pos, RandVelTwoPi(2f, 6f), RandLerpColor(Color.Orange, Color.OrangeRed), 45, RandRotTwoPi, RandZeroToOne, Projectile.scale, 0.5f).Spawn();
            }
            for (int i = 0; i < 5; i++)
            {
                Color Firecolor = RandLerpColor(Color.Black, Color.DarkOrange);
                //同样，这里需要提供速度
                new Fire(Projectile.Center + Projectile.SafeDirByRot() * Main.rand.NextFloat(10f), Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 4, Firecolor, 90, Main.rand.NextFloat(TwoPi), 1f, 0.2f).SpawnToPriorityNonPreMult();
            }
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            BounceTime++;
            bool dontKillProj = AttackType == Style.Direct && BounceTime < 2;
            return !dontKillProj;
        }
        public override bool? CanDamage()
        {
            return AttackType == Style.Attack || (AttackType == Style.Direct && Timer > 25f * Projectile.MaxUpdates);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, GetSeconds(5));
            base.OnHitNPC(target, hit, damageDone);
        }
        internal static string TheProjPath = "HJScarletRework/Assets/Texture/Projs";
        public override bool PreDraw(ref Color lightColor)
        {
            float rotFixer = ToRadians(60);
            Texture2D missle = Projectile.GetTexture();
            SB.Draw(missle, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + rotFixer, missle.Size() / 2, Projectile.scale, 0, 0);
            SB.EnterShaderArea();
            if(AttackType != Style.Direct || (AttackType ==Style.Direct && Timer > 35f * Projectile.MaxUpdates))
            DrawTrail();
            SB.EndShaderArea();
            return false;
        }
        private void DrawTrail()
        {
            //虽然我不想承认，但是这里Trail的绘制方法确实是用的starShape
            Texture2D starShape = HJScarletTexture.Specific_RocketTrail.Value;
            Rectangle cutSource = starShape.Bounds;
            //切边。
            cutSource.Height /= 2;
            //重新设定原点
            Vector2 ori = new Vector2(cutSource.Width / 2, cutSource.Height);
            //设定缩放大小
            Vector2 baseScale = new Vector2(5.2f, 4.2f);
            Effect shader = HJScarletShader.VolcanoEruptingShader;
            shader.Parameters["uBaseColor"].SetValue(Color.Orange.ToVector4() * 0.3f);
            shader.Parameters["uTargetColor"].SetValue(Color.OrangeRed.ToVector4() * 0.9f);
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 25);
            shader.Parameters["uIntensity"].SetValue(0.30f);
            shader.Parameters["uNoiseScale"].SetValue(new Vector2(8f, 5f));
            shader.Parameters["UseColor"].SetValue(true);
            shader.Parameters["uFadeRange"].SetValue(0.6f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Noise_Misc2.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;
            //绘制，然后叠图。
            //这里一共会画20次
            //是的没错，然后又有一堆火箭，所以理论来说会有总共超过500多次的绘制
            //希望电脑没事。
            for (float k = 1; k >= 0f; k -= 0.05f)
            {
                Vector2 scale3 = baseScale * new Vector2((1 - k) * .30f, .6f * k);
                float colorRatios = Clamp(k, 0.2f, 1f);
                shader.Parameters["uColorFactor"].SetValue(colorRatios);
                SB.Draw(starShape, Projectile.Center - Main.screenPosition - Projectile.rotation.ToRotationVector2() * 15f, cutSource, Color.OrangeRed * colorRatios, Projectile.rotation - PiOver2, ori, scale3, 0, 0);
            }
        }
    }
}
