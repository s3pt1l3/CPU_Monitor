using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using System.Threading;
using System.Diagnostics;
using LiveCharts.Configurations;

namespace cpu_monitor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Считает загруженность процессора в процентах
            var cPerfCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");

            var mapper = Mappers.Xy<MeasureModel>()
                .X(x => x.ElapsedMilliseconds)
                .Y(x => x.Value);

            Charting.For<MeasureModel>(mapper);

            Values = new ChartValues<MeasureModel>();
            var sw = new Stopwatch();
            sw.Start();

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    float cpu_value = cPerfCounter.NextValue(); // Получение процента загруженности
                    string created_at = DateTime.Now.ToString(); // Текущее время

                    // Добавление значений времени и загруженности процессора на график
                    Values.Add(new MeasureModel
                    {
                        ElapsedMilliseconds = sw.ElapsedMilliseconds, // Сколько прошло миллисекунд
                        Value = cpu_value
                    });

                    // Добавление точки в бд
                    PointModel point = new PointModel
                    {
                        Cpu = cpu_value,
                        Created_at = created_at
                    };
                    DatabaseDataAccess.AddPoint(point);
                }
            });

            DataContext = this;
        }

        public ChartValues<MeasureModel> Values { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<PointModel> records = DatabaseDataAccess.SelectAll();
            DataGrid.ItemsSource = records;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
public class MeasureModel
{
    public double ElapsedMilliseconds { get; set; }
    public double Value { get; set; }
}
