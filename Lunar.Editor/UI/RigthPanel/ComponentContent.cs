using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunar.Editor
{
    public abstract class ComponentContent : Grid
    {
        public ComponentPanel Parent;
        public ComponentContent() : base()
        {
            MainWindow.HierarchyView.OnSelectedItemChanged += UpdateValues;
        }

        public abstract void UpdateValues(object sender, SelectedItemEventArgs e);

        public void InsertElement(UIElement element, int colum = 0, int row = 0, int cSpan = 1, int rSpan = 1)
        {
            SetRow(element, row);
            SetColumn(element, colum);
            SetRowSpan(element, rSpan);
            SetColumnSpan(element, cSpan);

            Children.Add(element);
        }
    }
}
