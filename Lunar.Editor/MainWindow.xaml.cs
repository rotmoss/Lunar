using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Lunar.Editor
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static ComponentPanel TransformPanel;
        public static ComponentPanel SpritePanel;

        public static HierarchyView HierarchyView;

        public static DebugLogger DebugLogger;

        public bool Running = false;
        public MainWindow()
        {
            InitializeComponent();

            InitLeftBox();
            InitRightBox();
            InitBottomBox();

            ContentRendered += OnRenderered;
            Run.Click += OnRun;
        }

        public void InitRightBox()
        {
            TransformPanel = new ComponentPanel(RightBox, new ComponentHeader("Transform"), new TransformContent());
            SpritePanel = new ComponentPanel(RightBox, new ComponentHeader("Sprite"), new SpriteContent());

            TransformPanel.AddToListView();
            SpritePanel.AddToListView();
        }

        public void InitLeftBox()
        {
            HierarchyView = new HierarchyView(LeftBox);
        }

        public void InitBottomBox()
        {
            DebugLogger = new DebugLogger(BottomBox);
        }

        public void OnRenderered(object sender, EventArgs e)
        {
            LunarEngine.Init(new WindowInteropHelper(this).Handle, CenterBox);
            LunarEngine.Update();

            HierarchyView.LoadGameObjects();

            CenterBox.SizeChanged += OnSizeChanged;
            Closing += OnWindowClose;

            while (true) 
                if (!Running) LunarEngine.Render();
                else LunarEngine.Update(); 
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
