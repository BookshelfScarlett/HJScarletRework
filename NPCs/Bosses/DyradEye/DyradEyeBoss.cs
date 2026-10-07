using ContinentOfJourney.Items.Donar.BanSiZai;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.General;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.NPCs.Bosses.DyradEye
{
    [AutoloadBossHead]
    public class DyradEyeBoss : ModNPC
    {
        public enum AttackStyle
        {
            JustSpawn,
            BeginnerMode,
            Spinning,
            Charge,
            SmoothCharge
        }
        public ref float Timer => ref NPC.ai[0];
        public float GlobalLerp = 0;
        public AttackStyle State
        {
            get => (AttackStyle)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        public override string Texture => AssetHandler.DyradEye.Texture;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 16;
        }
        public override void SetDefaults()
        {
            NPC.width = 206;
            NPC.height = 124;
            NPC.damage = 25;
            NPC.defense = 16;
            NPC.lifeMax = 40000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = Item.buyPrice(1);
            NPC.knockBackResist = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.npcSlots = 10;
            NPC.lavaImmune = true;
            NPC.friendly = false;
        }
        public override void AI()
        {
            if (!NPC.IsLegalTarget())
            {

                NPC.TargetClosest();
            }
            Player player = Main.player[NPC.target];
            Timer++;
            if (player.dead)
            {
                NPC.active = false;
                return;

            }
            switch (State)
            {
                case AttackStyle.JustSpawn:
                    DoAI_JustSpawn(player);
                    break;
                case AttackStyle.BeginnerMode:
                    DoAI_BeginnerMode(player);
                    break;
            }

            base.AI();
        }
        /// <summary>
        /// 树妖眼具备一个新手教程
        /// <br>需要引入空归的核心格挡机制</br>
        /// </summary>
        /// <param name="player"></param>
        public  Projectile proj = null;
        public void DoAI_BeginnerMode(Player player)
        {
            //老大，直接使用追踪方法追这个玩家
            //开始追踪target
            //Vector2 home = (player.LocalMouseWorld()- NPC.Center).SafeNormalize(Vector2.UnitY);
            //Vector2 velo = (NPC.velocity * 16 + home * 8) / (16 + 1f);
            ////这里给了一个角度限制
            //NPC.velocity = velo;
            if (!NPC.HJScarlet().isBeingParry)
            {
                if (NPC.Distance(player.Center) > 16f * 15)
                {

                    //Move(player.Center, 24);
                    //开始追踪target
                    Vector2 home = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                    float maxtime = 60;
                    float lerpValue = Utils.GetLerpValue(0f, maxtime, Timer, true);
                    float inherit = 30f * (1 - lerpValue);
                    Vector2 velo = (NPC.velocity * inherit + home * 38 * EaseOutCubic(lerpValue)) / (inherit + 1f);
                    NPC.velocity = velo;
                    NPC.rotation = NPC.velocity.ToRotation();
                    NPC.spriteDirection = NPC.direction = (NPC.velocity.X > 0).ToDirectionInt();
                    NPC.dontTakeDamage = true;

                    if(Main.rand.NextBool())
                    ECSParticle.TurbulenceShinyOrb(NPC.ToRandRec(), 1.6f, Color.LimeGreen, 120, 1, 0.2f, RandRotTwoPi, .2f);
                    if(Main.rand.NextBool())
                    ECSParticle.LiliesPetal(NPC.ToRandRec(), NPC.velocity.ToSafeNormalize(), RandLerpColor(Color.LimeGreen, Color.DarkGreen), 100, 1, RandRotTwoPi, Main.rand.NextFloat(.8f, 1.1f)*.1f, 1f,fullBright:true,blendState:BlendState.AlphaBlend);
                }
                else
                {
                    if (player.velocity.LengthSquared() > 2f)
                        player.velocity *= .75f;
                    if (NPC.velocity.LengthSquared() >= 5f*5f)
                        NPC.velocity *= .75f;
                    else
                    {
                        if (NPC.velocity.LengthSquared() >= .05f)
                        {
                            NPC.velocity *= .75f;
                        }
                        if (player.velocity.LengthSquared() > .1f)
                            player.velocity *= .75f;
                        if (NPC.dontTakeDamage == true)
                        {
                            SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = .3f }, NPC.Center);
                            proj = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), player.Center - Vector2.UnitY * 35, Vector2.Zero, ProjectileType<GeneralStringProj>(), 0, 0);
                            proj.timeLeft = 180;
                            ((GeneralStringProj)proj.ModProjectile).TextValue = "按下 格挡键 以格挡";
                        }
                        NPC.dontTakeDamage = false;
                        ref float thisLerp = ref NPC.ai[2];
                        thisLerp = Lerp(thisLerp, 1f, 0.1f);
                        ScreenZoomSystem.ZoomIn(SmoothStep(0f, .37f, thisLerp));
                    }
                    NPC.rotation = NPC.velocity.ToRotation();
                    NPC.spriteDirection = NPC.direction = (NPC.velocity.X > 0).ToDirectionInt();
                }
            }
            else
            {
                if(proj is not null && proj.active && proj.timeLeft>10)
                {
                    proj.timeLeft = 10;
                }
                if (NPC.velocity.Y >= 1f)
                {
                    NPC.rotation = NPC.velocity.ToRotation();
                }
                else
                {
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Center.GetNormalVector2(player.Center).ToRotation(), .4f);
                    NPC.spriteDirection = NPC.direction = ((NPC.Center.X - player.Center.X) < 0).ToDirectionInt();
                }
            }
        }

        /// <summary>
        /// 刚生成的逻辑
        /// <br>考虑到可能使用祭坛，这里最后会重置一遍AI</br>
        /// </summary>
        public void DoAI_JustSpawn(Player player)
        {
            //目前还没有祭坛，直接跳转到新手教程AI
                          SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
            State = AttackStyle.BeginnerMode;
            Timer = 0;
        }
        void Move(Vector2 targetPos, float MaxSpeed = 20f)//之前教学的惯性追击方法
        {
            float accSpeed = 0.5f;//设定横纵向加速度
            if (NPC.Center.X - targetPos.X < 0f)
                NPC.velocity.X += NPC.velocity.X < 0 ? 2 * accSpeed : accSpeed;
            else
                NPC.velocity.X -= NPC.velocity.X > 0 ? 2 * accSpeed : accSpeed;

            if (NPC.Center.Y - targetPos.Y < 0f)
                NPC.velocity.Y += NPC.velocity.Y < 0 ? 2 * accSpeed : accSpeed;
            else
                NPC.velocity.Y -= NPC.velocity.Y > 0 ? 2 * accSpeed : accSpeed;
            if (Math.Abs(NPC.velocity.X) > MaxSpeed)//如果横向速度超越最大值，则回到最大值
                NPC.velocity.X = MaxSpeed * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > MaxSpeed)//如果纵向速度超越最大值，则回到最大值
                NPC.velocity.Y = MaxSpeed * Math.Sign(NPC.velocity.Y);
        }
        public void Do_SwitchAI(AttackStyle targetState)
        {
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D npcTex = TextureAssets.Npc[Type].Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            Vector2 ori = npcTex.Size() / 2f;
            float rotFixer = NPC.spriteDirection > 0 ? 0 : Pi;
            int length = NPC.oldPos.Length - 8;
            SpriteEffects se = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 shakeing = NPC.HJScarlet().isBeingParry ? Main.rand.NextVector2Circular(1.5f, 1.5f) : Vector2.Zero;
            float overAllScale = .9f;
            for (int i = 0; i < length; i++)
            {
                float progress = (float)i / length;
                Vector2 oldPos = NPC.oldPos[i] + NPC.Size / 2f - Main.screenPosition;
                float oldRot = NPC.oldRot[i] + rotFixer;
                Color c = Color.Lerp(Color.LightGreen, Color.LimeGreen, progress) * progress * .87f;
                float opac = Lerp(1f, .3f, progress);
                float scale = Lerp(1f, .45f, progress)*overAllScale;
                spriteBatch.FastDraw(npcTex, oldPos, c.ToAddColor() * opac * 1f, oldRot, ori, NPC.scale * scale * 1.1f, se);
                Color c2 = Color.Lerp(Color.White, Color.LimeGreen, progress) * progress * .9f;
                spriteBatch.FastDraw(npcTex, oldPos, c2.ToAddColor(20) * opac * .75f, oldRot, ori, NPC.scale * scale * 1, se);
            }
                        ref float thisLerp = ref NPC.ai[2];
            for (int i = 0; i < 8; i++)
                spriteBatch.FastDraw(npcTex, drawPos+shakeing+ (TwoPi / 8f * i).ToRotationVector2() * 1.2f, Color.White.ToAddColor()*thisLerp, NPC.rotation + rotFixer, ori, NPC.scale*overAllScale, se);
            spriteBatch.FastDraw(npcTex, drawPos+shakeing, Color.White, NPC.rotation + rotFixer, ori, NPC.scale*overAllScale, se);
            return false;
        }
    }
}
