using System.Windows;

namespace tetris
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void StartPlay_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void Rule_Click(object sender, RoutedEventArgs e)
        {
            RuleWindow ruleWindow = new RuleWindow();
            ruleWindow.Show();
            Close();
        }
    }
}
