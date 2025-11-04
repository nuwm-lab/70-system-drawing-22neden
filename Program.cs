using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Labwork07
{
    public partial class Form1 : Form
    {
        private readonly List<PointF> _points = new();

        // Variant 10: y = cos^3(t^2) / (1.5 t + 2), t in [2.3, 7.2], step = 0.8
        private const double StartT = 2.3;
        private const double EndT = 7.2;
        private const double Step = 0.8;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true; // reduce flicker
            CalculatePoints();
            Resize += OnResize;
        }

        private void CalculatePoints()
        {
            _points.Clear();

            for (double t = StartT; t <= EndT + 1e-9; t += Step)
            {
                double y = Math.Pow(Math.Cos(t * t), 3) / (1.5 * t + 2);
                _points.Add(new PointF((float)t, (float)y));
            }
        }

        private void OnResize(object? sender, EventArgs e)
        {
            Invalidate(); // request repaint
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_points.Count < 2)
                return;

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            const float margin = 50f;
            float width = ClientSize.Width - 2 * margin;
            float height = ClientSize.Height - 2 * margin;
            if (width <= 0 || height <= 0)
                return;

            // compute ranges
            float minX = _points.Min(p => p.X);
            float maxX = _points.Max(p => p.X);
            float minY = _points.Min(p => p.Y);
            float maxY = _points.Max(p => p.Y);

            if (Math.Abs(maxX - minX) < 1e-6f) maxX = minX + 1f;
            if (Math.Abs(maxY - minY) < 1e-6f) maxY = minY + 1f;

            // === GRID BACKGROUND ===
            using (var gridPen = new Pen(Color.LightGray, 1))
            {
                gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                int gridLines = 10; // кількість клітинок

                for (int i = 0; i <= gridLines; i++)
                {
                    // vertical lines
                    float x = margin + i * (width / gridLines);
                    g.DrawLine(gridPen, x, margin, x, margin + height);

                    // horizontal lines
                    float y = margin + i * (height / gridLines);
                    g.DrawLine(gridPen, margin, y, margin + width, y);
                }
            }

            // === AXES ===
            using (var axisPen = new Pen(Color.Black, 2))
            {
                // X axis
                g.DrawLine(axisPen, margin, margin + height, margin + width, margin + height);
                // Y axis
                g.DrawLine(axisPen, margin, margin, margin, margin + height);
            }

            // === AXES LABELS ===
            using (var font = new Font("Segoe UI", 10F, FontStyle.Bold))
            {
                g.DrawString("X (t)", font, Brushes.Black, margin + width - 30, margin + height + 10);
                g.DrawString("Y (y)", font, Brushes.Black, margin - 35, margin - 25);
            }

            // Range info
            using (var font = new Font("Segoe UI", 9F))
            {
                g.DrawString($"t: [{minX:F2} .. {maxX:F2}]", font, Brushes.Black, margin, 4);
                g.DrawString($"y: [{minY:F4} .. {maxY:F4}]", font, Brushes.Black, margin + 200, 4);
            }

            // === GRAPH LINE ===
            using (var graphPen = new Pen(Color.FromArgb(200, 0, 80), 2))
            {
                PointF? prev = null;

                foreach (PointF p in _points)
                {
                    float x = margin + (p.X - minX) / (maxX - minX) * width;
                    float y = margin + height - (p.Y - minY) / (maxY - minY) * height;

                    if (prev is not null)
                    {
                        g.DrawLine(graphPen, prev.Value, new PointF(x, y));
                    }

                    prev = new PointF(x, y);
                }
            }

            // === POINTS ===
            using (var brush = new SolidBrush(Color.DarkBlue))
            {
                foreach (PointF p in _points)
                {
                    float x = margin + (p.X - minX) / (maxX - minX) * width;
                    float y = margin + height - (p.Y - minY) / (maxY - minY) * height;
                    g.FillEllipse(brush, x - 3, y - 3, 6, 6);
                }
            }
        }
    }
}
