using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class HJScarletHeldProj: ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => $"Projs.HeldProj.{Category}";
        /// <summary>
        /// 职业类型，用于本地化的分类
        /// </summary>
        public virtual ClassCategory Category { get; }
        /// <summary>
        /// 贴图转角修正
        /// </summary>
        public virtual float RotFix => 0;
        /// <summary>
        /// 转向速度
        /// </summary>
        public virtual float RotAmount => 1f;
        public Player Owner => Main.player[Projectile.owner];

        public Vector2 LocalMouseWorld => Owner.LocalMouseWorld();
        public bool Active => (Owner.channel || Owner.controlUseTile) && !Owner.noItems && !Owner.CCed;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public int UseDelay = 0;

        public override void AI()
        {
            if (Projectile.HJScarlet().FirstFrame)
                Initialize();

            Projectile.netImportant = true;

            if (UseDelay > 0)
                UseDelay--;

            if (!Owner.active || Owner.dead)
                Projectile.Kill();

            Projectile.extraUpdates = 0;

            Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            UpdateAim(Projectile.Center);

            // Update the Prism's position in the world and relevant variables of the player holding it.
            UpdatePlayerVisuals(Owner, rrp);

            if (StillInUse())
            {
                if (Projectile.owner == Main.myPlayer)
                    HoldoutAI();

                ExtraHoldoutAI();
            }
            else if (CanDel())
            {
                InDel();
            }

            Projectile.netSpam = 0;
            Projectile.netUpdate = true;
            // 确保不会使用的时候消失
            Projectile.timeLeft = 2;
        }
        public virtual bool StillInUse()
        {
            return Active;
        }
        public virtual void Initialize() { }
        public virtual bool CanDel()
        {
            return UseDelay <= 0 && Projectile.owner == Main.myPlayer;
        }
        #region 更新玩家视觉效果
        public virtual void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {

            Projectile.Center = playerHandPos;
            Projectile.rotation = (Owner.MountedCenter - LocalMouseWorld).SafeNormalize(Vector2.UnitX).ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            player.ChangeDir(LocalMouseWorld.X - player.MountedCenter.X > 0 ? 1 : -1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        #endregion

        #region 更新弹幕朝向
        public virtual void UpdateAim(Vector2 source)
        {
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
            if (aim.HasNaNs())
            {
                aim = -Vector2.UnitY;
            }

            aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, RotAmount));

            Projectile.velocity = aim;
        }
        #endregion

        /// <summary>
        /// 手持弹幕的AI逻辑<br/>
        /// </summary>
        public virtual void HoldoutAI()
        {

        }

        /// <summary>
        /// 手持弹幕的AI逻辑，但是没有只在本地玩家运行的限制<br/>
        /// </summary>
        public virtual void ExtraHoldoutAI()
        {

        }
        /// <summary>
        /// 删除条件<br/>
        /// </summary>
        public virtual void InDel()
        {
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ExtraPreDraw(ref lightColor);
            return false;
        }
        private SpriteBatch SB { get => Main.spriteBatch; }
        public virtual bool ExtraPreDraw(ref Color lightColor)
        {
            Projectile.GetHeldProjDrawState(RotFix, out Texture2D projTex, out Vector2 drawPos, out Vector2 drawOri, out float projRot, out SpriteEffects projSP);
            SB.Draw(projTex, drawPos, null, Projectile.GetAlpha(lightColor), projRot, drawOri, Projectile.scale * Owner.gravDir, projSP, default);
            MorePreDraw(ref lightColor);
            return false;
        }
        public virtual void MorePreDraw(ref Color lightColor)
        {
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
