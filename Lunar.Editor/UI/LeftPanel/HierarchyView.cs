using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Lunar.Scenes;
using System.Windows;
using OpenGL;

namespace Lunar.Editor
{
    public class SelectedItemChangedEventArgs
    {
        public uint Id;
        public string Name;
        public bool isScene;
    }

    public class HierarchyView
    {
        TreeView _parent;
        public EventHandler<SelectedItemChangedEventArgs> OnSelectedItemChanged;
        public HierarchyView(TreeView parent)
        {
            _parent = parent;
            _parent.SelectedItemChanged += SelectedItemChanged;
        }

        private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = (TreeViewItem)_parent.SelectedItem;
            TreeViewItem temp = (TreeViewItem)_parent.SelectedItem;

            while (Scene.GetScene((string)temp.Header) == null)
                temp = (TreeViewItem)temp.Parent;

            Scene scene = Scene.GetScene((string)temp.Header);

            OnSelectedItemChanged.Invoke(this, new SelectedItemChangedEventArgs { Id = scene.GetGameObjectId((string)selectedItem.Header), Name = (string)selectedItem.Header, isScene = selectedItem.Header == temp.Header });
        }

        public void LoadGameObjects()
        {
            List<TreeViewItem> scenes = new List<TreeViewItem>();

            foreach (Scene scene in Scene.Scenes.Values) 
            {
                TreeViewItem sceneItem = new TreeViewItem();
                sceneItem.Header = scene.Name;
                scenes.Add(sceneItem);

                foreach (uint id in scene.GameObjects)
                    if(scene.GetParent(id) == 0)
                        sceneItem.Items.Add(GetItemFromId(scene, id));

                foreach (TreeViewItem gameObject in sceneItem.Items)
                    AddChildren(scene, gameObject);
            }

            foreach (TreeViewItem item in scenes)
                _parent.Items.Add(item);
        }

        public void AddChildren(Scene scene, TreeViewItem parent)
        {
            foreach (uint id in scene.GetChildren((string)parent.Header))
            {
                TreeViewItem child = GetItemFromId(scene, id);
                parent.Items.Add(child);
                AddChildren(scene, child);
            }
        }

        public TreeViewItem GetItemFromId(Scene scene, uint id)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = scene.GetGameObjectName(id);
            return item;
        }
    }
}
