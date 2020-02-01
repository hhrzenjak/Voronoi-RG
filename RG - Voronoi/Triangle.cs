using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RG___Voronoi
{
    public class Triangle
    {
        public Point[] Vertices { get; } = new Point[3];
        public Edge[] Edges { get; } = new Edge[3];
        private Circle circumcircle;

        public Triangle(Point a, Point b, Point c)
        {
            Vertices = new Point[] {a, b, c};
            Edges = new[] {new Edge(a, b), new Edge(b, c), new Edge(c, a)};
            CalculateCircumcircle();
        }

        public bool IsInsideCircumcircle2D(Point point)
        {
            return Point.GetDistance(point, circumcircle.center) < circumcircle.radius;
        }

        public Point Circumcenter()
        {
            Point A = Vertices[0];
            Point B = Vertices[1];
            Point C = Vertices[2];

            if (!ccw())
            {
                C = Vertices[0];
                A = Vertices[2];
            }

            double D = 2 * (A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y));
            double Ak = Math.Pow(A.x, 2) + Math.Pow(A.y, 2);
            double Bk = Math.Pow(B.x, 2) + Math.Pow(B.y, 2);
            double Ck = Math.Pow(C.x, 2) + Math.Pow(C.y, 2);

            double Ux = 1 / D * (Ak * (B.y - C.y) + Bk * (C.y - A.y) + Ck * (A.y - B.y));
            double Uy = 1 / D * (Ak * (C.x - B.x) + Bk * (A.x - C.x) + Ck * (B.x - A.x));


            Point circumcenter = new Point(Ux, Uy);
            return circumcenter;
        }

        private void CalculateCircumcircle()
        {
            Point A = Vertices[0];
            Point circumcenter = Circumcenter();
            double radius = Point.GetDistance(circumcenter, A);
            circumcircle = new Circle(circumcenter, radius);
        }

        public bool ccw()
        {
            Point A = Vertices[0];
            Point B = Vertices[1];
            Point C = Vertices[2];

            return (B.x - A.x) * (C.y - A.y) - (C.x - A.x) * (B.y - A.y) > 0;
        }

        public bool SharesEdgeWith(Triangle triangle)
        {
            var sharedEdges = Edges.Where(t => triangle.Edges.Contains(t));
            return sharedEdges.Count() > 0;
        }

        public bool SharesVertexWith(Triangle triangle)
        {
            var sharedVert = Vertices.Where(t => triangle.Vertices.Contains(t));
            return sharedVert.Count() > 0;
        }

        public List<Edge> NonSharedEdges(Triangle triangle)
        {
            var nonSharedEdges = Edges.Where(t => !triangle.Edges.Contains(t));
            return nonSharedEdges.ToList();
        }

        public List<Edge> SharedEdges(Triangle triangle)
        {
            var sharedEdges = Edges.Where(t => triangle.Edges.Contains(t));
            return sharedEdges.ToList();
        }

        public bool hasEdge(Edge edge)
        {
            //return Edges[0].Equals(edge) || Edges[1].Equals(edge) || Edges[2].Equals(edge);
            return Edges.Contains(edge);
        }

        public bool hasVertex(Point point)
        {
            //return Edges[0].Equals(edge) || Edges[1].Equals(edge) || Edges[2].Equals(edge);
            return Vertices.Contains(point);
        }


        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return Equals((Triangle) other);
        }

        public override int GetHashCode()
        {
            return (Edges != null ? Edges.GetHashCode() : 0);
        }

        protected bool Equals(Triangle other)
        {
            return Equals(Edges, other.Edges);
        }

        public static bool operator ==(Triangle left, Triangle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Triangle left, Triangle right)
        {
            return !Equals(left, right);
        }


        public class Edge
        {
            public Point start { get; }
            public Point end { get; }
            public List<Triangle> sharedTriangles = new List<Triangle>();

            public Edge(Point x, Point y)
            {
                start = x;
                end = y;
            }

            public override bool Equals(object other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (other.GetType() != this.GetType()) return false;
                return Equals((Edge) other);
            }

            protected bool Equals(Edge other)
            {
                return Equals(start, other.start) && Equals(end, other.end) ||
                       Equals(start, other.end) && Equals(end, other.start);
            }

            public static bool operator ==(Edge left, Edge right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Edge left, Edge right)
            {
                return !Equals(left, right);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((start != null ? start.GetHashCode() : 0) * 397) ^
                           ((end != null ? end.GetHashCode() : 0) * 397);
                }
            }

            public override string ToString()
            {
                return "(" + start + "," + end + ")";
            }
        }

        public class Point
        {
            public double x { get; }
            public double y { get; }
            public double z { get; }


            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
                z = 0;
            }

            public Point(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public Point(Vector3 point)
            {
                x = point.X;
                y = point.Y;
                z = point.Z;
            }

            public static double GetDistance(Point A, Point B)
            {
                return Math.Sqrt(Math.Pow((B.x - A.x), 2) + Math.Pow((B.y - A.y), 2));
            }

            public override bool Equals(object other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (other.GetType() != this.GetType()) return false;
                return Equals((Point) other);
            }

            protected bool Equals(Point other)
            {
                return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = x.GetHashCode();
                    hashCode = (hashCode * 397) ^ y.GetHashCode();
                    hashCode = (hashCode * 397) ^ z.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(Point left, Point right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Point left, Point right)
            {
                return !Equals(left, right);
            }

            public override string ToString()
            {
                return "(" + x + " ," + y + " ," + z + ")";
            }
        }
    }
}