using System;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Collections.Generic;

namespace sweep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random;
        private bool ClickedAfterIndicator;
        private Tile IndicatorTile;
        public List<(int, int)> MineCoordinates;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnLoad(object sender, EventArgs e)
        {
            ResetField();
        }
        private void OnIndicatorClick(object sender, EventArgs e)
        {
            if (!ClickedAfterIndicator || PlayingField.IndicatorLeft == 0) return;
            PlayingField.IndicatorLeft--;
            PlayingField.UpdateScore();
            IndicatorTile = AddIndicatorClick();
        }
        private void SetUpPlayingField()
        {
            random = new Random();
            LocalCreateTileGrid();
            PlayingField.MinesLeft = PlayingField.MineAmount;
            PlayingField.IndicatorLeft = PlayingField.IndicatorAmount;
            PlayingField.TileGrid = TileGrid;
            PlayingField.UIDispatcher = Dispatcher;
            LocalSetMines();
            LocalCalculateMinesInArea();
            IndicatorTile = AddIndicatorClick();
            PlayingField.ScoreBlock = ScoreBlock;
            PlayingField.UpdateScore();

            void LocalCreateTileGrid()
            {
                for (int i = 0; i < PlayingField.GridSquare; i++)
                {
                    TileGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                    TileGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                }
                for (int row = 0; row < PlayingField.GridSquare; row++)
                {
                    for (int col = 0; col < PlayingField.GridSquare; col++)
                    {
                        Tile tile = new Tile();
                        tile.X = col;
                        tile.Y = row;
                        Grid.SetRow(tile, row);
                        Grid.SetColumn(tile, col);
                        TileGrid.Children.Add(tile);
                        tile.IsClicked += TileClicked;
                    }
                }
            }
            void LocalCalculateMinesInArea()
            {
                foreach (Tile tile in TileGrid.Children)
                {
                    foreach (Tile neighbourTile in PlayingField.GetAllNeighbours(tile))
                    {
                        if (neighbourTile.IsMine) tile.AddValue();
                    }
                }
            }
            void LocalSetMines()
            {
                MineCoordinates = new List<(int, int)>();
                for (int i = 0; i <= PlayingField.MineAmount; i++)
                {
                    Tile mineTile = PlayingField.GetRandomFreeCoordinate();
                    mineTile.IsMine = true;
                    MineCoordinates.Add((mineTile.X, mineTile.Y));
                }
            }
        }
        private Tile AddIndicatorClick()
        {
            ClickedAfterIndicator = false;
            for (int i = 0; i < 100; i++)
            {
                Tile tile = PlayingField.GetRandomFreeCoordinate();
                if (tile.MinesInAreaValue != 0 || !LocalNeighbourTilesHaveEmptySpot(tile)) continue;
                tile.ActivateIndicator();
                return tile;
            }
            return null;

            bool LocalNeighbourTilesHaveEmptySpot(Tile tile)
            {
                foreach (Tile neighbourTile in PlayingField.GetAllNeighbours(tile))
                {
                    if (neighbourTile.MinesInAreaValue == 0) return true;
                }
                return false;
            }
        }
        private void TileClicked(bool IsMine, int X, int Y)
        {
            Tile tile = PlayingField.GetTile(X, Y);
            if (!ClickedAfterIndicator && IndicatorTile != null)
            {
                IndicatorTile.DeactivateIndicator();
                ClickedAfterIndicator = true;
            }
            if (IsMine)
            {
                MessageBox.Show("Game over");
                ResetField();
                return;
            }
            if (tile.MinesInAreaValue == 0) PlayingField.RevealNeighbourTilesWithChain(tile);
        }
        private void ResetField()
        {
            TileGrid.Children.Clear();
            TileGrid.RowDefinitions.Clear();
            TileGrid.ColumnDefinitions.Clear();
            SetUpPlayingField();
        }
    }
}
