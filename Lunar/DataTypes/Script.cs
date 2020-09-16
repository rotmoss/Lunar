namespace Lunar
{
    public abstract partial class Script
    {
        public uint Id { get; internal set; }
        public string Name { get; internal set; }

        public float DeltaTime { get => Time.DeltaTime; }
        public bool DrawColliders { get => Lunar.DrawColliders; set => Lunar.DrawColliders = value; }
        public bool Visible { get => SceneController.Instance.GetEntityVisibility(Id); set => SceneController.Instance.SetEntityVisibility(Id, value); }
        virtual public void Init() { }
        virtual public void Update() { }
        virtual public void LateUpdate() { }
        virtual public void PostRender() { }
    }
}
