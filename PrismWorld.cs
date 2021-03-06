using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

using prismmod.NPCs.WaterTown;
using prismmod.Tiles.Blox;
using prismmod.Walls;
using static prismmod.PrismHelper;
using Terraria.ModLoader.IO;

namespace prismmod
{
    internal class PrismWorld : ModWorld
    {
        public static int moistChiseledStoneCount = 0;
        public bool downedGargantuanTortoise;
        public bool downedPrismachine;
        public bool accessedWaterTown;
        public int gatesY;
        public int gatesX;
        public int bsb;

        public override void Initialize()
        {
            downedGargantuanTortoise = false;
            downedPrismachine = false;
            accessedWaterTown = false;
        }

        public override void Load(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            downedPrismachine = downed.Contains("Prismachine");
            downedGargantuanTortoise = downed.Contains("GargantuanTortoise");

            var accessed = tag.GetList<string>("accessed");
            accessedWaterTown = accessed.Contains("WaterTown");
        }

        public override TagCompound Save()
        {
            var downed = new List<string>();
            if (downedPrismachine)
                downed.Add("Prismachine");
            if (downedGargantuanTortoise)
            {
                downed.Add("GargantuanTortoise");
            }

            var accessed = new List<string>();
            if (accessedWaterTown)
                accessed.Add("WaterTown");
            return new TagCompound
            {
                ["downed"] = downed,
                ["accessed"] = accessed
            };
        }


        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = downedGargantuanTortoise;
            flags[1] = downedPrismachine;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedGargantuanTortoise = flags[0];
            downedPrismachine = flags[1];
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            
            /*int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (genIndex != -1)
            {
                tasks.Insert(genIndex + 1, new PassLegacy("Reserved Prismmod Test Space", delegate (GenerationProgress progress)
                {
                    progress.Message = "Test Code Here ;)";
                }));
            }*/

            int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Water Chests"));
            if (genIndex != -1)
            {
                tasks.Insert(genIndex + 1, new PassLegacy("Generate  Water Town", delegate (GenerationProgress progress)
                {

                    int operation=1;
                    bool wtRight;
                    int startXTunnel;
                    int endXTunnel;
                    int endXBiome;
                    if (WorldGen.dungeonX < Main.maxTilesX / 2)
                    {
                        wtRight = true;
                        operation = -1;
                        startXTunnel = Main.maxTilesX-50;
                        endXTunnel = Main.maxTilesX-63;
                        endXBiome = Main.maxTilesX-151;

                    }
                    else
                    {
                        wtRight = false;
                        operation = 1;
                        startXTunnel = 59;
                        endXTunnel = 72;
                        endXBiome = 210;
                    }
                    progress.Message = "Finding some Sand";
                    int baseSandBlock = 0;
                    for (int y = 0; y < Main.maxTilesY;y++)
                    {
                        Tile tile = Framing.GetTileSafely(startXTunnel,y);
                        if (tile.active() && tile.liquid == 0)
                        {
                            baseSandBlock = y;
                            break;
                        }
                    }

                    bsb = baseSandBlock;

                    int gateBlock = ModContent.TileType<UnbreakableGate>();
                    bool placedGate = false;//used to check if gateX and gateY should be set
                    int gateY = 0;
                    int gateX;

                    progress.Message = "Tunneling";
                    int activeBlock = ModContent.TileType<CityWall>();

                    for (int xCoord = startXTunnel; (operation*xCoord < operation*endXTunnel); xCoord=xCoord+operation)
                    {
                        for (int yCoord = baseSandBlock-1; yCoord < baseSandBlock+15; yCoord++)
                        {
                            Tile tile = Framing.GetTileSafely(xCoord, yCoord);
                            tile.ClearTile();
                            if ((xCoord == startXTunnel || xCoord == endXTunnel-operation)
                            && ((Framing.GetTileSafely(startXTunnel-operation, yCoord).liquid <= 2 && Framing.GetTileSafely(startXTunnel-operation, yCoord).active())
                            || (Framing.GetTileSafely(endXTunnel, yCoord).liquid <= 2 && Framing.GetTileSafely(endXTunnel, yCoord).active()))
                            || (Framing.GetTileSafely(xCoord, yCoord - 1).type == ModContent.TileType<CityWall>()))
                            {
                                if (!placedGate)
                                {
                                    placedGate = true;
                                    gateY = yCoord;
                                    gateX = xCoord;
                                }
                                WorldGen.PlaceTile(xCoord, yCoord, activeBlock);
                            }
                            else
                            {
                                Tile target = Framing.GetTileSafely(xCoord, yCoord);
                                target.liquid = 255;
                                target.liquidType(0);
                                target.liquid = 255;

                            }
                        }
                        gatesY = gateY;
                        WorldGen.PlaceTile(xCoord,gateY, gateBlock);

                    }

                    //Framing.GetTileSafely(gateX, gateY);
                    progress.Message = "Main Biome";
                    for (int xCoord = startXTunnel; operation * xCoord < endXBiome * operation; xCoord = xCoord + operation)
                    {
                        for (int yCoord = baseSandBlock+15; yCoord < baseSandBlock+175; yCoord++)
                        {
                            Tile tile = Framing.GetTileSafely(xCoord, yCoord);
                            tile.ClearEverything();
                            if (((xCoord == startXTunnel || xCoord == endXBiome - operation) || (yCoord == baseSandBlock+174 || yCoord == baseSandBlock+15)) && !(yCoord == baseSandBlock+15 && (xCoord* operation > startXTunnel* operation && xCoord*operation < (endXTunnel-1)*operation)))
                            {
                                WorldGen.PlaceTile(xCoord, yCoord, activeBlock);
                            }
                            else
                            {
                                tile.liquid = 255;
                                tile.liquidType(0);
                                tile.liquid = 255;
                                //tile.wall = (ushort)ModContent.WallType<Placeholder>();
                            }
                        }
                    }

                    int topY;
                    int botY;
                    int rightX;
                    int leftX;
                    topY = baseSandBlock+15;
                    botY = baseSandBlock+175;
                    if (wtRight)
                    {
                        rightX = startXTunnel;
                        leftX = endXBiome;
                    }
                    else 
                    {
                        rightX = endXBiome;
                        leftX = startXTunnel;
                    }

                    PrismHelper.drawBaseFishHouse(leftX+2, topY+30, 15, 15, ModContent.TileType<MoistChiseledStone>());

                    progress.Message = "Importing Fish People";
                    NPC.NewNPC((startXTunnel+endXBiome/2)*16, (Main.spawnTileY + 130)*16,ModContent.NPCType<FishBlue>());

                }));
            }
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            moistChiseledStoneCount = tileCounts[ModContent.TileType<Tiles.Blox.CityWall>()]; //Change back to chiseled stone
        }
    }
}