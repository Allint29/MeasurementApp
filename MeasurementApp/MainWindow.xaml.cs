using System;
using System.Collections;
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
using MeasurementApp.Models;
using MeasurementApp.ViewModels;

namespace MeasurementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Init();

            tcCities.DataContext = new CityViewModel();
            tcClients.DataContext = new ClientViewModel();
            tcLimits.DataContext = new MeasurementLimitViewModel();
            tcMeasurements.DataContext = new MeasurementViewModel();
        }

        private void Init()
        {
            //Добавляю города
            City[] arrCities =
            {
                new City("Москва"),
                new City("Санкт-Петербург"),
                new City("Краснодар"),
                new City("Липецк"),
                new City("Саратов"),
                new City("Новороссийск"),
            };

            foreach (var c in arrCities)
            {
                City.AllCities.Add(c);
            }

            //Добавляю клиентов

            Client[] arrClients =
            {
                new Client("Иван", "Иванов", 9271011001, City.AllCities[0], //0
                    $"г.{City.AllCities[0].Name}, ул. Михалкова, 11"),
                new Client("Артем", "Кутузов", 9271011002, City.AllCities[2],
                    $"г.{City.AllCities[2].Name}, ул. Куприянова, 41"),
                new Client("Генналий", "Петров", 9271011003, City.AllCities[0], //2
                    $"г.{City.AllCities[0].Name}, ул. Тихорецкая, 17"),
                new Client("Иван", "Уткин", 9271011004, City.AllCities[1],
                    $"г.{City.AllCities[1].Name}, ул. Кипра, 1"),
                new Client("Петр", "Картошкин", 9271011005, City.AllCities[3],
                    $"г.{City.AllCities[3].Name}, ул. Тумаковва, 3"),
                new Client("Виталий", "Цукенберг", 9271011006, City.AllCities[3],
                    $"г.{City.AllCities[3].Name}, ул. Герцина, 5"),
                new Client("Сергей", "Цикало", 9271011007, City.AllCities[0], //6
                    $"г.{City.AllCities[0].Name}, ул. Новотихая, 11"),
                new Client("Максим", "Трофимов", 9271011008, City.AllCities[4], //--7
                    $"г.{City.AllCities[4].Name}, ул. Любицкая, 7"),
                new Client("Алексей", "Курочкин", 9271011009, City.AllCities[0], //8
                    $"г.{City.AllCities[0].Name}, ул. Туннельная, 10"),
                new Client("Иван", "Тучин", 9271011011, City.AllCities[2],
                    $"г.{City.AllCities[2].Name}, ул. Куприянова, 16"),
                new Client("Антон", "Царевич", 9271011017, City.AllCities[0], //10
                    $"г.{City.AllCities[0].Name}, ул. Новотихая, 11"),
                new Client("Сергей", "Трофимов", 9271011018, City.AllCities[4], //--11
                    $"г.{City.AllCities[4].Name}, ул. Любицкая, 73"),
                new Client("Федор", "Курочкин", 9271011019, City.AllCities[0], //12
                    $"г.{City.AllCities[0].Name}, ул. Туннельная, 410"),
                new Client("Тихомир", "Тарасов", 9271011020, City.AllCities[2],
                    $"г.{City.AllCities[2].Name}, ул. Куприянова, 146"),
            };

            foreach (var c in arrClients)
            {
                Client.AllClients.Add(c);
            }

            //Добавляю лимиты замеров на города
            MeasurementLimit[] arrMesurLimits =
            {
                //Москва два лимита времени 
                new MeasurementLimit(8, 10, 4, City.AllCities[0]),
                new MeasurementLimit(15, 20, 4, City.AllCities[0]),
                //Санкт-Петербург два лимита времени 
                new MeasurementLimit(8, 14, 3, City.AllCities[1]),
                new MeasurementLimit(15, 20, 3, City.AllCities[1]),
                //Краснодар один лимит времени 
                new MeasurementLimit(8, 20, 5, City.AllCities[2]),
                //Липецк один лимит времени 
                new MeasurementLimit(8, 20, 3, City.AllCities[3]),
                //Саратов один лимит времени 
                new MeasurementLimit(8, 20, 4, City.AllCities[4]),
            };

            foreach (var m in arrMesurLimits)
            {
                MeasurementLimit.AllMeasurementLimits.Add(m);
            }

            Measurement[] arrMeas =
            {
                //запись на Москву 1 половина
                new Measurement(arrMesurLimits[0], arrClients[0]),
                //запись на Москву 1 половина
                new Measurement(arrMesurLimits[0], arrClients[2]),
                //запись на Москву 1 половина
                new Measurement(arrMesurLimits[0], arrClients[6]),
                //запись на Москву 1 половина -- полностью заполнена - если поставить им на одну и ту же дату
                new Measurement(arrMesurLimits[0], arrClients[10]),
                //запись на Саратов
                new Measurement(arrMesurLimits[6], arrClients[7]),
                //запись на Саратов
                new Measurement(arrMesurLimits[6], arrClients[11]),
            };

            int count = 0;
            foreach (var m in arrMeas)
            {
                if (count++ % 2 == 0)
                    m.SetDateForMeasurement(DateTime.Today.AddDays(1));
                Measurement.AllMeasurements.Add(m);

            }

        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private void MenuItem_OnClick1(object sender, RoutedEventArgs e)
        {
            string mes = "Помощь по вкладке 'Создать новый запрос на замер'.\n" +
                         "1) В начале нужно создать заявку на замер:\n" +
                         "   1.1) Выбрать город (клиенты и лимиты на город будут \n" +
                         "        отфильтрованы)\n" +
                         "   1.2) Выбрать из списка время для замера из списка \n" +
                         "        лимитов в данном городе\n" +
                         "   1.3) Выбрать клиента из списка (Клиента можно создать\n" +
                         "        на вкладке клиентов\n\n)" +
                         "   Результат: в нижнем списке создастся новый заказ на \n" +
                         "        замер в данном городе для данного клиента (заказы \n" +
                         "        могут повторяться, но при назначении для них дат\n" +
                         "        будут определены ограничения в зависимости от \n" +
                         "        усановленных лимитов на замеры для данного периода)\n" +
                         "";
            MessageBox.Show(mes, "Помощь");
        }

        private void MenuItem_OnClick2(object sender, RoutedEventArgs e)
        {
            string mes = "Помощь по вкладке 'Распределить замеры по датам'.\n" +
                         "1) Выбрать город в котором нужно распределить замеры;\n" +
                         "2) Нажать на кнопку 'Выбрать город';\n" +
                         "3) Выбрать заявку на замер из списка нераспределенных\n" +
                         "   заявок\n" +
                         "4) Выбрать дату в следующем поле;\n" +
                         "5) Нажать на кнопку 'Установить дату'\n" +
                         "\n" +
                         "Результат: \n" +
                         "   1) В нижнем списке появиться новая запись с \n" +
                         "    назначенной датой замера, а в верхнем списке она\n" +
                         "    исчезнет;\n" +
                         "   2) Если на данную дату в данном городе превышен лимит\n" +
                         "    на замеры - выпадет сообщение об ошибке и дата \n" +
                         "    назначена не будет.\n" +
                         "";

            MessageBox.Show(mes, "Помощь");
        }

        private void MenuItem_OnClick3(object sender, RoutedEventArgs e)
        {
            string mes = "Помощь по вкладке 'Посмотреть распределенные\n" +
                         "замеры по датам и городам'.\n\n" +
                         "На данной вкладке список с распределенными по\n" +
                         "датам заявкам на замеры. \n\n" +
                         "Для фильтрации можно выбрать город и период.\n" +
                         "Есть возможность удалить дату у распределенной заявки,\n" +
                         "тогда заявка попадет опять в список нераспределенных заявок.\n";

            MessageBox.Show(mes, "Помощь");
        }
    }
}
