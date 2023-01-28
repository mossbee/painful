using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Painful
{
    public partial class Form1 : Form
    {
        private IDrawingTool currentTool;
        private List<IDrawingTool> tools;
        private Bitmap buffer;
        private Graphics gBuffer;
        public Color colour;

        public Form1()
        {
            InitializeComponent();
            this.Width = 800;
            this.Height = 800;

            buffer = new Bitmap(picDrawingSurface.Width, picDrawingSurface.Height);
            gBuffer = Graphics.FromImage(buffer);

       

            tools = new List<IDrawingTool>();

            colour = Color.Black;

            SetCurrentTool(new Pencil(colour, 2));
        }

        private void SetCurrentTool(IDrawingTool tool)
        {
            currentTool = tool;
        }

        private void picDrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            currentTool.OnMouseDown(e.Location);
        }

        private void picDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                currentTool.OnMouseMove(e.Location);
                currentTool.Draw(gBuffer);
                picDrawingSurface.Invalidate();
            }
        }

        private void picDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {
            currentTool.OnMouseUp(e.Location);
            tools.Add(currentTool);
            currentTool = currentTool.Clone();
        }

        private void picDrawingSurface_Paint(object sender, PaintEventArgs e)
        {
            gBuffer.Clear(Color.White);
            foreach (IDrawingTool tool in tools)
            {
                tool.Draw(gBuffer);
            }
            currentTool.Draw(gBuffer);
            e.Graphics.DrawImageUnscaled(buffer, Point.Empty);
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            SetCurrentTool(new Pencil(colour, 2));
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            SetCurrentTool(new Rectangle(colour, 2));
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            SetCurrentTool(new Eraser(30));
        }


        private void btnColor_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colour = colorDialog1.Color;
            }
            SetCurrentTool(new Pencil(colour, 2));
        }
    }
    public interface IDrawingTool
    {
        IDrawingTool Clone();
        void OnMouseDown(Point location);
        void OnMouseMove(Point location);
        void OnMouseUp(Point location);
        void Draw(Graphics g);
    }
    public class Pencil : IDrawingTool
    {
        private List<Point> points;
        private Pen pen;

        public Pencil(Color color, int width)
        {
            points = new List<Point>();
            pen = new Pen(color, width);
        }
        /// <summary>
        /// Lưu quá trình vẽ bằng danh sách điểm mỗi khi mousemove và mousedown
        /// Để vẽ lại vào gBuffer 
        /// </summary>
        /// <returns></returns>
        public IDrawingTool Clone()
        {
            Pencil pencil = new Pencil(pen.Color, (int)pen.Width);
            pencil.points = new List<Point>(points);
            return pencil;
        }

        public void OnMouseDown(Point location)
        {
            points.Clear();
            points.Add(location);
        }

        public void OnMouseMove(Point location)
        {
            points.Add(location);
        }

        public void OnMouseUp(Point location)
        {
            points.Add(location);
        }

        public void Draw(Graphics g)
        {
            for (int i = 1; i < points.Count; i++)
            {
                g.DrawLine(pen, points[i - 1], points[i]);
            }
        }
    }
    public class Rectangle : IDrawingTool
    {
        private Point startLocation;
        private Point endLocation;
        private Pen pen;
        private Brush brush;

        public Rectangle(Color colour, int width)
        {
            pen = new Pen(colour, width);
            brush = new SolidBrush(colour);
        }

        public IDrawingTool Clone()
        {
            return new Rectangle(pen.Color, (int)pen.Width);
        }
        public void OnMouseDown(Point location)
        {
            startLocation = location;
            endLocation = location;
        }

        public void OnMouseMove(Point location)
        {
            endLocation = location;
        }

        public void OnMouseUp(Point location)
        {
            endLocation = location;
        }

        public void Draw(Graphics g)
        {
            int x = Math.Min(startLocation.X, endLocation.X);
            int y = Math.Min(startLocation.Y, endLocation.Y);
            int width = Math.Abs(startLocation.X - endLocation.X);
            int height = Math.Abs(startLocation.Y - endLocation.Y);
            g.DrawRectangle(pen, x, y, width, height);
        }
    }
    public class Eraser : Pencil
    {
        public Eraser(int width) : base(Color.White, width)
        {
        }
    }
}
