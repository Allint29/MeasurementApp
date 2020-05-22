using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using MeasurementApp.Annotations;
using MeasurementApp.Models;
using MeasurementApp.Tools;

namespace MeasurementApp.ViewModels
{
    class MeasurementViewModel : INotifyPropertyChanged
    {
        //public ObservableCollection<Measurement> Measurements { get; set; }

        public ObservableCollection<Measurement> MeasurementsWithoutDate { get; set; }

        public ObservableCollection<Measurement> MeasurementsWithDate { get; set; }

        public ObservableCollection<Measurement> MeasurementsWithDateReadyData { get; set; }

        public ObservableCollection<DateTime?> DatesToShow { get; set; }

        public ObservableCollection<City> Cities => Models.City.AllCities;

        public ObservableCollection<City> CitiesForFind { get; set; }

        public City AllCity;

        private City _cityForFind;
        public City CityForFind
        {
            get => _cityForFind;
            set
            {
                _cityForFind = value;
                OnPropertyChanged(nameof(CityForFind));
            }
        }
        

        private DateTime? _selectedDate = DateTime.Today;
        /// <summary>
        /// Хранит дату для замера
        /// </summary>
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        private DateTime? _selectedBeginDate = DateTime.Today;
        /// <summary>
        /// Хранит начальную дату периода выборки заявок с назначенными датами
        /// </summary>
        public DateTime? SelectedBeginDate
        {
            get => _selectedBeginDate;
            set
            {
                if (value > _selectedEndDate)
                    SelectedEndDate = value;

                _selectedBeginDate = value;
                OnPropertyChanged(nameof(SelectedBeginDate));
            }
        }

        private DateTime? _selectedEndDate = DateTime.Today.AddDays(2);
        /// <summary>
        /// Хранит конечную дату периода выборки заявок с назначенными датами
        /// </summary>
        public DateTime? SelectedEndDate
        {
            get => _selectedEndDate;
            set
            {
                if (value < _selectedBeginDate)
                    SelectedBeginDate = value;

                _selectedEndDate = value;
                OnPropertyChanged(nameof(SelectedEndDate));
            }
        }

        //--------------------------------------------------------------

        //список клиентов города
        public ObservableCollection<Client> Clients { get; set; }

        private string _lastName;
        //поиск по фамилии
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        //список лимитов на город
        public ObservableCollection<MeasurementLimit> Limits { get; set; }

        public MeasurementViewModel()
        {
            AllCity = new City("Все города");
            CityForFind = City.AllCities.FirstOrDefault();

            CitiesForFind = new ObservableCollection<City>();
            CitiesForFind.Insert(0, AllCity);

            foreach (var c in City.AllCities)
                CitiesForFind.Add(c);

            MeasurementsWithoutDate = new ObservableCollection<Measurement>();
            MeasurementsWithDate = new ObservableCollection<Measurement>();


            Clients = new ObservableCollection<Client>();
            Limits = new ObservableCollection<MeasurementLimit>();
            DatesToShow = new ObservableCollection<DateTime?>();

            if (CityForFind != null && CityForFind.Id != AllCity.Id)
            {
                foreach (var c in Client.AllClients.Where(c=>c.City.Id == CityForFind.Id))
                    Clients.Add(c);

                foreach (var m in MeasurementLimit.AllMeasurementLimits.Where(m => m.City.Id == CityForFind.Id))
                    Limits.Add(m);

                foreach (var m in Measurement.AllMeasurements.Where(m => m.MeasurementLimit.City.Id == CityForFind.Id))
                {
                    //формирую списки для показа с назначенными замерами датам и без дат
                    if (m.MeasurementDate == null || DateTime.Equals(m.MeasurementDate, DateTime.MinValue))
                        MeasurementsWithoutDate.Add(m);
                    else
                    {
                        MeasurementsWithDate.Add(m);
                        DatesToShow.Add(m.MeasurementDate);
                    }
                }
            }
            else
            {
                foreach (var c in Client.AllClients)
                    Clients.Add(c);

                foreach (var m in MeasurementLimit.AllMeasurementLimits)
                    Limits.Add(m);

                foreach (var m in Measurement.AllMeasurements)
                {
                    //формирую списки для показа с назначенными замерами датам и без дат
                    if (m.MeasurementDate == null || DateTime.Equals(m.MeasurementDate, DateTime.MinValue))
                        MeasurementsWithoutDate.Add(m);
                    else
                    {
                        MeasurementsWithDate.Add(m);
                        DatesToShow.Add(m.MeasurementDate);
                    }
                }
            }

            //при изменении коллекций удаляю связаанные данные о замерах

            City.AllCities.CollectionChanged += AllCitiesOnCollectionChanged;

            Client.AllClients.CollectionChanged += AllClients_CollectionChanged;

            MeasurementLimit.AllMeasurementLimits.CollectionChanged += AllMeasurementLimits_CollectionChanged;

        }

