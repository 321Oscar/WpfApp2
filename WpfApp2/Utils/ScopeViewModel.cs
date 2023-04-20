using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using ProtocolLib.Signal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp2.Model;
using WpfApp2.View;

namespace WpfApp2.Utils
{
    public class ScopeViewModel : BaseDataModelView, IDataUCClosing
    {
        private ObservableCollection<ScopeSignal> _ScopeSignals;
        public ObservableCollection<ScopeSignal> ScopeSignals { get => _ScopeSignals; private set=> SetProperty(ref _ScopeSignals, value); }
        private List<DBCSignal> DbcSignal;
        private bool _isGetData;
        public bool IsGetdata { get => _isGetData; set => SetProperty(ref _isGetData, value); }
        private CancellationTokenSource tokenSource;
        /// <summary>
        /// 生成测试数据随机数
        /// </summary>
        private readonly Random r = new Random();
        private Dictionary<string, IList<DataPoint>> points;
        private int maxPointCount = 2000;
        public int MaxPointCount { get => maxPointCount; set => SetProperty(ref maxPointCount, value); }
        private int interval = 10;
        public int Interval { get => interval; set => SetProperty(ref interval, value); }
        private PlotModel _ChartModel;
        public PlotModel ChartModel
        {
            get => _ChartModel;
            set => SetProperty(ref _ChartModel, value);
        }
        private bool disposedValue;

        public ScopeViewModel(ProjectItem pItem,FormItem formItem)
        {
            FormItem = formItem;
            ProjectItem = pItem;
            InitChart();
            DbcSignal = formItem.Singals.Signal;
        }

        #region -- Public --

        /// <summary>
        /// 启动获取数据
        /// </summary>
        public async Task StartOrStopGet()
        {
            if (IsGetdata)//停止
            {
                IsGetdata = false;
                // cancel the worker tasks
                this.tokenSource.Cancel();
                this.tokenSource = null;
            }
            else
            {
                IsGetdata = true;

                ClearOxyData();
                this.tokenSource = new CancellationTokenSource();

                var context = SynchronizationContext.Current;
                //采数据
                // Start the point calculation worker task
                Task<int> t = GetSignalData(tokenSource.Token);
                Task<int> tP = Update(context, this.tokenSource.Token);
                int x = await t;
                //Task.Factory.StartNew(() => this.GetSignalData(this.tokenSource.Token), this.tokenSource.Token);
                ////描点
                //Task.Factory.StartNew(() => this.Update(context, this.tokenSource.Token), this.tokenSource.Token);

            }
        }

        public void ModifiedSignals()
        {
            ModifiedFormItemForm modifiedFormItemForm = new(ProjectItem, FormItem, true);
            if (modifiedFormItemForm.ShowDialog() == true)
            {
                IsGetdata = false;
                ChangeSignals();
            }
        }

        public override void ChangeSignals()
        {
            LoadLines();
        }
        #endregion

        #region -- Priavte --

        private void InitChart()
        {
            _ChartModel = new PlotModel()
            {
                Title = "实时数据",
                //IsLegendVisible = false
                //Background = OxyColor.b
            };

            var l = new Legend
            {
                LegendPosition = LegendPosition.TopCenter,
                LegendPlacement = LegendPlacement.Outside,
                LegendBorder = OxyColors.Black,
                LegendBorderThickness =1,
                LegendItemAlignment = HorizontalAlignment.Center,
                LegendOrientation = LegendOrientation.Horizontal
            };
            _ChartModel.Legends.Add(l);

            //x轴
            _ChartModel.Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
            });
           
