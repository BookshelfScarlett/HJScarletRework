using HJScarletRework.Globals.Configs;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances.Items
{
    public partial class HJScarletGlobalItem : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.type == ItemID.PocketMirror)
            {
                player.HJScarlet().pocketMirror = !hideVisual;
            }
        }
        public override void UpdateVanity(Item item, Player player)
        {
            if (item.type == ItemID.PocketMirror)
            {
                player.HJScarlet().pocketMirror = true;
            }
        }
        public override void UpdateInfoAccessory(Item item, Player player)
        {
            base.UpdateInfoAccessory(item, player);
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (HJScarletConfigClient.Instance.DrawIcon && CanDrawGhost)
            {
                //在UpdateInventory内更新帧图的绘制，因为tooltip的draw实际上只会执行一次
                GhostTimer++;
                if (GhostTimer > 5)
                {
                    GhostFrame++;
                    GhostTimer = 0;
                }
                if (GhostFrame >= 16)
                    GhostFrame = 1;
            }
            if (!setTintIcon)
            {
                tintIconDrawLerp = Lerp(tintIconDrawLerp, 0f, 0.2f);
                if (tintIconDrawLerp <= 0.02f)
                    tintIconDrawLerp = 0;
            }
            else
            {
                tintIconDrawLerp = Lerp(tintIconDrawLerp, 1f, 0.2f);
                if (tintIconDrawLerp >= 0.98f)
                    tintIconDrawLerp = 1;
            }
            setTintIcon = false;
        }

    }

}
