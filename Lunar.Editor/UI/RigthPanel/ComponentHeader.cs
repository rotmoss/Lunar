using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunar.Editor
{
    public class ComponentHeader : Grid
    {
        public Button Button;
        public TextBlock Name;
        public ComponentHeader(string name) : base()
        {
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24, GridUnitType.Pixel) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });

            Button = new Button { Margin = new Thickness(4, 4, 4, 4), Width = 16, Height = 16, HorizontalAlignment = HorizontalAlignment.Left };
            Name = new TextBlock { Text = name, Margin = new Thickness(0, -3, 0, 0), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, FontWeight = FontWeights.SemiBold, FontSize = 16 };

            InsertElement(Button, 0, 0);
            InsertElement(Name, 1, 0);
        }

        public void InsertElement(UIElement element, int colum = 0, int row = 0, int cSpan = 1, int rSpan = 1)
        {
            SetColumn(element, colum);
            SetRow(element, row);
            SetColumnSpan(element, cSpan);
            SetRowSpan(element, rSpan);

            Children.Add(element);
        }
    }
}