            LoadLines();
        }

        private void LoadLines()
        {
            _ChartModel.Series.Clear();
            //删除Y轴
            while (_ChartModel.Axes.Count > 1)
            {
                _ChartModel.Axes.RemoveAt(1);
            }
            ScopeSignals = new ObservableCollection<ScopeSignal>();
            points = new Dictionary<string, IList<DataPoint>>();
            for (int i = 0; i < FormItem.Singals.Signal.Count; i++)
            {
                DBCSignal signal = FormItem.Singals.Signal[i];

                points.Add(signal.SignalName, new List<DataPoint>());

                signal.IsSelected = true;
                FormItem.Singals.Signal[i].DValue = 0d;
                //删除重复的Y轴 并添加新的Y轴
                var existAxes = _ChartModel.Axes.Where(x => x.Key == signal.SignalName).ToArray();
                if (existAxes.Length > 0)
                {
                    _ChartModel.Axes.Remove(existAxes[0]);
                }

                _ChartModel.Axes.Add(new LinearAxis
                {
                    Position = i % 2 == 0 ? AxisPosition.Left : AxisPosition.Right,//i % 2 == 0 ? AxisPosition.Left : AxisPosition.Right,
                    //StartPosition = i / (double)formItem.Singals.Signal.Count,
                    //EndPosition = (i + 0.8) / (double)formItem.Singals.Signal.Count,
                    PositionTier = i / 2,
                    AxislineStyle = LineStyle.Solid,
                    // TextColor = OxyColor.FromRgb(255, (byte)i, 255),
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,//MaximumPadding
                    Key = signal.SignalName,//$"Y{i}"
                    Title = signal.SignalName,
                });

                //增加曲线
                var series = new LineSeries()
                {
                    //Color = OxyColor.FromRgb(255, (byte)i, 255),
                    StrokeThickness = 1,
                    MarkerSize = 3,
                    MarkerStroke = OxyColors.DarkGreen,
                    MarkerType = MarkerType.None,
                    Title = signal.SignalName,
                    InterpolationAlgorithm = null,//InterpolationAlgorithms.CanonicalSpline，
                    YAxisKey = signal.SignalName//$"Y{i}" //y轴的Key
                };
                var scopeSignal = new ScopeSignal()
                {
                    LinearColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(series.Color.R, series.Color.G, series.Color.B)),
                    SignalName = signal.SignalName,
                    IsSelected = true,
                    DValue = 0,
                };
                scopeSignal.SelectedChange += ScopeSignal_SelectedChange;
                ScopeSignals.Add(scopeSignal);

                _ChartModel.Series.Add(series);

            }
            _ChartModel.InvalidatePlot(true);
        }

        private void ScopeSignal_SelectedChange(bool isSelected, string Name)
        {
            //throw new NotImplementedException();
            var lineSer2 = _ChartModel.Series.Where(x => x.Title == Name).ToArray()[0] as LineSeries;
            lineSer2.IsVisible = isSelected;

            var axes = _ChartModel.Axes.Where(x => x.Title == Name).ToArray()[0] as LinearAxis;
            axes.IsAxisVisible = isSelected;

            _ChartModel.InvalidatePlot(true);

            var signal = DbcSignal.Find(x => x.SignalName == Name);
            signal.IsSelected = isSelected;
        }

        private async Task<int> GetSignalData(CancellationToken token)
        {
            await Task.Run(() =>
            {
                while (!token.IsCancellationRequested && IsGetdata)
                {
                    lock (this.points)
                    {
                        foreach (var item in DbcSignal)
                        {
                            if (item.IsSelected)
                            {
                                item.DValue = 100 * r.NextDouble();
                                //显示数据


                                DateTime dt = DateTime.Now;
                                if (points[item.SignalName].Count > MaxPointCount)
                                {
                                    points[item.SignalName].Clear();
                                }
                                if (item.DValue > 75 || item.DValue < 25)
                                {
                                    points[item.SignalName].Add(new DataPoint(double.NaN, double.NaN));
                                    item.DValue = double.NaN;
                                }
                                else
                                {
                                    points[item.SignalName].Add(new DataPoint(DateTimeAxis.ToDouble(dt), item.DValue));
                                }

                                ScopeSignals.First(x => x.SignalName == item.SignalName).DValue = item.DValue;
                            }
                        }
                    }

                    Thread.Sleep(Interval);
                }
            }, token);
            return Interval;
        }

        private void ClearOxyData()
        {
            foreach (var item in points.Values)
            {
                item.Clear();
            }
        }
        /// <summary>
        /// 描点
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<int> Update(SynchronizationContext context, CancellationToken token)
        {
            await Task.Run(() =>
            {
                while (!token.IsCancellationRequested && IsGetdata)
                {
                    context.Post(_ => this.UpdatePlot(), null);
                    Thread.Sleep(250);
                }
            }, token);
            // this loop runs on a worker thread
            return 1;
        }

        private void UpdatePlot()
        {
            Debug.WriteLine("Updating on: " + Thread.CurrentThread.Name);
            lock (this.points)
            {
                foreach (var item in DbcSignal)
                {
                    if (item.IsSelected)
                    {
                        //添加点
                        var series = _ChartModel.Series.Where(x => x.Title == item.SignalName).ToArray()[0] as LineSeries;
                        series.Points.Clear();
                        series.Points.AddRange(this.points[item.SignalName]);

                        //ScopeSignals.First(x => x.SignalName == item.SignalName).LinearColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(series.Color.R, series.Color.G, series.Color.B));
                    }
                }
            }
            
            this.ChartModel.InvalidatePlot(true);
        }

        #endregion

        #region -- IDataUCClosing --

        public void Closing()
        {
            IsGetdata = false;
            if (tokenSource != null)
            {
                this.tokenSource.Cancel();
                this.tokenSource.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    Closing();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ScopeViewModel()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

}
