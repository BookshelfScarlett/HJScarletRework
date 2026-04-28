using HJScarletRework.Items.Armor.Monk;
using HJScarletRework.Items.Armor.Shinobi;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Systems
{
    public class HJScarletILSystem : ModSystem
    {
        public override void OnModLoad()
        {
            On_ShimmerTransforms.IsItemTransformLocked += On_ShimmerTransforms_IsItemTransformLocked;
        }

        private bool On_ShimmerTransforms_IsItemTransformLocked(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
        {
            if (type == ItemType<MonkHead>() || type == ItemType<ShinobiHead>())
            {
                return !Condition.DownedGolem.IsMet();
            }
            if (type == ItemType<MonkBody>() || type == ItemType<ShinobiBody>())
            {
                return !Condition.DownedGolem.IsMet();

            }
            if (type == ItemType<MonkLegs>() || type == ItemType<ShinobiLegs>())
            {
                return !Condition.DownedGolem.IsMet();
            }
            return orig(type);

        }
    }
}
