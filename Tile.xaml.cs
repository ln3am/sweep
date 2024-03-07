using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace sweep
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        public int X;
        public int Y;
        public bool IsMine = false;
        public bool IsFlag = false;
        public bool IsRevealed = false;
        public int MinesInAreaValue;
        public Action<bool, int, int> IsClicked;
        public Tile()
        {
            InitializeComponent();
        }
        public void AddValue() 
        {
            MinesInAreaValue++;
            FieldValue.Text = MinesInAreaValue.ToString(); 
        }
        public void OnRightClick(object sender, RoutedEventArgs e)
        {
            SetFlag();
        }
        public void OnLeftClick(object sender, RoutedEventArgs e)
        {
            if (IsFlag) return;
            RevealTile();
            IsClicked?.Invoke(IsMine, X, Y);
        }
        public void ActivateIndicator() => Indicator.Visibility = Visibility.Visible;
        public void DeactivateIndicator() => Indicator.Visibility = Visibility.Collapsed;
        public void RevealTile()
        {
            if (IsRevealed || IsFlag) return;
            IsRevealed = true;
            DefaultTile.Fill = new SolidColorBrush(Colors.LightGray);
            if (IsMine) Mine.Visibility = Visibility.Visible;
            else FieldValue.Visibility = Visibility.Visible;
        }
        public void SetFlag()
        {
            if (IsRevealed) return;
            if (IsFlag)
            {
                IsFlag = false;
                Flag.Visibility = Visibility.Collapsed;
                PlayingField.MinesLeft++;
            }
            else
            {
                IsFlag = true;
                Flag.Visibility = Visibility.Visible;
                PlayingField.MinesLeft--;
            }
            PlayingField.UpdateScore();
        }
    }
}
