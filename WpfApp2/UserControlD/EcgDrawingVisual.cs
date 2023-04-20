using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp2.UserControlD
{
    public class EcgDrawingVisual : FrameworkElement
    {
        private readonly List<Visual> visuals = new List<Visual>();
        private DrawingVisual Layer;

        private Pen ecg_pen = new Pen(Brushes.Orange, 1.5);
        private Pen primarygrid_pen = new Pen(Brushes.Black, 1);
        private Pen secondgrid_pen = new Pen(Brushes.Gray, 1);

        private int?[] ecg_points = new int?[2000];//曲线数据

        private int currentStart = 0;

        private double x_offset = 0;//滚动条偏移值
        private double y_offset = 0;
        private double y_scale;//y轴方向缩放比例

        private int Top_ecg_max = 100;
        private int Top_ecg_min = -100;
        private int X_Sex = 20;//X轴分度值
        private int Y_Sex = 20;
        private int Bottom = 30;//底部X轴坐标显示高度

        public EcgDrawingVisual()
        {
            ecg_pen.Freeze();
            primarygrid_pen.Freeze();
            secondgrid_pen.Freeze();

            Layer = new DrawingVisual();
            visuals.Add(Layer);
        }

        public void SetupData(int ecg)
        {
            ecg_points[currentStart] = ecg;
            for (int i = 1; i <= 20; i++)
            {
                ecg_points[currentStart + i] = null;
            }

            currentStart++;
            if (currentStart >= RenderSize.Width / 2)
            {
                currentStart = 0;
            }

            x_offset = 0;

            DrawEcgLine();
            InvalidateVisual();
        }

        private void DrawEcgLine()
        {
            DrawingContext dc = Layer.RenderOpen();

            var scale = RenderSize.Height / (Top_ecg_max - Top_ecg_min);
            y_scale = (RenderSize.Height - Bottom) / (Top_ecg_max - Top_ecg_min);
            //y_offset = Top_ecg_min * -y_scale;

            Matrix mat = new Matrix();
            mat.ScaleAt(1, -1, 0, RenderSize.Height / 2);

            mat.OffsetX = -x_offset;

            dc.PushTransform(new MatrixTransform(mat));

            //横线
            for (int y = 0; y <= Top_ecg_max - Top_ecg_min; y += 10)
            {
                Point point1 = new Point(x_offset, y * y_scale + Bottom);
                Point point2 = new Point(x_offset + RenderSize.Width, y * y_scale + Bottom);
                if (y % Y_Sex == 0)
                {
                    dc.DrawLine(primarygrid_pen, point1, point2);
                    continue;
                }
                dc.DrawLine(secondgrid_pen, point1, point2);
            }

            //竖线与文字
            for (int i = 0; i <= (x_offset + RenderSize.Width); i += X_Sex * 2)
            {
                if (i < x_offset)
                {
                    continue;
                }
                var point1 = new Point(i, Bottom);
                var point2 = new Point(i, (Top_ecg_max - Top_ecg_min) * y_scale + Bottom);

                //Y轴文字
                if (i % 100 == 0)
                {
                    var text1 = new FormattedText(i + "", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 16, Brushes.Black);
                    var mat3 = new Matrix();
                    mat3.ScaleAt(1, -1, i - text1.Width / 2, 8 + text1.Height / 2);
                    dc.PushTransform(new MatrixTransform(mat3));
                    dc.DrawText(text1, new Point(i - text1.Width / 2, 8));
                    dc.Pop();
                }

                //表格刻度文字
                if (i % 100 == 0)
                {
                    for (int y = Top_ecg_min; y <= Top_ecg_max; y += 10)
                    {
                        if (y % Y_Sex == 0)
                        {
                            var text1 = new FormattedText(y + "", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Black);
                            var mat3 = new Matrix();
                            mat3.ScaleAt(1, -1, i + 1, (y - Top_ecg_min) * y_scale + Bottom + (text1.Height / 2));
                            dc.PushTransform(new MatrixTransform(mat3));
                            dc.DrawText(text1, new Point(i + 1, y: ((y - Top_ecg_min) * y_scale) + Bottom));
                            dc.Pop();
                        }
                    }
                    //深色竖线
                    dc.DrawLine(primarygrid_pen, point1, point2);
                    continue;
                }
                //浅色竖线
                dc.DrawLine(secondgrid_pen, point1, point2);
            }

            for (int i = 0, left = 0; left < RenderSize.Width; i++, left += 2)
            {
                if (ecg_points[i] == null || ecg_points[i + 1] == null)
                {
                    continue;
                }

                dc.DrawLine(ecg_pen, new Point(left, ((ecg_points[i].Value - Top_ecg_min) * y_scale) + Bottom), new Point(left + 2, ((ecg_points[i + 1].Value - Top_ecg_min) * y_scale) + Bottom));
            }

            dc.Pop();
            dc.Close();
        }

        protected override int VisualChildrenCount => visuals.Count;
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            base.OnRender(drawingContext);
        }
    }
}
