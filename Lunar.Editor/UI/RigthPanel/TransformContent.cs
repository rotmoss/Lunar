using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunar.Editor
{
    public class TransformContent : ComponentContent
    {
        Field<float> Position;
        Field<float> Scale;
        Field<float> Rotation;

        public TransformContent()
        {       
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Pixel), MinWidth = 4 });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 64 });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Pixel), MinWidth = 4 });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 64 });

            Position = new Field<float>("Position", new Variable<float>("X"), new Variable<float>("Y"));
            Scale = new Field<float>("Scale", new Variable<float>("X"), new Variable<float>("Y"));
            Rotation = new Field<float>("Rotation", new Variable<float>("Z"));

            InsertField(Position);
            InsertField(Scale);
            InsertField(Rotation);
        }

        public void InsertField<T>(Field<T> field) where T : struct
        {
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            int row = RowDefinitions.Count - 1;

            InsertElement(field.Name, 0, row);

            int column = ColumnDefinitions.Count - field.Vars.Length * 2;

            for (int i = 0, j = 0; i < field.Vars.Length; i++, j += 2)
            {
                InsertElement(field.Vars[i].Name, column + j, row);
                InsertElement(field.Vars[i].Input, column + j + 1, row);
            }
        }

        public override void UpdateValues(object sender, SelectedItemEventArgs e)
        {
            if (e.isScene) { Parent.RemoveFromListView(); return; }
            Parent.AddToListView();

            Transform transform = Transform.GetGlobalTransform(e.Id);

            Position.Vars[0].Input.Text = transform.position.x.ToString();
            Position.Vars[1].Input.Text = transform.position.y.ToString();

            Scale.Vars[0].Input.Text = transform.scale.x.ToString();
            Scale.Vars[1].Input.Text = transform.scale.y.ToString();
        }
    }
}
