using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using MeasurementApp.Annotations;
using MeasurementApp.Models;
using MeasurementApp.Tools;

namespace MeasurementApp.ViewModels
{
    class ClientViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ObservableCollection<Client> Clients { get; set; }

        public ObservableCollection<City> Cities => Models.City.AllCities;

        public ObservableCollection<City> CitiesForFind { get; set; }

        public City AllCity;

        public ClientViewModel()
        {
            FindClientObject = new FindClientObject();

            AllCity = new City("Все города");

            CitiesForFind = new ObservableCollection<City>();
            CitiesForFind.Insert(0, AllCity);

            foreach (var c in City.AllCities)
            {
                CitiesForFind.Add(c);
            }

            Clients = new ObservableCollection<Client>();
            foreach (var c in Client.AllClients)
            {
                Clients.Add(c);
            }

            City.AllCities.CollectionChanged += AllCitiesOnCollectionChanged;
        }

        /// <summary>
        /// При изменении коллекции городов, обновляю список для поиска городов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllCitiesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    CitiesForFind.Insert(1, e.NewItems[0] as City);
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    City city = e.OldItems[0] as City;
                    if (city != null)
                    {
                        for (int i = Clients.Count()-1; i >= 0; i--)
                            if (Clients[i].City.Id == city.Id)
                                Clients.Remove(Clients[i]);

                        for (int i = Client.AllClients.Count() - 1; i >= 0; i--)
                            if (Client.AllClients[i].City.Id == city.Id)
                                Clients.Remove(Client.AllClients[i]);

                        if(CitiesForFind.Contains(city))
                            CitiesForFind.Remove(city);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                    CitiesForFind.Remove(e.OldItems[0] as City);
                    CitiesForFind.Insert(1, e.NewItems[0] as City);
                    break;
            }

        }

        private FindClientObject _findClientObject;

        public FindClientObject FindClientObject
        {
            get { return _findClientObject; }
            set
            {
                _findClientObject = value;
                OnPropertyChanged(nameof(_findClientObject));
            }
        }


        private Client _selectedItem;
        public Client SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        private string _firstName;
        [Required]
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
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private long _phone;
        [Required]
        [Range(9000000000, 9999999999)]
        public long PhoneNumber
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(PhoneNumber));

            }
        }

        private City _city;
        public City City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged(nameof(City));
            }
        }

        private string _address;
        [Required]
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }


        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(obj =>
                {
                    if (Errors.Count < 1)
                    {
                        var client = new Client(FirstName, LastName, PhoneNumber, City, Address);

                        Client.AllClients.Insert(0, client);

                        Clients.Clear();
                        foreach (var c in Client.AllClients)
                        {
                            Clients.Add(c);
                        }

                        SelectedItem = client;

                        this.LastName = "";
                        this.FirstName = "";
                        this.PhoneNumber = 0;
                        this.City = null;
                        this.Address = "";
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var k in Errors.Keys)
                        {
                            sb.Append($"Ошибка в поле {k}: {Errors[k]};\n");
                        }

                        MessageBox.Show(sb.ToString());
                    }
                });
            }
        }

        private RelayCommand _clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand ??= new RelayCommand(obj =>
                {
                    this.LastName = "";
                    this.FirstName = "";
                    this.PhoneNumber = 0;
                    this.City = null;
                    this.Address = "";
                });
            }
        }

        private RelayCommand _findCommand;
        public RelayCommand FindCommand
        {
            get
            {
                return _findCommand ??= new RelayCommand(obj =>
                {
                    if (FindClientObject != null)
                    {
                        Clients.Clear();

                        var list =
                            Client.AllClients
                                .Where(c => string.IsNullOrEmpty(FindClientObject.FirstName) ||
                                            c.FirstName.Contains(FindClientObject.FirstName, StringComparison.InvariantCultureIgnoreCase))
                                .Where(c => string.IsNullOrEmpty(FindClientObject.LastName) ||
                                            c.LastName.Contains(FindClientObject.LastName, StringComparison.InvariantCultureIgnoreCase))
                                .Where(c => string.IsNullOrEmpty(FindClientObject.PhoneNumber) ||
                                            c.PhoneNumber.ToString().Contains(FindClientObject.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
                                .Where(c => FindClientObject.City == null || FindClientObject.City.Id == AllCity.Id ||
                                            c.City.Id == FindClientObject.City.Id);

                        foreach (var c in list)
                        {
                            Clients.Add(c);
                        }


                        SelectedItem = null;
                    }
                });
            }
        }
        
        private RelayCommand _removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??= new RelayCommand(obj =>
                    {
                        if (obj is Client client)
                        {
                            Clients.Remove(client);
                            Client.AllClients.Remove(client);
                        }
                    },
                    (obj) => Client.AllClients.Count > 0);
            }
        }

        public string Error => "Ошибка ввода данных клиента";

        private Dictionary<string, string> Errors = new Dictionary<string, string>();

        /// <summary>
        /// Error indexer
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
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
                        else if (client != null)
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

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }

    public class FindClientObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
    }
}
