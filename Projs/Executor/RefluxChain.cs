using ContinentOfJourney.Items.FielderSentries;
using ContinentOfJourney.Items.ThrowerWeapons;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.DeepGlowSystem;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class RefluxChain : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public ref NPC MountedTarget => ref Projectile.HJScarlet().CurStoredTarget;
        public NPC ChainTarget = null;
        public bool AnyActiveTarget => MountedTarget.IsLegal() && ChainTarget.IsLegal();
        public ref float ChainLengthRatios => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Projectile.MaxUpdates = 2;
            Projectile.SetupImmnuity(30 * Projectile.MaxUpdates, ImmnuityType.Static);
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 4;
            Projectile.timeLeft = GetSeconds(10) ^ Projectile.MaxUpdates;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!AnyActiveTarget)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = MountedTarget.Center;
            Vector2 beamEndPos = ChainTarget.Center;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 4f, ref _);
            //HJScarletMethods.LineThroughRect(MountedTarget.Center, ChainTarget.Center, targetHitbox,64);
            return c;
        }
        public override void OnFirstFrame()
        {
            if (!MountedTarget.IsLegal())
            {
                Projectile.Kill();
                return;
            }
            float searchDistance = 600f * 600f;
            NPC curTar = null;
            foreach (var tar in Main.ActiveNPCs)
            {
                bool legalTar = !tar.Equals(MountedTarget) && tar.CanBeChasedBy();
                float distPerTar = Vector2.DistanceSquared(tar.Center, Projectile.Center);
                if (legalTar && distPerTar < searchDistance)
                {
                    searchDistance = distPerTar;
                    curTar = tar;
                }
            }
            if (curTar.IsLegal())
            {
                ChainTarget = curTar;
            }
            //第一帧搜索附近的可用单位
            //搜索完毕后如果ChainTarget为null，立即处死
            if (!ChainTarget.IsLegal())
            {
                Projectile.Kill();
            }
        }
        public override void ProjAI()
        {
            //目标单位与敌对单位若不存在，不要做任何事情，直接自杀
            if (!AnyActiveTarget)
            {
                Projectile.Kill();
                return;
            }
            if (MountedTarget.IsLegal())
                Projectile.Center = MountedTarget.Center;
            int fadeTime = 30 * Projectile.MaxUpdates;
            if (Projectile.timeLeft > fadeTime)
            {
                ChainLengthRatios = Lerp(ChainLengthRatios, 1f, .12f);
                if (ChainLengthRatios > .98f)
                    ChainLengthRatios = 1;
            }
            else
            {
                Projectile.damage = 0;
                ChainLengthRatios = Lerp(ChainLengthRatios, 0f, 0.09f);
            }
            MountedTarget.HJScarlet().refluxChain = true;
            ChainTarget.HJScarlet().refluxChain = true;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno,GetSeconds(2));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            if (!AnyActiveTarget)
                return false;
            //开始绘制链条
            Texture2D chains = Projectile.GetTexture();
            SB.EnterShaderArea(BlendState.Additive);
            DrawTheLine(Projectile.Center, ChainTarget.Center, Color.LimeGreen, 1);
            SB.EndShaderArea();

            Vector2 pCenter = ChainTarget.Center;
            Vector2 projCenter = Projectile.Center;
            Vector2 directionToPlayer = pCenter - projCenter;
            float chainRot = directionToPlayer.ToRotation() - PiOver2;
            float distanceToPlayer = directionToPlayer.Length();
            while (distanceToPlayer > 30f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer;
                directionToPlayer *= chains.Height;
                projCenter += directionToPlayer;
                directionToPlayer = pCenter - projCenter;
                distanceToPlayer = directionToPlayer.Length();
                Color c = Color.White * ChainLengthRatios;
                Vector2 pos = projCenter - Main.screenPosition;
                SB.Draw(chains, pos + Main.rand.NextVector2Circular(5, 5), chains.Bounds, Color.LimeGreen.ToAddColor()*ChainLengthRatios, chainRot, chains.Size() / 2f, 1 * new Vector2(ChainLengthRatios, 1), 0, 0);
                SB.Draw(chains, pos + Main.rand.NextVector2Circular(1, 1), chains.Bounds, c.ToAddColor(175), chainRot, chains.Size() / 2f, 1 * new Vector2(ChainLengthRatios, 1), 0, 0);
            }
            //Texture2D orb = HJScarletTexture.Particle_HRShinyOrb.Value;
            SB.EnterShaderArea();
            //SB.FastDraw(orb, orbPos, Color.Green, 0, orb.Size() / 2f, Projectile.scale * 0.5f, 0);
            SB.EndShaderArea();
            return false;
        }
        public void DrawTheLine(Vector2 beginPos, Vector2 targetPos, Color c, float thick)
        {
            targetPos -= Main.screenPosition;
            beginPos -= Main.screenPosition;
            c *= .75f;
            Asset<Texture2D> tex = HJScarletTexture.Trail_BloomDualLine.Texture;
            Vector2 vec = beginPos.GetNormalVector2(targetPos);
            float length = Vector2.Distance(beginPos, targetPos);
            Vector2 orig = new Vector2(0, tex.Height() / 2f);
            float xScale = length / tex.Width();
            float rotation = vec.ToRotation();
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(length, tex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -0);
            shader.Parameters["uColor"].SetValue(c.ToVector4());
            shader.Parameters["uFadeoutLength"].SetValue(0.21f);
            shader.Parameters["uFadeinLength"].SetValue(0.21f);
            shader.CurrentTechnique.Passes[0].Apply();
            SB.Draw(tex.Value, beginPos+Main.rand.NextVector2Circular(5,5), null, c, rotation, orig, new Vector2(xScale*ChainLengthRatios, .15f * thick*ChainLengthRatios), 0, 0);
        }
    }
}
