using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace RG___Voronoi
{
    public class BowyerWatson
    {
        private int width;
        private int height;
        private int numberOfPoints = 100;

        private Vector3 upperBounds = new Vector3(1000, 700, 0);
        private Vector3 lowerBounds = new Vector3(50, 50, 0);

        public List<Triangle.Point> pointList = new List<Triangle.Point>();
        private List<Triangle> triangulation = new List<Triangle>();

        public BowyerWatson(int width, int height)
        {
            this.upperBounds = new Vector3(width - 100, height - 100, 0);
            this.width = width;
            this.height = height;
        }

        public List<Triangle> BW_method()
        {
            var rand = new Random();
            for (var i = 0; i < numberOfPoints; i++)
            {
                var newPoint = new Triangle.Point(
                    rand.NextDouble() * (upperBounds.X - lowerBounds.X) + lowerBounds.X,
                    rand.NextDouble() * (upperBounds.Y - lowerBounds.Y) + lowerBounds.Y, 0);
                pointList.Add(newPoint);
            }

//        ADD POINTS MANUALLY
//        pointList = new List<Triangle.Point>
//        {
//            new Triangle.Point(100,200,0),
//            new Triangle.Point(200,50,0),
//            new Triangle.Point(300,200,0),
//            new Triangle.Point(200,158.3),
//            new Triangle.Point(200, 170,0),
//            new Triangle.Point(250, 175,0),
//            new Triangle.Point(230, 150,0),
//            new Triangle.Point(260, 250,0),
//
//        };
//            var superTriangle1 = new Triangle(new Triangle.Point(0, 0, 0), new Triangle.Point(1500, 0, 0),
//                new Triangle.Point(0, 800, 0));
//            var superTriangle2 = new Triangle(new Triangle.Point(0, 800, 0), new Triangle.Point(1500, 0, 0),
//                new Triangle.Point(1500, 800, 0));

            var superTriangle1 = new Triangle(new Triangle.Point(-500, 0, 0),
                new Triangle.Point(0, height + 5000, 0), new Triangle.Point(width + 5000, 0, 0));
            triangulation.Add(superTriangle1);

            foreach (var point in pointList)
            {
                var badTriangles = new HashSet<Triangle>();
                foreach (var triangle in triangulation)
                    if (triangle.IsInsideCircumcircle2D(point))
                        badTriangles.Add(triangle);


                var edges = new HashSet<Triangle.Edge>();

                foreach (var triangle1 in badTriangles)
                foreach (var triangleEdge in triangle1.Edges)
                {
                    var shared = false;
                    foreach (var triangle2 in badTriangles)
                        if (!triangle1.Equals(triangle2))
                        {
                            shared = triangle2.hasEdge(triangleEdge);
                            if (shared) break;
                        }

                    if (!shared) edges.Add(triangleEdge);
                }

                foreach (var triangle in badTriangles) triangulation.Remove(triangle);

                foreach (var edge in edges)
                {
                    var newTriangle = new Triangle(edge.start, edge.end, point);
                    if (!triangulation.Contains(newTriangle)) triangulation.Add(newTriangle);
                }
            }

            //REMOVE STARTING TRIANGLES
//            for (int i = triangulation.Count - 1; i >= 0; i--)
//            {
//                if (triangulation[i].SharesVertexWith(superTriangle1))
////                    || triangulation[i].SharesVertexWith(superTriangle2))
//                {
//                    triangulation.RemoveAt(i);
//                }
//            }

            return triangulation;
        }
    }
}