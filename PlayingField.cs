using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Threading;

namespace sweep
{
    public static class PlayingField
    {
        public static int GridSquare = 25;
        public static int MineAmount = 150;
        public static int MinesLeft;
        public static int IndicatorAmount = 1;
        public static int IndicatorLeft;
        public static Grid TileGrid;
        public static Random random = new Random();
        public static TextBlock ScoreBlock;
        public static Dispatcher UIDispatcher;
        public static bool RandomDecide(int ChanceToOne) 
        {
           return random.Next(0, ChanceToOne) == 1;
        }
        public static void UpdateScore()
        {
            UIDispatcher.Invoke(new Action(() => {
                ScoreBlock.Text = $"Mines: {MinesLeft}/{MineAmount}\nIndicators: {IndicatorLeft}/{IndicatorAmount}";
            }));
        }
        public static Tile GetTile(int X, int Y)
        {
            int index = Y * GridSquare + X;
            if (index < TileGrid.Children.Count && index >= 0) return TileGrid.Children[index] as Tile;
            return null;
        }
        public static bool IsWithinGridBounds(int x, int y)
        {
            return x >= 0 && x < GridSquare && y >= 0 && y < GridSquare;
        }
        public static Tile GetRandomFreeCoordinate()
        {
            for(int i = 0; i < 100; i++)
            {
                int x = random.Next(0, GridSquare);
                int y = random.Next(0, GridSquare);
                Tile tile = GetTile(x, y);
                if (tile.IsMine || tile.IsRevealed) continue; 
                return tile;
            }
            return null;
        }
        public static List<Tile> GetAllNeighbours(Tile tile)
        {
            List<Tile> neighbourList = new List<Tile>();
            for (int row = tile.X - 1; row <= tile.X + 1; row++)
            {
                 for (int column = tile.Y - 1; column <= tile.Y + 1; column++)
                 {
                      if (tile.X == row && tile.Y == column || !IsWithinGridBounds(row, column)) continue;
                      Tile neighbourTile = GetTile(row, column);
                      if (neighbourTile != null) neighbourList.Add(neighbourTile);
                 }
            }
            return neighbourList;
        }
        public static void RevealNeighbourTilesWithChain(Tile tile)
        {
            foreach (Tile neighbourTile in GetAllNeighbours(tile))
            {
                if (neighbourTile.IsMine || neighbourTile.IsRevealed) continue;
                neighbourTile.RevealTile();
                if (neighbourTile.MinesInAreaValue == 0) RevealNeighbourTilesWithChain(neighbourTile);
            }
        }
    }
}
