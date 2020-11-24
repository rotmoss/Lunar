using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Lunar.Editor
{
    public class SpriteContent : ComponentContent
    {
        public SpriteContent()
        {
            //InsertField(new Field("Texture", new Variable("File", typeof(string))));
            //InsertField(new Field("Shader", new Variable("Vertex shader", typeof(string)), new Variable("Fragment shader", typeof(float))));
        }

        public override void UpdateValues(object sender, SelectedItemChangedEventArgs e)
        {
        }
    }
}
