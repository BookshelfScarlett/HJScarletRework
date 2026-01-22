using ContinentOfJourney.Projectiles;
using ContinentOfJourney.Tiles.Theatre;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FlybackHandThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<FlybackHand>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(10,2);
        public ref float AttackTimer => ref Projectile.ai[1];
        public bool Ishit = false;
        public override void ExSD()
        {
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.timeLeft = 2;
            DrawDust();
            if (Vector2.Distance(Projectile.Center, Owner.MountedCenter) > 1800f)
                Projectile.Kill();
            //命中后减速并消失
            if (Ishit)
            {
                Projectile.Opacity -= 0.01f;
                Projectile.velocity *= 0.93f;
                if (Projectile.velocity.Length() < 0.1f && Projectile.Opacity <=0)
                    Projectile.Kill();
            }
        }

        private void DrawDust()
        {
            if (!Ishit)
            {
                for (int j = 0; j < 2; j++)
                    new TurbulenceShinyOrb(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), 0.5f, RandLerpColor(Color.Gold, Color.PaleGoldenrod), 40, 0.1f * Projectile.Opacity, Projectile.rotation).SpawnToNonPreMult();
            }
            else
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 mountedPos = Projectile.Center + dir * 60f;
                    new TurbulenceShinyOrb(mountedPos - dir * i + Main.rand.NextVector2Circular(10f, 10f), 0.5f, RandLerpColor(Color.Gold, Color.PaleGoldenrod), (int)((float)40 * Projectile.Opacity), 0.1f * Projectile.Opacity, Projectile.rotation).SpawnToNonPreMult();
                }
            }
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Ishit = true;
            ClockDust(target);
        }
        public void ClockDust(NPC target)
        {
            //获取当前泰拉时间，即Main.Time
            double time = Main.time;

            //假定当前是晚上，则将时间进行校准，出于某些原因，夜晚的时间必须得直接增加54000来表示
            if (!Main.dayTime)
                time += 54000D;

            //内部时间单位以“刻”表示，而60刻=1秒，因此这里3600刻为1小时，我们将当前的量拆分
            time /= 3600D;

            //泰拉瑞亚的夜晚起始于晚上7点半，因此需要用这个值去指针
            time -= 19.5;

            //假如过度校准，直接自增24小时，让时钟转移一圈避免负值
            if (time < 0D)
                time += 24D;

            //而后，我们开始常识性地获取分钟
            int intTime = (int)time;
            double deltaTime = time - intTime;

            //将当前的时间转化为一个实际的分钟值
            deltaTime = (int)(deltaTime * 60D);

            //然后，将24个整数小时转为12小时
            if (intTime > 12)
                intTime -= 12;

            float hour = (float)intTime;
            float hourAngle = PiOver2 - (TwoPi / 12f) * hour;
            //校准一下，草
            hourAngle *= -1f;
            Vector2 hourVector = hourAngle.ToRotationVector2();

            float minute = (float)deltaTime;
            float minuteAngle = PiOver2 - (TwoPi / 60f) * minute;
            minuteAngle *= -1f;
            Vector2 minuteVector = minuteAngle.ToRotationVector2();
            //最后。开始生成需要的星辰
            Projectile star1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, hourVector * 12f, ProjectileType<FlybackHandStar>(), Projectile.damage, 10f, Owner.whoAmI);
            star1.extraUpdates = 1;
            star1.HJScarlet().GlobalTargetIndex = target.whoAmI;
            
            Projectile star2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, minuteVector * 12f, ProjectileType<FlybackHandStar>(), Projectile.damage, 10f, Owner.whoAmI);
            star2.extraUpdates = 1;
            star2.HJScarlet().GlobalTargetIndex = target.whoAmI;
            for (float i = 0; i < 10f; i++)
            {
                Vector2 dir = Projectile.SafeDirByRot().RotatedBy(Main.rand.NextFloat(ToRadians(20)) * Main.rand.NextBool().ToDirectionInt());
                for (float k = 0; k < 10f; k++)
                {
                    Vector2 vel = (Main.rand.NextBool() ? minuteVector : hourVector) * Main.rand.NextFloat(1.2f, 2.1f);
                    Vector2 spawnPos = target.Center + Main.rand.NextVector2Circular(16, 16);
                    //第一步：金色粒子，用的橙色火把
                    Dust d = Dust.NewDustPerfect(spawnPos, DustID.IchorTorch, vel * k / 3f, 100);
                    d.noGravity = true;
                    d.scale *= Main.rand.NextFloat(1.2f, 1.4f);
                }
            }
            //最后，用粒子组成一个需要的还
            float counts = 150;
            for (float i = 0; i < counts; i++)
            {
                float val = (1 - i / counts);
                Color lerpColor = Color.Lerp(Color.LightGoldenrodYellow, Color.LightYellow, Clamp(val, 0, 1)) with { A = 50 } * 0.9f;
                Vector2 pos = Vector2.UnitY.RotatedBy(ToRadians(360 / counts * i));
                new HRShinyOrbMedium(target.Center + pos * 150f, RandLerpColor(Color.Gold, Color.White), 60, 0.15f, 0).Spawn();
            }
            //准备依据时间的不同给予增益
            Owner.HJScarlet().FlybackHitBuffTimer = 180;
        }
        public override bool? CanDamage() => Ishit == false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            //绘制射弹
            Projectile.DrawGlowEdge(Color.Gold * Projectile.Opacity, rotFix: ToRadians(135));
            Projectile.DrawProj(Color.White * Projectile.Opacity, 6, 0.1f, rotFix: ToRadians(135));
            //绘制双方描边
            DrawShaderParaLine();
            //为内部填色
            FillColor(star);
            
            //绘制矛尖高光
            DrawTopGlowStar(star);
            return false;
        }
        private void DrawShaderParaLine()
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            float widthOffset = -80f;
            DrawTrails(Color.Gold, HJScarletTexture.Trail_ManaStreak.Texture, 13f, 12f, widthOffset, 0.7f * Projectile.Opacity);
            DrawTrails(Color.LightYellow, HJScarletTexture.Trail_ManaStreak.Texture, 7f, 12f, widthOffset, 1f * Projectile.Opacity);
            DrawTrails(Color.Gold, HJScarletTexture.Trail_ManaStreak.Texture, 7f, -12f, widthOffset, 0.7f * Projectile.Opacity);
            DrawTrails(Color.LightYellow, HJScarletTexture.Trail_ManaStreak.Texture, 7f, -12f, widthOffset, 1f * Projectile.Opacity );
            SB.End();
            SB.BeginDefault();

        }
        //绘制最顶头的星辰
        private void DrawTopGlowStar(Texture2D star)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition + Projectile.SafeDir() * 80f;
            for (int j = 1; j <= 3; j++)
            {
                Color drawColor = Color.Lerp(Color.Gold, Color.PaleGoldenrod, (float)j / 8);
                Vector2 starPos = drawPos + Projectile.SafeDir() * j * 0.5f;
                SB.Draw(star, starPos, null, drawColor with { A = 125 }, Projectile.rotation + PiOver4, star.Size() / 2, new Vector2(1.5f, 0.2f) * Projectile.Opacity, 0, 0);
                SB.Draw(star, starPos, null, drawColor with { A = 125 }, Projectile.rotation + ToRadians(135), star.Size() / 2, new Vector2(1.5f, 0.2f) * Projectile.Opacity, 0, 0);
            }
            SB.Draw(HJScarletTexture.Particle_ShinyOrb.Value, drawPos, null, Color.White with { A = 0 }, 0f, HJScarletTexture.Particle_ShinyOrb.Origin, Projectile.scale * 1f * Projectile.Opacity, 0, 0);
        }
        //这里的实现原理是用一个切边棱形不断叠层
        private void FillColor(Texture2D star)
        {
            Rectangle cutSource = star.Bounds;
            //切边。
            cutSource.Height /= 2;
            //重新设定原点
            Vector2 ori = new Vector2(cutSource.Width / 2, cutSource.Height);
            int length = 45;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.DarkGoldenrod, Color.DarkKhaki, rads) with { A = 150 }) * 0.4f * Projectile.Opacity * (1 - rads);
                Vector2 trailPos = Projectile.Center - Main.screenPosition - Projectile.SafeDir() * 2.5f * i + Projectile.SafeDir() * 50f;
                SB.Draw(star, trailPos, null, drawColor * Projectile.Opacity, Projectile.rotation , ori, Projectile.scale * new Vector2(1.0f, 0.6f), 0, 0);
            }

        }
        public void DrawTrails(Color trailColor, Asset<Texture2D> useTex, float height, float offset = 1f, float rotationVectorOffset = 1f, float alphaValue = 1f, int laserLength = 50)
        {
            HJScarletShader.TerrarRayLaser.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            HJScarletShader.TerrarRayLaser.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            HJScarletShader.TerrarRayLaser.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -3.2f);
            HJScarletShader.TerrarRayLaser.Parameters["uColor"].SetValue(trailColor.ToVector4() * alphaValue);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeoutLength"].SetValue(1f);
            HJScarletShader.TerrarRayLaser.Parameters["uFadeinLength"].SetValue(0.1f);
            HJScarletShader.TerrarRayLaser.CurrentTechnique.Passes[0].Apply();
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            GD.Textures[0] = useTex.Value;
            GD.SamplerStates[0] = SamplerState.PointClamp;
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                Vector2 Position = validPosition[j];
                Vector2 NextPosition = validPosition[j + 1];
                float rot = (NextPosition - Position).ToRotation();
                float ratio = (float)j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offset + rot.ToRotationVector2() * rotationVectorOffset;
                trailDrawDates.Add(new(Position + Projectile.Size / 2 + posOffset, trailColor, new Vector2(0, height), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
