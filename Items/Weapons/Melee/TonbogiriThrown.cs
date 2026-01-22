using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class TonbogiriThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<Tonbogiri>().Texture;
        public float CurrentAttackSpeedBonues = 0f;
        public static readonly int MaxBubbles = 8;
        public override void SetStaticDefaults()
        {
            Type.ShimmerEach<Tonbogiri>();
            ItemID.Sets.BonusAttackSpeedMultiplier[Type] = 2f;
        }
        public override void ExSD()
        {
            Item.damage = 106;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 12f;
            Item.shootSpeed = 16;
            //这里的shoot是为了适配weaponoutlite。实际上我们不会直接shoot这个东西
            Item.shoot = ProjectileType<TonbogiriThrownProj>();
            Item.UseSound = SoundID.Item71;
        }
        public override Color MainTooltipColor => Color.SkyBlue;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string localAddress = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}");
            string path = $"{localAddress}.Tooltip";
            tooltips.ReplaceAllTooltip(path, MainTooltipColor, GetBubblesCount, MaxBubbles);

            if (HJScarletMethods.HasFuckingCalamity)
            {
                string calamityPath = $"{localAddress}.CalamitySupport";
                tooltips.QuickAddTooltipDirect(calamityPath.ToLangValue(), new(220, 20, 6));
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float attackSpeed = player.GetTotalAttackSpeed<MeleeDamageClass>() * 1.24f;
            int theCritBonuse = (int)player.GetTotalCritChance<MeleeDamageClass>();
            theCritBonuse -= 50;
            if (theCritBonuse < 0)
                theCritBonuse = 0;
            int totalBubble = theCritBonuse * (int)attackSpeed;
            totalBubble = (int)Clamp(totalBubble, 1, MaxBubbles);
            Projectile proj =  Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().ExtraAI[0] = totalBubble;
            return false;
        }
        private int GetBubblesCount
        {
            get
            {
                Player player = Main.LocalPlayer;
                float attackSpeed = player.GetTotalAttackSpeed<MeleeDamageClass>() * 1.24f;
                int theCritBonuse = (int)player.GetTotalCritChance<MeleeDamageClass>();
                theCritBonuse -= 50;
                if (theCritBonuse < 0)
                    theCritBonuse = 0;
                int totalBubble = theCritBonuse + (int)attackSpeed;
                totalBubble = (int)Clamp(totalBubble, 1, MaxBubbles);
                return totalBubble;
            }
        }
    }
}
