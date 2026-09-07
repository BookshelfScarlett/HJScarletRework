using HJScarletRework.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.AccessoriesSlot
{
    public class HJScarletAccessoriesSlot : ModAccessorySlot
    {
        public override string Name => "LightofHorizon";
        public override bool IsEnabled()
        {
            bool isEnable = Main.LocalPlayer.TryGetModPlayer<HJScarletPlayer>(out var value);
            if (isEnable)
                isEnable = value.LightofHorizon;
            return isEnable;
        }
    }
    public class CombatSlotAcceesorySlot : ModAccessorySlot
    {
        public override string Name => "CombatSlot";
        public override string FunctionalBackgroundTexture => GetInstance<CombatSlot>().Texture;
        public override bool IsEnabled()
        {
            bool isEnable = Main.LocalPlayer.TryGetModPlayer<HJScarletPlayer>(out var value);
            if (isEnable)
                isEnable = value.combatSlot;
            return isEnable;
        }
    }
}
