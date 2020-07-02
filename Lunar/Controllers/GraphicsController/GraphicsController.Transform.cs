using System.Collections.Generic;

namespace Lunar
{
    partial class GraphicsController
    {
        private Dictionary<uint, Transform> _graphicsTransform;

        public void AddTransform(uint id)
        {
            if (!_graphicsTransform.ContainsKey(id)) _graphicsTransform.Add(id, new Transform(0, 0, 1, 1));
        }

        public void Translate(uint id, Transform transform)
        {
            if (_graphicsTransform.ContainsKey(id)) _graphicsTransform[id] += transform;
        }
    }
}
