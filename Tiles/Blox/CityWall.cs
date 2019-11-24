﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace prismmod.Tiles.Blox
{
    public class CityWall : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            drop = ItemType<Items.Placeables.MoistChiseledStone>();
            AddMapEntry(new Color(200, 200, 200));
            mineResist = 7f;
            minPick = 100;
            
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 74/255;
            g = 71/255;
            b = 245/255;
        }
    }
}