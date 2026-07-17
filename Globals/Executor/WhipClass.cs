using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Globals.Executor
{
    public abstract class ExecutorWhipProj : HJScarletProj
    {
        public sealed override ClassCategory Category => ClassCategory.Executor;
        /// <summary>
        /// 这个射弹归属的物品，用于命中计时。
        /// </summary>
        public virtual int OriginalWhip => -1;
        /// <summary>
        /// 鞭子中，每个节点之间的连线。一般使用了原版的鱼线
        /// 这个是预留的，后续如果有自定义的连线，就会用到这个。
        /// </summary>
        public virtual (Texture2D LineTexture, Color LineColor, int LineEndCut, bool FullBright) LineSetting => (TextureAssets.FishingLine.Value, Color.White, 2, false);
        /// <summary>
        /// 鞭子的基础数据
        /// 第一个空位记录鞭子的段数，第二个空位记录绳子的长度系数，第三个空位记录额外更新
        /// </summary>
        public virtual (int SegmentCount, float RangeFactor, int ExtraUpdates, int SpriteFrames) WhipDefaults => (12, 0.6f, 0, 3);
        public virtual (int ExecutorProgressAdd, float PenetrateDamageRedcution) WhipHitDefaults => (1, 0.35f);
        /// <summary>
        /// 获取鞭头（尖端）在鞭子顶点列表中的索引值。
        /// <para>该值表示从鞭子根部（索引0）到鞭头所在的顶点索引偏移量。</para>
        /// <para>此值必须大于等于1，因为索引0始终预留给鞭子的根部节点；若设置为0或负值，将导致顶点索引越界或逻辑错误。</para>
        /// <para>子类可以重写此属性以调整鞭头在顶点链中的实际位置。</para>
        /// </summary>
        public virtual int HeadPosOffsetFactor => 1;
        /// <summary>
        /// 是否持续不断地记录末端节点的位置
        /// 用于绘制一些轨迹，比如顶点轨迹。
        /// 设置为true时，即可使用<see cref="HeadOldPosList"/>正常进行储存
        /// </summary>
        public virtual bool StoredHeadPosition => false;
        public List<Vector2> HeadOldPosList = [];
        public bool HasPlayedWhipSound = false;
        public ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.DamageType = ExecutorDamageClass.Instance;
            Projectile.WhipSettings.Segments = WhipDefaults.SegmentCount;
            Projectile.WhipSettings.RangeMultiplier = WhipDefaults.RangeFactor;
            Projectile.extraUpdates = WhipDefaults.ExtraUpdates;
            ExSD();
        }
        //
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 pos = TryGetPossibleHeadPos();
            return HJScarletMethods.LineThroughRect(Projectile.Center, pos, targetHitbox);
        }
        /// <summary>
        /// 一个工具方法，用于尝试直接获得末端位置
        /// <para>其中，索引使用了<see cref="HeadPosOffsetFactor"/></para>
        /// </summary>
        /// <returns></returns>
        public Vector2 TryGetPossibleHeadPos()
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);
            return list[^HeadPosOffsetFactor];
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(OriginalWhip, WhipHitDefaults.ExecutorProgressAdd);
            Projectile.damage = (int)(Projectile.damage * WhipHitDefaults.PenetrateDamageRedcution);
            ExOnHitNPC(target, hit, damageDone);
        }

        public virtual void ExOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) { }

        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + PiOver2;
            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;

            Projectile.spriteDirection = Projectile.velocity.X >= 0 ? 1 : -1;
            Timer++;
            Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out _, out _);
            if (Timer >= timeToFlyOut || Owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            Owner.heldProj = Owner.whoAmI;
            Owner.MatchItemTimeToItemAnimation();
            if (Timer >= timeToFlyOut / 2 && !HasPlayedWhipSound)
            {
                HasPlayedWhipSound = true;
                SoundEngine.PlaySound(SoundID.Item153, TryGetPossibleHeadPos());
            }
            if (StoredHeadPosition)
            {
                HeadOldPosList.Add(TryGetPossibleHeadPos());
            }
            float swingProgress = Timer / timeToFlyOut;
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f)
            {
                OnWhipActualSwinging(swingProgress);

            }
        }
        public virtual void OnWhipActualSwinging(float swingProgress) { }
        public void DrawLine(List<Vector2> list)
        {
            Rectangle frame = LineSetting.LineTexture.Frame();
            Vector2 ori = new Vector2(frame.Width / 2, 2);
            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - HeadPosOffsetFactor; i++)
            {
                Vector2 element = list[i];
                Vector2 differ = list[i + 1] - element;
                float rot = differ.ToRotation() - PiOver2;
                Color color = LineSetting.FullBright ? LineSetting.LineColor : Lighting.GetColor(element.ToTileCoordinates(), LineSetting.LineColor);
                Vector2 scale = new Vector2(1, (differ.Length() + 2) / frame.Height);
                SB.Draw(LineSetting.LineTexture, pos - Main.screenPosition, frame, color, rot, ori, scale, 0, 0);
                pos += differ;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);
            DrawLine(list);
            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Rectangle frame = tex.Frame(1, WhipDefaults.SpriteFrames);
            Vector2 origin = frame.Size() / 2f;
            //没法再省了，只能这样了
            DrawWhipInPreDraw(list, tex, frame, origin, flip);
            return false;
        }

        public virtual void DrawMiscOnHead(Vector2 vector2, SpriteEffects flip)
        {
        }

        public virtual void DrawWhipInPreDraw(List<Vector2> list, Texture2D texture, Rectangle frame, Vector2 origin, SpriteEffects flip) { }
    }
}
