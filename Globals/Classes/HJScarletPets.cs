using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
    public abstract class ScarletPetProj : ModProjectile,ILocalizedModType
    {
        public new string LocalizationCategory => "Projs.Friendly.Pets";
        public static string TexturePath = "HJScarletRework/Assets/Texture/Pets/Pet_";
        public override string Texture => TexturePath + GetType().Name;
        public Player Owner => Main.player[Projectile.owner];
        /// <summary>
        /// 帧图张数
        /// </summary>
        public virtual int PetFrames => 1;
        /// <summary>
        /// 是否为发光宠物
        /// </summary>
        public virtual bool LightPets => false;
        /// <summary>
        /// 发起传送的距离
        /// </summary>
        public virtual float TeleportThreshold => 1440f;
        /// <summary>
        /// 是否允许传送
        /// </summary>
        public virtual bool CanTeleport => true;

        //封住他避免意外复写
        public sealed override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = PetFrames;
            if (LightPets)
                ProjectileID.Sets.LightPet[Type] = true;
        }
        public sealed override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            ExSD();
        }
        public virtual void ExSD() { }

        public virtual void ExSSD() { }
        /// <summary>
        /// 用于处理宠物的一些正确与否的存活态，如：主人存续等
        /// </summary>
        public virtual void SimplePetFunction()
        {

        }

        /// <summary>
        /// 序列帧处理，这里推荐直接填入至SimplePetFunction钩子内。
        /// </summary>
        /// <param name="Speed"></param>
        public void SimplePetAnimation(float Speed)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > Speed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;
            }
        }
        /// <summary>
        /// 获取宠物贴图的“左”或者“右”朝向。即贴图本身的朝向，以及一些别的东西
        /// 对于宠物如果是左右堆成的情况，这里应该是无所谓的。
        /// </summary>
        /// <param name="FaceLeft">贴图朝向，默认为真，即朝向左方</param>
        /// <param name="ShouldFiip">是否需要手动操作贴图朝向，默认为真</param>
        public virtual void GetPetSpriteState(out bool FaceLeft, out bool ShouldFiip)
        {
            FaceLeft = true;
            ShouldFiip = true;
        }
        /// <summary>
        /// 宠物发起传送时，会进行什么行为
        /// 用于SimpleAI内，且如果禁用传送时，也不会调用该钩子
        /// 同时内部什么都没有。需要自己手动复写
        /// </summary>
        public virtual void OnTeleport() { }


    }
    public abstract class ScarletFloatingPet : ScarletPetProj
    {
        public override void AI()
        {
            if (!Owner.active)
            {
                Projectile.active = false;
                return;
            }
            SimpleAI();
            ExAI();
        }
        /// <summary>
        /// 飞行宠物的其余AI，后于SimpleAI进行
        /// 一般情况下直接用CustomBehavior就足够，这里一般推荐是用于进行别的行为，如生成一些新的粒子之类的。
        /// </summary>

        public virtual void ExAI()
        {

        }
        /// <summary>
        /// 飞行宠物的简易AI。如果需要的话可以直接复写，也可以不用。
        /// </summary>
        public virtual void SimpleAI()
        {
            GetPetSpriteState(out bool FacesLeft, out bool ShouldFlip);
            //处理宠物贴图朝向相关
            HandleSpriteDirection(FacesLeft, ShouldFlip);
            Movement();
            SimplePetFunction();
        }
        public virtual Vector2 GetIdlePos(Player player) => new Vector2(player.direction * 30f, -30f);
        public virtual void GetIdleState(out float MinDistance, out float SpeedMult, out float FloatingMult)
        {
            MinDistance = 2f;
            SpeedMult = 2f;
            FloatingMult = 5f;
        }
        private bool Movement()
        {

            //计算需要的常态待机位置
            int dir = Owner.direction;
            Projectile.direction = Projectile.spriteDirection = dir;

            Vector2 desiredCenterRelative = GetIdlePos(Owner);
            GetIdleState(out float MinDistance, out float SpeedMult, out float FloatingMult);
            //增加需要的上下浮动
            desiredCenterRelative.Y += (float)Math.Sin(Main.GameUpdateCount / 120f * TwoPi) * FloatingMult;
            //而后我们设置当前的位置，并根据当前位置获取未知的差值……额我也不知道我在说啥，反正看得懂就行了
            Vector2 desiredCenter = Owner.MountedCenter + desiredCenterRelative;
            Vector2 betweenDirection = desiredCenter - Projectile.Center;
            //我们只需要一个距离，使用LengthSquared可以很好地减少开销
            float betweenSQ = betweenDirection.LengthSquared();
            //假如需要的位置与宠物的距离过远，我们直接让他传送到需要的位置(或者是过近)
            bool TooFarAway = betweenSQ > TeleportThreshold * TeleportThreshold; 
            if (TooFarAway || betweenSQ < MinDistance * MinDistance)
            {
                Projectile.Center = desiredCenter;
                Projectile.velocity = Vector2.Zero;
                //如果是距离过远的情况才会执行这个AI
                if (TooFarAway)
                    OnTeleport();
            }
            if (betweenDirection != Vector2.Zero)
            {
                Projectile.velocity = betweenDirection * 0.1f * SpeedMult;
            }
            //如果需要更快的速度的话……
            bool movesFast = Projectile.velocity.LengthSquared() > 6f * 6f;

            if (movesFast)
            {
                float rotationVel = Projectile.velocity.X * 0.08f + Projectile.velocity.Y * Projectile.spriteDirection * 0.02f;
                if (Math.Abs(Projectile.rotation - rotationVel) >= Pi)
                {
                    if (rotationVel < Projectile.rotation)
                    {
                        Projectile.rotation -= TwoPi;
                    }
                    else
                    {
                        Projectile.rotation += TwoPi;
                    }
                }

                float rotationInertia = 12f;
                Projectile.rotation = (Projectile.rotation * (rotationInertia - 1f) + rotationVel) / rotationInertia;
            }
            else
            {
                //如果移动速度比较常规，则用常规的平滑移动。
                if (Projectile.rotation > Pi)
                {
                    Projectile.rotation -= TwoPi;
                }

                if (Projectile.rotation > -0.005f && Projectile.rotation < 0.005f)
                {
                    Projectile.rotation = 0f;
                }
                else
                {
                    Projectile.rotation *= 0.96f;
                }
            }

            return movesFast;
        }
        /// <summary>
        /// 直接封装的一个方法来处理宠物贴图的朝向。
        /// </summary>
        /// <param name="FacesLeft"></param>
        /// <param name="ShouldFlip"></param>
        public void HandleSpriteDirection(bool FacesLeft, bool ShouldFlip)
        {
            if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 0)
            {
                if (FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
                else if (!FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            }
        }
    }
    public abstract class ScarletFlyingPet :  ScarletPetProj
    {
        public enum FlyingPetState
        {
            Flying,
            Custom
        }
        public FlyingPetState State
        {
            get => (FlyingPetState)Projectile.HJScarlet().ExtraAI[0];
            set => Projectile.HJScarlet().ExtraAI[0] = (float)value;
        }
        public bool IsSpeedingUp = false;
        public override void AI()
        {
            //主人本身如果死掉直接干掉
            if(!Owner.active)
            {
                Projectile.active = false;
                return;
            }
            SimpleAI();
            ExAI();            
           
        }

        /// <summary>
        /// 飞行宠物的其余AI，后于SimpleAI进行
        /// 一般情况下直接用CustomBehavior就足够，这里一般推荐是用于进行别的行为，如生成一些新的粒子之类的。
        /// </summary>
        public virtual void ExAI() { }

        /// <summary>
        /// 飞行宠物的简易AI。如果需要的话可以直接复写，也可以不用。
        /// </summary>
        public virtual void SimpleAI()
        {
            GetPetSpriteState(out bool FacesLeft, out bool ShouldFlip);
            GetPetFlyingState(out float FlyingSpeed, out float FlyingIneritia, out bool ShouldSpeedUp, out float SpeedUpThreshold);
            GetPetIdleState(out Vector2 FlyingOffset, out float FlyingArea, out float FlyingDrag, out bool ShouldFlyRotate);
            if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 0)
            {
                if (FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
                else if (!FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            }
            SimplePetFunction();
            Vector2 ProjectileCenter = Projectile.Center;
            Vector2 vectorToPlayer = Owner.Center - ProjectileCenter;
            float lengthToPlayer = vectorToPlayer.Length();
            //超出距离立刻传送
            if (lengthToPlayer > TeleportThreshold && CanTeleport)
            {
                Projectile.position = Owner.Center;
                Projectile.velocity *= 0.1f;
                OnTeleport();
            }

            float flyingSpeed = FlyingSpeed;
            float flyingInertia = FlyingIneritia;

            if ((lengthToPlayer > SpeedUpThreshold) && ShouldSpeedUp)
            {
                flyingSpeed *= 1.25f;
                flyingInertia *= 0.75f;
                IsSpeedingUp = true;
            }
            else
            {
                IsSpeedingUp = false;
            }
            ModifyFlyingState(ref flyingSpeed, ref flyingInertia);
            switch (State)
            {
                case FlyingPetState.Flying:
                    Projectile.tileCollide = false;
                    vectorToPlayer += FlyingOffset;
                    lengthToPlayer = vectorToPlayer.Length();

                    if (lengthToPlayer > FlyingArea)
                    {
                        vectorToPlayer.Normalize();
                        vectorToPlayer *= flyingSpeed;

                        if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.velocity = new Vector2(-0.15f);
                        }

                        if (flyingInertia != 0f && flyingSpeed != 0f)
                            Projectile.velocity = (Projectile.velocity * (flyingInertia - 1) + vectorToPlayer) / flyingInertia;
                    }
                    else
                    {
                        Projectile.velocity *= FlyingDrag;
                    }

                    if (ShouldFlyRotate)
                        Projectile.rotation = Projectile.velocity.X * 0.1f;
                    break;
                case FlyingPetState.Custom:
                    ModifyExtraBehaviour(ref flyingSpeed, ref flyingInertia);
                    break;
            }
        }
        /// <summary>
        /// 复写飞行动画之外，这条钩子用于添加更多的AI。
        /// 在执行这条钩子的时候，State状态将会为Custom，直到你手动复原为飞行动画为止
        /// </summary>
        /// <param name="flyingSpeed"></param>
        /// <param name="flyingInertia"></param>
        private void ModifyExtraBehaviour(ref float flyingSpeed, ref float flyingInertia)
        {
        }

        
        /// <summary>
        /// 获取宠物是否允许传送、飞行速度等一些别的东西
        /// 即宠物跟随玩家行为的基本数据，这些会直接用于SimpleAI里面
        /// 如果直接复写了SimpleAI的话，这里的方法需要重新调用
        /// </summary>
        /// <param name="FlyingSpeed">跟随速度，默认取16f</param>
        /// <param name="FlyingIneritia">跟随惯性，默认值12f</param>
        public virtual void GetPetFlyingState(out float FlyingSpeed, out float FlyingIneritia , out bool ShouldSpeedUp, out float SpeedUpThreshold)
        {
            FlyingSpeed = 16f;
            FlyingIneritia = 12f;
            ShouldSpeedUp = true;
            SpeedUpThreshold = 800f;
        }
        /// <summary>
        /// 修改宠物的飞行速度与飞行惯性。
        /// </summary>
        /// <param name="FlyingSpeed"></param>
        /// <param name="Flyingineritia"></param>
        public virtual void ModifyFlyingState(ref float FlyingSpeed, ref float Flyingineritia) { }
        /// <summary>
        /// 宠物环绕在玩家身边时，一些状态的数据
        /// </summary>
        /// <param name="FlyingOffset">宠物与玩家的偏移位置，默认玩家背后上方</param>
        /// <param name="FlyingArea">宠物环绕在玩家周围飞行的区域</param>
        /// <param name="FlyingDrag">what</param>
        /// <param name="ShouldFlyRotate"></param>
        public virtual void GetPetIdleState(out Vector2 FlyingOffset, out float FlyingArea, out float FlyingDrag, out bool ShouldFlyRotate)
        {
            FlyingOffset = new(48f * -Owner.direction, -50f);
            FlyingArea = 20f;
            FlyingDrag = 1f;
            ShouldFlyRotate = true;
        }
    }
}
