using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs;

namespace HJScarletRework.Projs.Executor
{
    /// <summary>
    /// 这个标记是隐形标记，没有任何作用
    /// <br>鞭类的效果后续再进行修改</br>
    /// </summary>
    public class TearEyeMark : KnifeMarkClass
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExtraFirstFrame()
        {
            base.ExtraFirstFrame();
        }
        public override void ExProjAI()
        {
            Owner.AddBuff(BuffType<TearEyeBuff>(), 2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    /// <summary>
    /// 这个标记是隐形标记，没有任何作用
    /// <br>鞭类的效果后续再进行修改</br>
    /// </summary>
    public class StarofHoperMark : KnifeMarkClass
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void ExtraFirstFrame()
        {
            base.ExtraFirstFrame();
        }
        public override void ExProjAI()
        {
            Owner.AddBuff(BuffType<StarofHopeBuff>(), 2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

}
