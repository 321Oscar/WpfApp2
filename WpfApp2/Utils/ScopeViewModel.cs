using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Utils
{
    public class ScopeViewModel : BindableBase
    {
        public ScopeViewModel()
        {
            GetData().ContinueWith(x => ChartModel = CreateChartModel(x.Result));
        }

        public ScopeViewModel(List<ChartData> list)
        {
            _ = Task.Run(new Action(() =>
            {
                ChartModel = CreateChartModel(list);
            }
            ));
        }

        private PlotModel _ChartModel;
        public PlotModel ChartModel
        {
            get => _ChartModel;
            set
            {
                SetProperty(ref _ChartModel, value);
            }
        }

        private Task<List<ChartData>> GetData()
        {
            List<ChartData> data = new()
            {
                new ChartData { Date = DateTime.Now.Date.AddDays(-15), Total = 121, PassRate = .84 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-14), Total = 88, PassRate = .92 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-13), Total = 180, PassRate = .35 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-12), Total = 150, PassRate = .46 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-11), Total = 78, PassRate = .58 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-10), Total = 99, PassRate = .71 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-9), Total = 143, PassRate = .81 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-8), Total = 56, PassRate = .85 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-7), Total = 108, PassRate = .95 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-6), Total = 79, PassRate = .78 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-5), Total = 63, PassRate = .65 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-4), Total = 157, PassRate = .58 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-3), Total = 148, PassRate = .36 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-2), Total = 115, PassRate = .48 },
                new ChartData { Date = DateTime.Now.Date.AddDays(-1), Total = 89, PassRate = .63 },
            };

            return Task.FromResult(data);
        }

        private PlotModel CreateChartModel(List<ChartData> list)
        {
            var model = new PlotModel() { Title = "测试" };

            //添加图说明
            model.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0,
                LegendTextColor = OxyColor.FromRgb(244, 12, 12)
            });

            //定义第一个Y轴y1，显示数量
            LinearAxis ay1 = new()
            {
                Key = "y1",
                Position = AxisPosition.Left,
            };

            //定义第二个y轴 y2,显示百分比
            LinearAxis ay2 = new() { Key = "y2", Position = AxisPosition.Right, Minimum = 0.1, MajorStep = .1, LabelFormatter = v => $"{v:P1}" };
            //在第二Y轴坐标50%和80%显示网格线
            ay2.ExtraGridlines = new double[2] { 0.5, 0 / 8 };
            ay2.ExtraGridlineStyle = LineStyle.DashDashDot;

            //定义X轴为日期，从15天前到现在
            var minValue = DateTimeAxis.ToDouble(DateTime.Now.Date.AddDays(-15));
            var maxvalue = DateTimeAxis.ToDouble(DateTime.Now.Date);
            DateTimeAxis ax = new()
            {
                Minimum = minValue,
                Maximum = maxvalue,
                StringFormat = "yyyy-MM-dd日",
                MajorStep = 2,
                Position = AxisPosition.Bottom,
                Angle = 45,
                IsZoomEnabled = false
            };

            //定义柱形图序列，指定数据轴为Y1
            LinearBarSeries totalBarSeries = new();
            totalBarSeries.YAxisKey = "y1";
            totalBarSeries.BarWidth = 10;
            totalBarSeries.Title = "总数";
            //点击时弹出的Label内容
            totalBarSeries.TrackerFormatString = "{0}\r\n{2:dd}日：{4:0}";
            //设置数据绑定字段
            totalBarSeries.ItemsSource = list;
            totalBarSeries.DataFieldX = "Date";
            totalBarSeries.DataFieldY = "Total";
            //下面为手动添加数据方式
            //totalBarSeries.Points.Add();

            //定义三色折线图序列，指定数据轴为y2
            ThreeColorLineSeries passedRateSeries = new();
            passedRateSeries.Title = "通过率";
            passedRateSeries.YAxisKey = "y2";
            passedRateSeries.TrackerFormatString = "{0}\r\n{2:dd}日：{4:P1}";
            //设置颜色阈值
            passedRateSeries.LimitHi = .8;
            passedRateSeries.LimitLo = .5;
            //设置数据源和字段
            passedRateSeries.ItemsSource = list;
            passedRateSeries.DataFieldX = "Date";
            passedRateSeries.DataFieldY = "PassRate";

            model.Series.Add(totalBarSeries);
            model.Series.Add(passedRateSeries);
            model.Axes.Add(ay1);
            model.Axes.Add(ay2);
            model.Axes.Add(ax);

            model.PlotAreaBorderThickness = new OxyThickness(1, 0, 1, 1);

            return model;
        }
    }

    public class ChartData
    {
        public DateTime Date { get; set; }
        public double Total { get; set; }
        public double PassRate { get; set; }
    }
}
