using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RG___Voronoi
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MyForm myform = new MyForm();

            myform.Text = "Main Window";
            myform.Size = new Size(1280, 720);
            myform.FormBorderStyle = FormBorderStyle.FixedDialog;
            myform.StartPosition = FormStartPosition.CenterScreen;
            myform.MaximizeBox = true;
            //myform.WindowState = FormWindowState.Maximized;

            BowyerWatson bw = new BowyerWatson(myform.Width, myform.Height);


            List<Triangle> triangulation = bw.BW_method();
            myform.randomPoints = bw.pointList;
            myform.triangles = triangulation;

            Voronoi v = new Voronoi();
            IEnumerable<Triangle.Edge> realEdges = v.GenerateEdgesFromDelaunay(triangulation);
            myform.edges = realEdges.ToList();
            myform.voronoiCells = v.GetVoronoiCells(bw.pointList, triangulation).ToList();

            Graphics g = myform.CreateGraphics();
            myform.ShowDialog();

            Triangle.Edge e1 = new Triangle.Edge(new Triangle.Point(1, 1), new Triangle.Point(2, 2));
            Triangle.Edge e2 = new Triangle.Edge(new Triangle.Point(2, 2), new Triangle.Point(1, 1));
            HashSet<Triangle.Edge> e = new HashSet<Triangle.Edge>();
            e.Add(e2);
            e.Add(e1);
        }
    }

    public class MyForm : Form
    {
        public List<Triangle> triangles;
        public List<Triangle.Point> randomPoints;
        public List<Triangle.Edge> edges;
        public List<VoroniCell> voronoiCells;

        Random rnd = new Random();

        private ToolStripContainer toolStripContainer1;
        ToolBar toolBar1 = new ToolBar();
        private int buttonClickedIndex;

        public MyForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            toolBar1 = new ToolBar();
            ToolBarButton toolBarButton1 = new ToolBarButton();
            ToolBarButton toolBarButton2 = new ToolBarButton();
            ToolBarButton toolBarButton3 = new ToolBarButton();
            ToolBarButton toolBarButton4 = new ToolBarButton();
            // Set the Text properties of the ToolBarButton controls.
            toolBarButton1.Text = "Delaunay triangulation";
            toolBarButton2.Text = "Delaunay triangulation with Voronoi outline";
            toolBarButton3.Text = "Voronoi diagram";
            toolBarButton4.Text = "Voronoi diagram with outline";

            // Add the ToolBarButton controls to the ToolBar.
            toolBar1.Buttons.Add(toolBarButton1);
            toolBar1.Buttons.Add(toolBarButton2);
            toolBar1.Buttons.Add(toolBarButton3);
            toolBar1.Buttons.Add(toolBarButton4);

            toolBar1.ButtonClick += this.toolBar1_ButtonClick;

            // Add the ToolBar to the Form.
            Controls.Add(toolBar1);
        }

        private void toolBar1_ButtonClick(
            Object sender,
            ToolBarButtonClickEventArgs e)
        {
            this.Invalidate();
            buttonClickedIndex = toolBar1.Buttons.IndexOf(e.Button);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen blackPen = new Pen(Color.Black, 3);
            Brush blackBrush = new SolidBrush(Color.Black);

            Graphics g = e.Graphics;

            if (buttonClickedIndex.Equals(0) || buttonClickedIndex.Equals(1))
            {
                //DELUNAY TRIANGULATION
                foreach (var triangle in triangles)
                {
                    Color randomColor = Color.FromArgb(rnd.Next(250), rnd.Next(250), rnd.Next(250));
                    Brush brush = new SolidBrush(randomColor);
                    PointF point1 = new PointF((float) triangle.Vertices[0].x, (float) triangle.Vertices[0].y);
                    PointF point2 = new PointF((float) triangle.Vertices[1].x, (float) triangle.Vertices[1].y);
                    PointF point3 = new PointF((float) triangle.Vertices[2].x, (float) triangle.Vertices[2].y);
                    PointF[] points =
                    {
                        point1,
                        point2,
                        point3
                    };
                    g.FillPolygon(brush, points);
                }
            }
            else if (buttonClickedIndex.Equals(2) || buttonClickedIndex.Equals(3))
            {
                //VORONOI COLOURED
                foreach (VoroniCell cell in voronoiCells)
                {
                    List<Triangle.Point> polygonPoints = cell.Points;

                    PointF[] arrayResult = polygonPoints.Select(p => new PointF((float) p.x, (float) p.y)).ToArray();
                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    Brush brush = new SolidBrush(randomColor);
                    if (arrayResult.Length > 0)
                    {
                        e.Graphics.FillPolygon(brush, arrayResult);
                    }
                }
            }

            if (buttonClickedIndex.Equals(1) || buttonClickedIndex.Equals(3))
            {
                //VORONOI CELLS OUTLINE
                foreach (var edge in edges)
                {
                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    g.DrawLine(blackPen, (float) edge.start.x, (float) edge.start.y, (float) edge.end.x,
                        (float) edge.end.y);
                }
            }


            // DRAW POINTS
            foreach (var point in randomPoints)
            {
                e.Graphics.FillRectangle(blackBrush, (float) point.x, (float) point.y, 4f, 4f);
            }
        }
    }
}