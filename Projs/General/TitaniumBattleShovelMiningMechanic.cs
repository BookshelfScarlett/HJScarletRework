using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.ColdSteel;
using HJScarletRework.Projs.Executor;
using Terraria;

namespace HJScarletRework.Projs.General
{
    public class TitaniumBattleShovelMiningMechanic : HJScarletProj
    {
        public override EnumDamageClass Category => EnumDamageClass.Typeless;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public int ParentID
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.SetUpHeldProj();
            Projectile.noEnchantmentVisuals = true;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void ProjAI()
        {
            if (ParentID != 0)
            {
                Projectile proj = Main.projectile[ParentID];
                if (proj.IsLegalFriendlyProj() && proj.type == ProjectileType<TitaniumBattleShovelHeldProj>() && Owner.IsHolding<TitaniumBattleShovel>())
                {
                    proj.timeLeft = 2;
                }
                else
                    proj.Kill();
            }
            else
                Projectile.Kill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int radius = 32;
            Main.NewText(1);
            if (Main.dedServ)
            {
                Point center = Projectile.Center.ToTileCoordinates();
                NetMessage.SendTileSquare(-1, center.X - radius * 2, center.Y - radius * 2, 4 * radius);
            }
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    int tilePosX = (int)(i + Projectile.Center.X / 16f);
                    int tilePosY = (int)(j + Projectile.Center.Y / 16f);
                    if (tilePosX >= 0 && tilePosX < Main.maxTilesX && tilePosY >= 0 && tilePosY < Main.maxTilesY)
                    {
                        Tile tile = Main.tile[tilePosX, tilePosY];
                        if (i * i + j * j <= radius && !tile.IsActuated)
                        {
                            if (WorldGen.CanKillTile(tilePosX, tilePosY))
                            {
                                WorldGen.KillTile(tilePosX, tilePosY);

                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
