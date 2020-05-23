using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using MeasurementApp.Annotations;

namespace MeasurementApp.Models
{
    public class Client: IDisposable, INotifyPropertyChanged, IDataErrorInfo
    {
        public int Id { get; private set; }

        private static int _id;

        private string _firstName;

        [Required]
        [DisplayName("Имя")]
        public string FirstName 
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName;

        [Required]
        [DisplayName("Фамилия")]
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private long _phoneNumber;

        [Required]
        [Range(9000000000, 9999999999)]
        [DisplayName("Телефон")]
        public long PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                var client = Client.AllClients.FirstOrDefault(c => c.PhoneNumber == value && c.Id != this.Id);
                if (client != null)
                {
                    MessageBox.Show($"Телефон \"{value}\" уже зарегистрирован. Измените номер.");
                    return;
                }

                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));

            }
        }


        private int? _cityId;

        [Required]
        public int? CityId
        {
            get => _cityId;
            set
            {
                _cityId = value;
                OnPropertyChanged(nameof(CityId));
            }
        }

        private City _city;

        [Required]
        public City City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged(nameof(City));
            }
        }

        public City GetCity => City.AllCities.FirstOrDefault(c => c.Id == _cityId);

        private string _address;

        [Required]
        [DisplayName("Адрес клиента")]
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public static ObservableCollection<Client> AllClients;
        //public static ObservableCollection<long> AllPhones;

        static Client()
        {
            _id = 0;
            AllClients = new ObservableCollection<Client>();
            //AllPhones = new ObservableCollection<long>();
        }

        public Client()
        {
            Id = ++_id;
        }

        public Client(string firstName, string lastName, long phone, City city, string address) : this()
        {
            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Введите имя клиента.");
                return;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Введите фамилию клиента.");
                return;
            }

            if (phone == 0)
            {
                MessageBox.Show("Введите телефон клиента.");
                return;
            }

            if (city == null)
            {
                MessageBox.Show("Выберите город клиента.");
                return;
            }

            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Введите адрес клиента.");
                return;
            }

            _firstName = firstName;
            _lastName = lastName;
            _phoneNumber = phone;
            _cityId = city.Id;
            _city = city;
            _address = address;
        }

        public Client GetRandomClient()
        {
            var rnd = new Random();
            var listNames = new List<string>() { "Алексей", "Сергей", "Иван", "Дмитрий", "Петр", "Тимур", "Семен", "Максим", "Григорий" };
            int numName = rnd.Next(listNames.Count);

            var listLastname = new List<string>() { "Сергеев", "Бадаев", "Пупкин", "Иванов", "Депордье", "Филимонов", "Быков", "Киреев", "Тупиков", "Агиев" };
            int numLastName = rnd.Next(listLastname.Count);

            var address = GetSomeAddress();

            return new Client(listNames[numName], listLastname[numLastName], GetUniqueNumberPhone(), address.Item2, address.Item1);

        }

        private long GetUniqueNumberPhone()
        {
            var rnd = new Random();
            long phone;
            while (true)
            {
                phone = 9000000000 + rnd.Next(10000, 99999);
                if (AllClients.FirstOrDefault(c=>c.PhoneNumber == phone) == null)
                    break;
            }
            return phone;
        }

        public (string, City) GetSomeAddress()
        {
            var rnd = new Random();
            
            var list_streets = new List<string>() { "Беговая", "Клочкова", "Самарская", "Кучкина", "Проспект Кирова", "Смоленская", "Торофеева", "Гурина", "Первомайская" };

            City city = City.AllCities[rnd.Next(City.AllCities.Count)];
            
            string address = $"г. {city.Name}, ул. {list_streets[rnd.Next(list_streets.Count)]}, д. {rnd.Next(1,500)}, кв. {rnd.Next(1,600)}";

            return (address, city);
        }

        public void Dispose()
        {
            //AllPhones.Remove(this.PhoneNumber);
            AllClients.Remove(this);
        }


        public string Error => "Ошибка ввода данных клиента";

        private Dictionary<string, string> Errors = new Dictionary<string, string>();

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;

                switch (columnName)
                {
                    case "FirstName":
                        if (string.IsNullOrEmpty(FirstName))
                        {
                            error = "Имя не может иметь пустые значения";
                            Errors["FirstName"] = error;
                        }
                        else
                        {
                            if (Errors.ContainsKey("FirstName"))
                                Errors.Remove("FirstName");
                        }
                        break;

                    case "LastName":
                        if (string.IsNullOrEmpty(LastName))
                        {
                            error = "Фамилия не может иметь пустые значения!";
                            Errors["LastName"] = error;
                        }
                        else
                        {
                            if (Errors.ContainsKey("LastName"))
                                Errors.Remove("LastName");
                        }
                        break;

                    case "Address":
                        if (string.IsNullOrEmpty(Address))
                        {
                            error = "Адрес не может быть пустым";
                            Errors["Address"] = error;
                            
                        }
                        else
                        {
                            if (Errors.ContainsKey("Address"))
                                Errors.Remove("Address");
                        }
                        break;
                    case "PhoneNumber":
                        var client = Client.AllClients.FirstOrDefault(c => c.PhoneNumber == PhoneNumber);
                        if (PhoneNumber < 9000000000 || PhoneNumber > 9999999999)
                        {
                            error = "Телефон валиден в диапазоне 9000000000-9999999999\n";
                            Errors["PhoneNumber"] = error;
                        }
                        else if (client != null && client.Id != this.Id)
                        {
                            error = $"Телефон \"{PhoneNumber}\" уже зарегистрирован\n";
                            Errors["PhoneNumber"] = error;
                        }
                        else
                        {
                            if (Errors.ContainsKey("PhoneNumber"))
                                Errors.Remove("PhoneNumber");
                        }
                        
                        break;
                    case "City":
                        if (City == null)
                        {
                            error = "Выберите город";
                            Errors["City"] = error;
                        }
                        else
                        {
                            if (Errors.ContainsKey("City"))
                                Errors.Remove("City");
                        }
                        break;
                }



                return error;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
