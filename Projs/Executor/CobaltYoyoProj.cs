using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class CobaltYoyoProj :HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public const int MaxUpdates = 2;
        public bool IsCloneYoyo = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = GetSeconds(10);
            ProjectileID.Sets.YoyosMaximumRange[Type] = 600f;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 16f / MaxUpdates;

            Projectile.ToTrailSetting(8, 0);
        }
        public override void ExSD()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.SetupImmnuity(15 * Projectile.MaxUpdates);
            Projectile.penetrate = -1;
            Projectile.DamageType = ExecutorDamageClass.Instance;
            Projectile.friendly = true;
        }
        public override void ProjAI()
        {
            //判断一下这个悠悠球是否为由悠悠球袋克隆出来的第二悠悠球
            if (!IsCloneYoyo)
            {
                CheckCloneYoyo();
                NotCloneYoyoBehaviour();
            }
        }

        public void NotCloneYoyoBehaviour()
        {
            if(!Projectile.HJScarlet().ExecutionStrike &&Owner.GetExecutionSrike())
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                Owner.RemoveExecutionProgress();
            }
            float rotFixer = Projectile.direction > 0 ? ToRadians(30) : -ToRadians(30);
            Owner.ControlPlayerArm(Owner.Center.GetNormalVector2(Projectile.Center).ToRotation() + rotFixer, 1);
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(4), Vector2.UnitY * Main.rand.NextFloat(0.5f, 1f) * 3, RandLerpColor(Color.AliceBlue, Color.LightSkyBlue), 40, Projectile.Opacity, Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .4f, .2f);
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePosEdge(4), Vector2.UnitY * Main.rand.NextFloat(.5f, 1f) * 3, RandLerpColor(Color.AliceBlue, Color.LightSkyBlue), 40, Projectile.Opacity, Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .23f, 0);
        }

        public void CheckCloneYoyo()
        {
            int mainYoyo = -1;
            foreach (var proj in Main.ActiveProjectiles)
            {
                //遍历是否为主悠悠球。如果是，跳出去
                //由于泰拉瑞亚射弹列表的特性与悠悠球手套的特性，遍历时遇到的第一个悠悠球总会是主悠悠球。不需要额外判定
                if (proj.IsLegalFriendlyProj() && proj.type == Type && proj.owner == Owner.whoAmI)
                {
                    mainYoyo = proj.whoAmI;
                    break;
                }
            }
            //检查这个悠悠球是否为当前的悠悠球的索引
            if (Projectile.whoAmI != mainYoyo)
                IsCloneYoyo = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsCloneYoyo)
            {
                if (!Owner.HasProj<CobaltYoyoSlash>())

                    Projectile.AddExecutionTimeImmediate<CobaltYoyo>();
                Projectile proj1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<InvisBoom>(), Projectile.damage / 4, Projectile.knockBack, Owner.whoAmI);
                for (int i = 0; i < 15; i++)
                {
                    ECSParticle.SmokeParticle(target.Center, RandVelTwoPi(1f, 12f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 25, RandRotTwoPi, .75f, Projectile.scale * Main.rand.NextFloat(.9f, 1.15f) * .24f, Main.rand.NextBool(), BlendState.Additive);
                }
                for (int i = 0; i < 15; i++)
                {
                    ECSParticle.ShinyCrossStarECS(target.Center, RandVelTwoPi(1f, 4.5f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 25, 1, Projectile.scale * Main.rand.NextFloat(.90f, 1.05f) * .75f, .2f);
                }
                ScarletSound(SoundID.DD2_GoblinBomb, Projectile.Center, volume: .75f, pitch: .6f);
                if (target.IsLegal() && Projectile.HJScarlet().ExecutionStrike)
                {
                    ScarletSound(HJScarletSounds.GrabCharge, Projectile.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<CobaltYoyoSlash>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    proj.HJScarlet().CurStoredTarget = target;
                    Projectile.HJScarlet().ExecutionStrike = false;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int drawLength = Projectile.oldPos.Length;
            if (!IsCloneYoyo)
            {
                SB.EnterShaderArea();
                Texture2D glowTex = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
                SB.FastDraw(glowTex, drawPos, Color.SkyBlue, 0, glowTex.Size() / 2f, Projectile.scale * .25f, 0);
                SB.FastDraw(glowTex, drawPos, Color.White, 0, glowTex.Size() / 2f, Projectile.scale * .20f, 0);
                SB.EndShaderArea();
                SB.EndShaderArea();
                for (int i = drawLength - 1; i >= 0; i--)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                        continue;
                    Vector2 trailingDrawPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.10f) + Projectile.PosToCenter();
                    float faded = 1 - i / (float)drawLength;
                    //平方放缩
                    faded = MathF.Pow(faded, 3);
                    Color trailColor = Color.Lerp(Color.LightSkyBlue, Color.Lerp(Color.RoyalBlue, Color.White, 0.18f), faded) * 0.9f;
                    float opa = Lerp(0.85f, 1f, faded);
                    trailColor = trailColor.ToAddColor((byte)(Lerp(0, 0, faded))) * opa;
                    float scaleMult = Lerp(0.50f, 1f, faded);
                    SB.FastDraw(projTex, trailingDrawPos, trailColor, Projectile.oldRot[i], ori, Projectile.scale * scaleMult, 0);
                }
            }
            SB.FastDraw(projTex, drawPos, Color.White, Projectile.rotation, ori, Projectile.scale, 0);
            
            return false;
        }
    }
}
