using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar
{
    public struct Point
    {
        public float x;
        public float y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point a, Transform b)
        {
            a.x += b.position.X;
            a.y += b.position.Y;
            return a;
        }
        public static Point operator +(Transform a, Point b)
        {
            b.x += a.position.X;
            b.y += a.position.Y;
            return b;
        }

        private bool onSegment(Point p, Point q, Point r)
        {
            return q.x <= Math.Max(p.x, r.x) &&
                   q.x >= Math.Min(p.x, r.x) &&
                   q.y <= Math.Max(p.y, r.y) &&
                   q.y >= Math.Min(p.y, r.y);
        }

        private int orientation(Point p, Point q, Point r)
        {
            float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
            return val == 0 ? 0 : val > 0 ? 1 : 2;
        }

        private bool doIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            if (o1 != o2 && o3 != o4) return true;
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false;
        }

        public bool isInside(Point[] polygon)
        {
            if (polygon.Length < 3) return false;
            Point extreme = new Point(float.MaxValue, y);

            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % polygon.Length;

                if (doIntersect(polygon[i], polygon[next], this, extreme))
                {
                    if (orientation(polygon[i], this, polygon[next]) == 0)
                        return onSegment(polygon[i], this, polygon[next]);
                    count++;
                }
                i = next;
            } while (i != 0);

            return (count % 2 == 1);
        }
    }
}