        private void AllMeasurementLimits_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    Limits.Insert(0, e.NewItems[0] as MeasurementLimit);
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    MeasurementLimit mes = e.OldItems[0] as MeasurementLimit;
                    if (mes != null)
                    {
                        for (int i = MeasurementsWithDate.Count() - 1; i >= 0; i--)
                            if (MeasurementsWithDate[i].MeasurementLimit.Id == mes.Id)
                                MeasurementsWithDate.Remove(MeasurementsWithDate[i]);

                        for (int i = MeasurementsWithoutDate.Count() - 1; i >= 0; i--)
                            if (MeasurementsWithoutDate[i].MeasurementLimit.Id == mes.Id)
                                MeasurementsWithoutDate.Remove(MeasurementsWithoutDate[i]);

                        for (int i = Measurement.AllMeasurements.Count() - 1; i >= 0; i--)
                            if (Measurement.AllMeasurements[i].MeasurementLimit.Id == mes.Id)
                                Measurement.AllMeasurements.Remove(Measurement.AllMeasurements[i]);

                        if(Limits.Contains(mes))
                            Limits.Remove(mes);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                    Clients.Insert(1, e.NewItems[0] as Client);
                    Clients.Remove(e.OldItems[0] as Client);
                    break;
            }
        }

        private void AllClients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                    Clients.Insert(0, e.NewItems[0] as Client);
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                    Client client = e.OldItems[0] as Client;
                    if (client != null)
                    {
                        for (int i = MeasurementsWithDate.Count() - 1; i >= 0; i--)
                            if (MeasurementsWithDate[i].Client.Id == client.Id)
                                MeasurementsWithDate.Remove(MeasurementsWithDate[i]);

                        for (int i = MeasurementsWithoutDate.Count() - 1; i >= 0; i--)
                            if (MeasurementsWithoutDate[i].Client.Id == client.Id)
                                MeasurementsWithoutDate.Remove(MeasurementsWithoutDate[i]);

                        for (int i = Measurement.AllMeasurements.Count() - 1; i >= 0; i--)
                            if (Measurement.AllMeasurements[i].Client.Id == client.Id)
                                Measurement.AllMeasurements.Remove(Measurement.AllMeasurements[i]);

                        if(Clients.Contains(client))
                            Clients.Remove(client);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                    Clients.Insert(1, e.NewItems[0] as Client);
                    Clients.Remove(e.OldItems[0] as Client);
                    break;
            }
        }

        /// <summary>
        /// При изменении коллекции городов, обновляю список для поиска городов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllCitiesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CitiesForFind.Clear();

            CitiesForFind.Insert(0, AllCity);

            foreach (var c in City.AllCities)
            {
                CitiesForFind.Add(c);
            }
        }

        //выбранный клиент
        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
            }
        }

        //Выбранный лимит
        private MeasurementLimit _selectedLimit;
        public MeasurementLimit SelectedLimit
        {
            get => _selectedLimit;
            set
            {
                _selectedLimit = value;
                OnPropertyChanged(nameof(SelectedLimit));
            }
        }

        //Выбранный замер без даты
        private Measurement _selectedMeasureWithoutDate;
        public Measurement SelectedMeasureWithoutDate
        {
            get => _selectedMeasureWithoutDate;
            set
            {
                _selectedMeasureWithoutDate = value;
                OnPropertyChanged(nameof(SelectedMeasureWithoutDate));
            }
        }

        //Выбранный замер с датой
        private Measurement _selectedMeasureWithDate;
        public Measurement SelectedMeasureWithDate
        {
            get => _selectedMeasureWithDate;
            set
            {
                _selectedMeasureWithDate = value;
                OnPropertyChanged(nameof(SelectedMeasureWithDate));
            }
        }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedClient != null && SelectedLimit != null)
                    {
                        Measurement mes = new Measurement(SelectedLimit, SelectedClient);
                        //Measurements.Add(mes);
                        MeasurementsWithoutDate.Add(mes);
                        Measurement.AllMeasurements.Add(mes);

                        SelectedLimit = null;
                        SelectedClient = null;
                    }
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
                    //если выбран определенный город, то фильтрую лимиты и клиентов
                    if (CityForFind != null && CityForFind?.Id != AllCity.Id)
                    {
                        Clients.Clear();

                        var listClients =
                            Client.AllClients
                                 .Where(c => string.IsNullOrEmpty(LastName) ||
                                            c.LastName.Contains(LastName, StringComparison.InvariantCultureIgnoreCase))
                                 .Where(c => CityForFind == null || CityForFind.Id == AllCity.Id ||
                                            c.City.Id == CityForFind.Id);

                        foreach (var c in listClients)
                        {
                            Clients.Add(c);
                        }

                        SelectedClient = null;
                        
                        Limits.Clear();

                        var listLimits =
                            MeasurementLimit.AllMeasurementLimits
                                .Where(m => CityForFind == null || CityForFind.Id == AllCity.Id ||
                                            m.City.Id == CityForFind.Id);

                        foreach (var m in listLimits)
                        {
                            Limits.Add(m);
                        }

                        SelectedLimit = null;

                        MeasurementsWithoutDate.Clear();
                        MeasurementsWithDate.Clear();

                        var listMeasure =
                            Measurement.AllMeasurements
                                .Where(m => CityForFind == null || CityForFind.Id == AllCity.Id ||
                                            m.MeasurementLimit.City.Id == CityForFind.Id);

                        foreach (var m in listMeasure)
                        {
                            //формирую списки для показа с назначенными замерами датам и без дат
                            if (m.MeasurementDate == null || DateTime.Equals(m.MeasurementDate, DateTime.MinValue))
                                MeasurementsWithoutDate.Add(m);
                            else
                                MeasurementsWithDate.Add(m);
                        }

                    }
                    //если не выбран определенный город
                    else
                    {
                        MeasurementsWithoutDate.Clear();
                        MeasurementsWithDate.Clear();

                        foreach (var c in Client.AllClients)
                        {
                            Clients.Add(c);
                        }
                        SelectedClient = null;

                        foreach (var m in MeasurementLimit.AllMeasurementLimits)
                        {
                            Limits.Add(m);
                        }
                        SelectedLimit = null;

                        foreach (var m in Measurement.AllMeasurements)
                        {
                            //формирую списки для показа с назначенными замерами датам и без дат
                            if (m.MeasurementDate == null || DateTime.Equals(m.MeasurementDate, DateTime.MinValue))
                                MeasurementsWithoutDate.Add(m);
                            else
                                MeasurementsWithDate.Add(m);
                        }


                    }
                });
            }

        }

        private RelayCommand _nfoWithDateCommand;
        public RelayCommand InfoWithDateCommand
        {
            get
            {
                return _nfoWithDateCommand ??= new RelayCommand(obj =>
                    {
                        if (obj is Measurement mes)
                        {
                            string message =
                                $"Клиент: {mes.Client.LastName} {mes.Client.FirstName}, телефон {mes.Client.PhoneNumber}\n" +
                                $"Город заказа: {mes.MeasurementLimit.City}, дата: {mes.MeasurementDate?.ToString("dd.MM.yyyy")} \n" +
                                $"Время с {mes.MeasurementLimit.BeginHour}:00 по {mes.MeasurementLimit.EndHour}:00\n" +
                                $"Номер заказа: {mes.Id}.";
                            MessageBox.Show(message);
                            SelectedMeasureWithDate = null;
                        }
                    },
                    (obj) => SelectedMeasureWithDate != null);
            }
        }


        private RelayCommand _findReadyDataCommand;
        public RelayCommand FindReadyDataCommand
        {
            get
            {
                return _findReadyDataCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedBeginDate != null && SelectedEndDate != null)
                    {
                        if (CityForFind != null && CityForFind?.Id != AllCity.Id)
                        {
                            var list = Measurement.AllMeasurements
                                .Where(m => m.MeasurementLimit.City.Id == CityForFind.Id
                                            && m.MeasurementDate >= SelectedBeginDate
                                            && m.MeasurementDate <= SelectedEndDate);

                            MeasurementsWithDate.Clear();

                            foreach (var m in list)
                                MeasurementsWithDate.Add(m);
                        }
                        else
                        {
                            var list = Measurement.AllMeasurements
                                .Where(m => m.MeasurementDate >= SelectedBeginDate
                                            && m.MeasurementDate <= SelectedEndDate);

                            MeasurementsWithDate.Clear();

                            foreach (var m in list)
                                MeasurementsWithDate.Add(m);

                        }

                        SelectedLimit = null;
                    }
                    else
                    {
                        MessageBox.Show("Выберите город, начальную и конечную даты для поиска назначенных замеров.");
                    }
                });
            }
        }

        private RelayCommand _setDateCommand;
        public RelayCommand SetDateCommand
        {
            get
            {
                return _setDateCommand ??= new RelayCommand(obj =>
                {
                    if (SelectedDate != null && SelectedMeasureWithoutDate != null)
                    {
                        if (SelectedMeasureWithoutDate.SetDateForMeasurement(SelectedDate))
                        {
                            MeasurementsWithDate.Add(SelectedMeasureWithoutDate);
                            MeasurementsWithoutDate.Remove(SelectedMeasureWithoutDate);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Для установки даты замера нужно выбрать дату и заявку на замер.");
                    }
                });
            }
        }

        private RelayCommand _removeWithOutDateCommand;
        public RelayCommand RemoveWithOutDateCommand
        {
            get
            {
                return _removeWithOutDateCommand ??= new RelayCommand(obj =>
                    {
                        if (obj is Measurement mes)
                        {
                            //Measurements.Remove(mes);
                            MeasurementsWithoutDate.Remove(mes);
                            Measurement.AllMeasurements.Remove(mes);
                        }
                    },
                    (obj) => MeasurementsWithoutDate.Count > 0);
            }
        }

        private RelayCommand _removeWithDateCommand;
        public RelayCommand RemoveWithDateCommand
        {
            get
            {
                return _removeWithDateCommand ??= new RelayCommand(obj =>
                    {
                        if (SelectedMeasureWithDate != null)
                        {
                            if (SelectedMeasureWithDate.SetDateForMeasurement(null))
                            {
                                MeasurementsWithoutDate.Add(SelectedMeasureWithDate);
                                MeasurementsWithDate.Remove(SelectedMeasureWithDate);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Для установки даты замера нужно выбрать дату и заявку на замер.");
                        }
                    },
                    (obj) => MeasurementsWithDate.Count > 0);
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
