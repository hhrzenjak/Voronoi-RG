namespace RG___Voronoi
{
    public class Circle
    {
        public Triangle.Point center;
        public double radius;

        public Circle(Triangle.Point center, double radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }
}