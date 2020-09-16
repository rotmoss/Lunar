using System;

namespace Lunar
{
    public partial class Script
    {
        public void CreateSprite(string texture, string vertexShader, string fragmentShader, out int w, out int h) => GraphicsController.Instance.CreateSprite(Id, texture, vertexShader, fragmentShader, out w, out h);
        public void CreateText(string font, string message, int size, Color color, uint wrapper, string vertexShader, string fragmentShader, out int w, out int h) => GraphicsController.Instance.CreateText(Id, font, message, size, color, wrapper, vertexShader, fragmentShader, out w, out h);
        public void SetUniform<T>(T data, string uniformName) where T : struct => GraphicsController.Instance.SetUniform(Id, data, uniformName);
        public void SetUniform<T>(uint id, T data, string uniformName) where T : struct => GraphicsController.Instance.SetUniform(id, data, uniformName);
        public void ForeachShader(Action<uint> action) => GraphicsController.Instance.ForeachShader(action);
    }
}
