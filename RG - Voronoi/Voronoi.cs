using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.VisualStyles;

namespace RG___Voronoi
{
    public class Voronoi
    {
        public IEnumerable<Triangle.Edge> GenerateEdgesFromDelaunay(List<Triangle> triangulation)
        {
            var voronoiEdges = new List<Triangle.Edge>();

            foreach (Triangle triangle1 in triangulation)
            {
                foreach (Triangle triangle2 in triangulation)
                {
                    if (!triangle1.Equals(triangle2))
                    {
                        if (triangle1.SharesEdgeWith(triangle2))
                        {
                            var edge = new Triangle.Edge(triangle1.Circumcenter(), triangle2.Circumcenter());
                            voronoiEdges.Add(edge);
                        }
                    }
                }
            }


            return voronoiEdges;
        }

        public IEnumerable<VoroniCell> GetVoronoiCells(List<Triangle.Point> points, List<Triangle> triangulation)
        {
            List<VoroniCell> voronoiCells = new List<VoroniCell>();

            foreach (Triangle.Point point in points)
            {
                HashSet<Triangle> tri = new HashSet<Triangle>();
                foreach (Triangle triangle in triangulation)
                {
                    if (triangle.hasVertex(point))
                    {
                        tri.Add(triangle);
                    }
                }

                HashSet<Triangle.Point> pointsCell = new HashSet<Triangle.Point>();

                VoroniCell currentCell = new VoroniCell(point);
                foreach (Triangle triangle1 in tri)
                {
                    foreach (Triangle triangle2 in tri)
                    {
                        if (!triangle1.Equals(triangle2))
                        {
                            if (triangle1.SharesEdgeWith(triangle2))
                            {
                                pointsCell.Add(triangle1.Circumcenter());
                                pointsCell.Add(triangle2.Circumcenter());
                            }
                        }
                    }
                }

                currentCell.Points = pointsCell.ToList();
                currentCell.SortPoints();
                voronoiCells.Add(currentCell);
            }

            return voronoiCells;
        }
    }


    public class VoroniCell
    {
        private Triangle.Point center;
        public List<Triangle.Point> Points = new List<Triangle.Point>();

        public VoroniCell(Triangle.Point center)
        {
            this.center = center;
        }

        public void SortPoints()
        {
            List<Tuple<Triangle.Point, double>> sortedPoints = new List<Tuple<Triangle.Point, double>>();
            foreach (Triangle.Point point in Points)
            {
                double angle = Math.Atan2(point.y - center.y, point.x - center.x);
                sortedPoints.Add(new Tuple<Triangle.Point, double>(point, angle));
            }

            sortedPoints.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            Points = new List<Triangle.Point>();
            for (int i = 0; i < sortedPoints.Count; i++)
            {
                Points.Add(sortedPoints[i].Item1);
            }
        }
    }
}