using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MinecraftArchive.control {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Test.Click += Test_Click;
            Test1.Click += Open_Click;
        }

        private async void Open_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            count++;
            tbv.Add("���Ա���", $"�� {count} �α�����", () => {
                Trace.WriteLine($"�� {count} �α�����");
            });
        }

        int count;
        private async void Test_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            count++;
            tbv.Add("���Ա���", $"�� {count} �α�����");
        }
    }
}