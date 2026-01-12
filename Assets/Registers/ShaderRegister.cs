using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace HJScarletRework.Assets.Registers
{
    public class HJScarletShader: ModSystem
    {        
        // 当未提供特定着色器时，用作基本绘图的默认值。此着色器仅渲染顶点颜色数据，无需修改。
        private const string ShaderPath = "HJScarletRework/Assets/Effects/";
        internal const string ShaderPrefix = "HJScarletRework";
        public static Effect TerrarRayLaser;
        public static Effect VolcanoEruptingShader;
        public static Effect Pixelation;
        public static Effect MetaBallShader;
        public override void Load()
        {
            if (Main.dedServ)
                return;

            static Effect LoadShader(string path)
            {
                return Request<Effect>($"{ShaderPath}{path}", AssetRequestMode.ImmediateLoad).Value;
            }
            TerrarRayLaser = LoadShader(nameof(TerrarRayLaser));
            VolcanoEruptingShader = LoadShader(nameof(VolcanoEruptingShader));
            Pixelation = LoadShader(nameof(Pixelation));
            MetaBallShader = LoadShader(nameof(MetaBallShader));

            RegisterMiscShader(TerrarRayLaser, "HJScarletReworkTerrarRayLaserPass", nameof(TerrarRayLaser));
            RegisterMiscShader(VolcanoEruptingShader, ToPassName(nameof(VolcanoEruptingShader)), nameof(VolcanoEruptingShader));
            RegisterMiscShader(Pixelation, ToPassName(nameof(Pixelation)), nameof(Pixelation));
            RegisterMiscShader(MetaBallShader, ToPassName(nameof(MetaBallShader)), nameof(MetaBallShader));
        }
        public override void Unload()
        {
            TerrarRayLaser = null;
            VolcanoEruptingShader = null;
            Pixelation = null;
            MetaBallShader = null;
        }
        public static string ToPassName(string oriShadername) => ShaderPrefix + oriShadername + "Pass";
        public static void RegisterMiscShader(Effect shader, string passName, string registrationName)
        {
            Ref<Effect> shaderPointer = new(shader);
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
            GameShaders.Misc[$"{ShaderPrefix}:{registrationName}"] = passParamRegistration;
        }
    }
}
