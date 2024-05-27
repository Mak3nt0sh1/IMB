using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LAB_6
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> problems = new ObservableCollection<string>();
        private Dictionary<string, string> problemToFaultMapping = new Dictionary<string, string>();
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            problemListBox.ItemsSource = problems;
            InitialSetup();
        }

        private void InitialSetup()
        {
            
            problemToFaultMapping.Add("Отсутствует звук", "Неисправен звуковой адаптер");
            problemToFaultMapping.Add("Не включается монитор", "Проблема со слотом расширения PSI Express");
            problemToFaultMapping.Add("Не работают USB порты", "Сгорел южный мост");
            problemToFaultMapping.Add("Не включается компьютер", "Короткое замыкание");
            problemToFaultMapping.Add("Некорректное отображение времени", "Неисправность часового кварцевого резонатора");
            problemToFaultMapping.Add("Упала тактовая частота процессора", "Неисправен базовый генератор");
            problemToFaultMapping.Add("Не горят индикаторы на передней панели", "Неисправен разъем JFP-1");
            problemToFaultMapping.Add("Не работает контроллер ввода/вывода", "Неисправен микроконтроллер SIO/MIO");
            problemToFaultMapping.Add("Не работает охлаждение процессора", "Неисправно питание кулера");
            problemToFaultMapping.Add("Не запускается ОС", "Проблемы в микросхеме BIOS");

            foreach (var problem in problemToFaultMapping.Keys)
            {
                problems.Add(problem);
            }

        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (problemListBox.SelectedItem != null)
            {
                string selectedProblem = problemListBox.SelectedItem.ToString();
                string correspondingFault = problemToFaultMapping[selectedProblem];
                DiagnosticToolWindow diagnosticWindow = new DiagnosticToolWindow(selectedProblem, correspondingFault);
                if (diagnosticWindow.ShowDialog() == true)
                {
                    MarkProblemAsSolved(selectedProblem);
                }
            }
        }

        public void MarkProblemAsSolved(string problem)
        {
            var index = problemListBox.Items.IndexOf(problem);
            var container = problemListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;

            if (container != null)
            {
                container.Background = Brushes.Green;
            }

        }
        public void MarkProblemAsUnSolved(string problem)
        {
            var index = problemListBox.Items.IndexOf(problem);
            var container = problemListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;

            if (container != null)
            {
                container.Background = Brushes.OrangeRed;
            }

        }
    }
}