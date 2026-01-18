using HJScarletRework.Assets.Registers;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class Judgement: ThrownHammerItem
    {
        public override int ShootProjID => ProjectileType<JudgementMainProj>();
        public override int NeedFocusStrikeTime => 30;
        public override void ExSD()
        {
            Item.width = Item.height = 58;
            Item.damage = 50;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.LightRed;
        }
        public override float StealthDamageMultipler => 0.25f;
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, Color.White with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.UltraBright);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Pwnhammer).
                AddIngredient(ItemID.SoulofMight, 10).
                AddIngredient(ItemID.SoulofFright, 10).
                AddIngredient(ItemID.SoulofSight, 10).
                AddIngredient(ItemID.Diamond, 5).
                AddIngredient(ItemID.Amethyst, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
