using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Lunar.IO;
using Lunar.Scenes;

namespace Lunar.Editor
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static XmlScene Scene;

        public static HierarchyView HierarchyView;
        public static DebugLogger DebugLogger;

        public bool Running = false;

        public MainWindow()
        {
            InitializeComponent();

            HierarchyView = new HierarchyView(LeftBox);
            DebugLogger = new DebugLogger(BottomBox);

            HierarchyView.OnSelectedItemChanged += OnSelectedGameobjectChanged;
            ContentRendered += OnRenderered;
            Run.Click += OnRun;
        }

        public void OnRenderered(object sender, EventArgs e)
        {
            LunarEngine.Init(new WindowInteropHelper(this).Handle, CenterBox);
            LunarEngine.Update();
            Scene = 

            HierarchyView.LoadGameObjects();

            CenterBox.SizeChanged += OnSizeChanged;
            Closing += OnWindowClose;

            while (true) 
                if (!Running) LunarEngine.Render();
                else LunarEngine.Update(); 
        }

        public void OnSelectedGameobjectChanged(object sender, SelectedItemChangedEventArgs e)
        {
            e.Name
        }

        public void OnRun(object sender, RoutedEventArgs e) => Running = !Running;
        public void OnWindowClose(object sender, EventArgs eventArgs) => LunarEngine.Close();

        public void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rect rect = CenterBox.GetAbsolutePlacement(false);
            LunarEngine.UpdateWindow((int)rect.X, (int)rect.Y, (int)e.NewSize.Width, (int)e.NewSize.Height);
        }
    }
}
