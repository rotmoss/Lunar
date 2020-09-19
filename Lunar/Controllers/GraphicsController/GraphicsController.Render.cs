using System;
using System.Linq;
using OpenGL;

namespace Lunar
{
    public partial class GraphicsController
    {
        internal void RenderTexture(GraphicsObject graphicsObject)
        {
            if (!graphicsObject.Visible || graphicsObject.ShaderProgram == 0 || graphicsObject.VertexArrayObject == 0 || graphicsObject.SelectedTexture == 0) return;

            Gl.UseProgram(graphicsObject.ShaderProgram);
            Gl.Enable(EnableCap.Texture2d);
            Gl.BindVertexArray(graphicsObject.VertexArrayObject);
            Gl.BindTexture(TextureTarget.Texture2d, graphicsObject.SelectedTexture);

            Gl.DrawArrays(PrimitiveType.Quads, 0, 4);

            Gl.UseProgram(0);
            Gl.BindVertexArray(0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            Gl.Disable(EnableCap.Texture2d);
        }

        public void DrawQuad(bool fill, float x1, float y1, float x2, float y2, Color color = new Color())
        {
            PrimitiveType type = fill ? PrimitiveType.Quads : PrimitiveType.LineStrip;

            Gl.Color3(color.r, color.g, color.b);
            Matrix4x4f matrix = WindowController.Instance.Scaling;

            Gl.Begin(type);
            Gl.Vertex2((x1 * matrix.Row0.x) + (y1 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y1 * matrix.Row1.y));
            Gl.Vertex2((x1 * matrix.Row0.x) + (y2 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y2 * matrix.Row1.y));
            Gl.Vertex2((x2 * matrix.Row0.x) + (y2 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y2 * matrix.Row1.y));
            Gl.Vertex2((x2 * matrix.Row0.x) + (y1 * matrix.Row0.y), (x1 * matrix.Row1.x) + (y1 * matrix.Row1.y));
            Gl.End();
        }

        public void DrawQuad(bool fill, Transform t, Color color = new Color())
        {
            PrimitiveType type = fill ? PrimitiveType.Quads : PrimitiveType.LineStrip;

            Gl.Color3(color.r, color.g, color.b);
            Matrix4x4f matrix = WindowController.Instance.Scaling;

            Gl.Begin(type);
            Gl.Vertex2(((t.position.X - t.scale.X) * matrix.Row0.x) + ((t.position.Y - t.scale.Y) * matrix.Row0.y), ((t.position.X - t.scale.X) * matrix.Row1.x) + ((t.position.Y - t.scale.Y) * matrix.Row1.y));
            Gl.Vertex2(((t.position.X - t.scale.X) * matrix.Row0.x) + ((t.position.Y + t.scale.Y) * matrix.Row0.y), ((t.position.X - t.scale.X) * matrix.Row1.x) + ((t.position.Y + t.scale.Y) * matrix.Row1.y));
            Gl.Vertex2(((t.position.X + t.scale.X) * matrix.Row0.x) + ((t.position.Y + t.scale.Y) * matrix.Row0.y), ((t.position.X + t.scale.X) * matrix.Row1.x) + ((t.position.Y + t.scale.Y) * matrix.Row1.y));
            Gl.Vertex2(((t.position.X + t.scale.X) * matrix.Row0.x) + ((t.position.Y - t.scale.Y) * matrix.Row0.y), ((t.position.X + t.scale.X) * matrix.Row1.x) + ((t.position.Y - t.scale.Y) * matrix.Row1.y));
            Gl.End();
        }

        public void Render() { foreach (GraphicsObject x in _graphicsObjects.Values) { RenderTexture(x); } }
    }
}
