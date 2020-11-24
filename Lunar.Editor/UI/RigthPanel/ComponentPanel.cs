using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lunar.Editor
{
    public class ComponentPanel 
    {
        ListView _parent;
        ComponentHeader _header;
        ComponentContent _content;

        public ComponentPanel(ListView parent, ComponentHeader header, ComponentContent content)
        {
            _parent = parent;
            _header = header;
            _content = content;
            _header.Button.Click += HideContent;
            _content.Parent = this;
        }

        public void AddToListView()
        {
            if (!_parent.Items.Contains(_header)) _parent.Items.Add(_header);
            if (!_parent.Items.Contains(_content)) _parent.Items.Add(_content);
        }

        public void RemoveFromListView()
        {
            if (_parent.Items.Contains(_header)) _parent.Items.Remove(_header);
            if (_parent.Items.Contains(_content)) _parent.Items.Remove(_content);
        }

        public void HideContent(object sender, RoutedEventArgs e)
        {
            _content.Visibility = _header.Visibility == Visibility.Visible ? _content.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible : Visibility.Collapsed;
        }
    }
}
