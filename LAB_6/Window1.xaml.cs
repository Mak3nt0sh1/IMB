using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LAB_6
{
    public partial class DiagnosticToolWindow : Window
    {
        private Random rnd = new Random();
        private string activeProblem;
        private string activeFault;
        private int count = 0;

        private NormalDiagnosticData normalData = new NormalDiagnosticData();
        private FaultyDiagnosticData faultyData = new FaultyDiagnosticData();

        private List<RadioButton> optionButtons = new List<RadioButton>();
        private string selectedTool = string.Empty; // Инициализация по умолчанию

        public DiagnosticToolWindow(string problem, string fault)
        {
            InitializeComponent();
            activeProblem = problem;
            activeFault = fault;
            InitializeGameContent();
        }

        private void InitializeGameContent()
        {
            problemTextBlock.Text = $"{activeProblem}";

            var randomFaults = GenerateRandomFaults();

            foreach (var fault in randomFaults)
            {
                var radioButton = new RadioButton
                {
                    Content = fault,
                    Margin = new Thickness(2)
                };

                optionButtons.Add(radioButton);
            }
        }

        private List<string> GenerateRandomFaults()
        {
            var allFaults = faultyData.FaultyResults.Keys.ToList();
            var randomFaults = allFaults.OrderBy(x => rnd.Next()).Take(2).ToList();
            randomFaults.Add(activeFault);
            return randomFaults.OrderBy(x => rnd.Next()).ToList();
        }

        private void ConfirmFault_Click(object sender, RoutedEventArgs e)
        {
            var selectedFault = optionButtons.FirstOrDefault(rb => rb.IsChecked == true)?.Content.ToString();

            if (selectedFault == activeFault)
            {
                MessageBox.Show("Правильно! Вы верно выявили проблему.");
                DialogResult = true;

                // Закрываем окна
                this.Close();
                Application.Current.Windows.OfType<DiagnosticToolWindow>().FirstOrDefault()?.Close();

                // Находим главное окно и обновляем проблему
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    mainWindow.MarkProblemAsSolved(activeProblem);
                }
            }
            else
            {
                MessageBox.Show("Неправильно! Попробуйте еще раз.");
            }
        }


        private void SelectTool(object sender, RoutedEventArgs e)
        {
            Button toolButton = (Button)sender;
            selectedTool = toolButton.Content?.ToString() ?? string.Empty; // Убедитесь, что значение не NULL
            MessageBox.Show($"Выбран инструмент: {selectedTool}");
        }


        private void TestComponent(object sender, RoutedEventArgs e)
        {
            Button componentButton = (Button)sender;
            string componentName = componentButton.Content?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для тестирования!");
                return;
            }

            string result = PerformTest(componentName, selectedTool);
            MessageBox.Show(result);
        }

        private string PerformTest(string componentName, string tool)
        {
            Dictionary<string, Dictionary<string, string>> resultsDictionary;

            if (faultyData.FaultyResults.ContainsKey(activeFault) && activeFault == componentName)
            {
                resultsDictionary = faultyData.FaultyResults;
            }
            else if (normalData.NormalResults.ContainsKey(componentName))
            {
                resultsDictionary = normalData.NormalResults;
            }
            else if (faultyData.FaultyResults.ContainsKey(componentName))
            {
                resultsDictionary = faultyData.FaultyResults;
            }
            else
            {
                return "Неизвестный компонент.";
            }

            if (!resultsDictionary[componentName].ContainsKey(tool))
            {
                var availableTools = string.Join(", ", resultsDictionary[componentName].Keys);
                return $"Для компонента '{componentName}' следует использовать: {availableTools}.";
            }

            return resultsDictionary[componentName][tool];
        }

        

        private void askButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы смогли определить неисправность?", "Завершение диагностики", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Поздравляем!", "Завершение диагностики");
                this.Close();
                Application.Current.Windows.OfType<DiagnosticToolWindow>().FirstOrDefault()?.Close();
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    mainWindow.MarkProblemAsSolved(activeProblem);
                }
            }
            else if (result == MessageBoxResult.No)
            {
                count++;
                if (count == 1)
                {
                    MessageBox.Show("Придется попробовать еще раз", "Завершение диагностики");
                }
                else if (count == 2)
                {
                    MessageBox.Show($"Вы дважды не смогли определить неисправность. { activeProblem }", "Завершение диагностики");
                    this.Close();
                    Application.Current.Windows.OfType<DiagnosticToolWindow>().FirstOrDefault()?.Close();

                    var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    if (mainWindow != null)
                    {
                        mainWindow.MarkProblemAsUnSolved(activeProblem);
                    }
                }
            }
        }


        // Остальные методы диагностики...
        private void CheckPowerLines(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для проверки линий питания!");
                return;
            }
            string result = PerformTest("Короткое замыкание", selectedTool);
            MessageBox.Show(result);
        }

        private void CheckUSBVoltage(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для проверки напряжения USB!");
                return;
            }
            string result = PerformTest("Сгорел южный мост", selectedTool);
            MessageBox.Show(result);
        }

        private void TestVideoCardLines(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для теста линий видеокарты!");
                return;
            }
            string result = PerformTest("Проблема со слотом расширения PSI Express", selectedTool);
            MessageBox.Show(result);
        }

        private void CheckFrequencyGenerator(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для проверки частоты генератора!");
                return;
            }
            string result = PerformTest("Неисправен базовый генератор", selectedTool);
            MessageBox.Show(result);
        }

        private void TestSoundSignal(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для теста звукового сигнала!");
                return;
            }
            string result = PerformTest("Неисправен звуковой адаптер", selectedTool);
            MessageBox.Show(result);
        }

        private void CheckSIO(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для проверки сигналов SIO/MIO!");
                return;
            }
            string result = PerformTest("Неисправен микроконтроллер SIO/MIO", selectedTool);
            MessageBox.Show(result);
        }

        private void TestGPULines(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для проверки питания кулера");
                return;
            }
            string result = PerformTest("Неисправно питание кулера", selectedTool);
            MessageBox.Show(result);
        }

        private void TestRTC(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для теста часового резонатора!");
                return;
            }
            string result = PerformTest("Неисправность часового кварцевого резонатора", selectedTool);
            MessageBox.Show(result);
        }

        private void Indikator(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для проверки индикаторов");
                return;
            }
            string result = PerformTest("Неисправен разъем JFP-1", selectedTool);
            MessageBox.Show(result);
        }

        private void TestBIOS(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для теста BIOS!");
                return;
            }
            string result = PerformTest("Проблемы в микросхеме BIOS", selectedTool);
            MessageBox.Show(result);
        }

        private void TestMemory(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTool))
            {
                MessageBox.Show("Выберите инструмент для теста оперативной памяти!");
                return;
            }
            string result = PerformTest("Проблемы с оперативной памятью", selectedTool);
            MessageBox.Show(result);
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            foreach (var radioButton in optionButtons)
            {
                if (radioButton.IsChecked == true)
                {
                    MessageBox.Show($"Ваш выбор: {radioButton.Content}");
                }
            }
        }

       
    }

        public class NormalDiagnosticData
    {
        public Dictionary<string, Dictionary<string, string>> NormalResults;

        public NormalDiagnosticData()
        {
            NormalResults = new Dictionary<string, Dictionary<string, string>>
            {
                {"Короткое замыкание", new Dictionary<string, string>
                {
                    {"Мультиметр", "Короткого замыкания не обнаружено между линиями +12V, +5V, +3.3V и землей."},
                    {"Осциллограф", "Сигнал имеет регулярную форму, без шумов и колебаний."}
                }},
                {"Проблемы в микросхеме BIOS", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление и напряжение микросхемы BIOS в пределах нормы."},
                    {"Осциллограф", "Сигналы BIOS не содержат помех, все работает в штатном режиме."}
                }},
                {"Сгорел южный мост", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление USB портов допустимое. Мультиметр показывает стабильное значение напряжения в 5V."},
                    {"Осциллограф", "USB-сигналы устойчивы и не имеют отклонений."}
                }},
                {"Неисправность часового кварцевого резонатора", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление не превышает нормального значения. Напряжение стабильно."},
                    {"Осциллограф", "Частота сигнала резонатора соответствует заявленному значению."}
                }},
                {"Проблема со слотом расширения PSI Express", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление и напряжение на линиях видеокарты нормальное."},
                    {"Осциллограф", "Сигнал имеет регулярную форму, без шумов и колебаний."}
                }},
                {"Неисправен звуковой адаптер", new Dictionary<string, string>
                {
                    {"Мультиметр", "Аудио цепи показывают нормальное сопротивление и стабильное напряжение."},
                    {"Осциллограф", "Аудиосигналы чистые, звук в норме."}
                }},
                {"Неисправен микроконтроллер SIO/MIO", new Dictionary<string, string>
                {
                    {"Омметр", "Сигнальные линии SIO/MIO в норме. Напряжение имеет допустимое значение."},
                    {"Осциллограф", "Сигналы SIO/MIO ясные. Помехи отсутствуют."}
                }},
                {"Неисправен базовый генератор", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление и напряжение контура генератора в норме"},
                    {"Осциллограф", "Частота генерации в норме."}
                }},
                {"Неисправно питание кулера", new Dictionary<string, string>
                {
                    {"Мультиметр", "Короткого замыкания не обнаружено между линией +12V и землёй"},
                    {"Осциллограф", "Сигнал имеет регулярную форму, без шумов и колебаний."}
                }},
                {"Проблемы с оперативной памятью", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление оперативной памяти находится в допустимых значениях. Напряжение стабильное."},
                    {"Осциллограф", "Сигналы передачи данных оперативной памяти стабильны и не искажены."}
                }},
                {"Неисправен разъем JFP-1", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление и напряжение на контактах не отклоняются от нормального значения."},
                    {"Осциллограф", "Сигнал имеет регулярную форму, без шумов и колебаний."}
                }}
            };
        }
    }

    public class FaultyDiagnosticData
    {
        public Dictionary<string, Dictionary<string, string>> FaultyResults;

        public FaultyDiagnosticData()
        {
            FaultyResults = new Dictionary<string, Dictionary<string, string>>
            {
                {"Короткое замыкание", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Обнаружено низкое сопротивление между линиями +12V, +5V, +3.3V и землей, указывающее на короткое замыкание."},
                        {"Осциллограф", "Сигналы на линиях электропитания искажены, имеются пики и провалы."}
                    }
                },
                {"Неисправно питание кулера", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Обнаружено низкое сопротивление между линией +12V и землей, питание кулера неисправно"},
                        {"Осциллограф", "Сигналы на линиях электропитания искажены, имеются пики и провалы."}
                    }
                },
                {"Сгорел южный мост", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Сопротивление на микросхеме южного моста ниже нормы, напряжение не стабильно, возможно внутреннее короткое замыкание."},
                        {"Осциллограф", "Сигналы на линиях южного моста искажены, зафиксированы нестабильные колебания и шумы."}
                    }
                },
                {"Проблемы в микросхеме BIOS", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Сопротивление и напряжение микросхемы в норме, но это не исключает возможность программной ошибки."},
                        {"Осциллограф", "Сигналы отклоняются от нормы, возможно, из-за ошибки прошивки или повреждения микросхемы."}
                    }
                },
                {"Неисправность часового кварцевого резонатора", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Напряжение и сопротивление цепи нормальное, но это не исключает возможность проблемы с самим резонатором."},
                        {"Осциллограф", "Сигнал резонатора отсутствует или значительно отличается от нормы, требуется замена."}
                    }
                },
                {"Проблема со слотом расширения PSI Express", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Напряжение в разъеме видеокарты нестабильно, имеются пики и провалы."},
                        {"Осциллограф", "Сигналы в линиях передачи данных видеокарты искажены, присутствуют помехи и шумы."}
                    }
                },
                {"Неисправен базовый генератор", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Сопротивление контура генератора и напряжение на генераторе частоты в норме, но это не исключает возможность проблемы с самим резонатором."},
                        {"Осциллограф", "Частота сигнала значительно отличается от заявленной, требуется замена."}
                    }
                },
                {"Неисправен звуковой адаптер", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Сопротивление и напряжение микроконтроллера в пределах нормы, но это не исключает возможность проблемы с самим микроконтроллером."},
                        {"Осциллограф", "Сигналы генерации звука отсутствуют или искажены, требуется замена микроконтроллера."}
                    }
                },
                {"Неисправен микроконтроллер SIO/MIO", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Напряжение питания и сопротивление микроконтроллера SIO/MIO в норме."},
                        {"Осциллограф", "Сигналы управления не соответствуют спецификациям, требуется замена микроконтроллера."}
                    }
                },
                {"Проблемы с оперативной памятью", new Dictionary<string, string>
                    {
                        {"Мультиметр", "Обнаружено низкое сопротивление на одной из линий оперативной памяти, указывающее на короткое замыкание."},
                        {"Осциллограф", "Сигналы передачи данных оперативной памяти искажены, зафиксированны значительные сильные помехи и шумы."}
                    }
                },
                 {"Неисправен разъем JFP-1", new Dictionary<string, string>
                {
                    {"Мультиметр", "Сопротивление и напряжение на контактах отсутствуют, разъем вышел из строя."},
                    {"Осциллограф", "Сигнал отсутствует, требуется ремонт."}
                }}

            };
        }
    }
}


