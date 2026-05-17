using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Executor
{
    public abstract class ExecutorHeldProj : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public virtual ClassCategory Category => ClassCategory.Executor;
        public virtual int OriginalItemID => -1;
        public static string ProjPath => $"HJScarletRework/Assets/Texture/Projs/";
        public override string Texture => ProjPath + GetType().Name;
        public new string LocalizationCategory => $"Projs.Friendly.{Category}";
        public bool PerformanceMode = HJScarletConfigClient.Instance.PerformanceMode;
        public virtual Vector2 TileHitbox => Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = ExecutorDamageClass.Instance;
            ExSD();
        }
        public virtual void OnFirstFrame() { }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                OnFirstFrame();
            ProjAI();
        }
        /// <summary>
        /// 直接管理的工具方法。
        /// </summary>
        /// <returns></returns>
        public bool HandleExecution()
        {
            if(Owner.CheckExecution(OriginalItemID) && !Projectile.HJScarlet().ExecutionStrike)
            {
                Projectile.HJScarlet().ExecutionStrike = true;
                Owner.RemoveExecutionProgress(OriginalItemID);
                OnExecution();
                return true;
            }
            Owner.HJScarlet().CanExecution = false;
            return false;
        }
        public virtual void OnExecution() { }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (!TileHitbox.Equals(Vector2.Zero))
            {
                width = (int)TileHitbox.X;
                height = (int)TileHitbox.Y;
            }
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public virtual void ProjAI() { }

        public virtual void ExSD() { }
    }
}
