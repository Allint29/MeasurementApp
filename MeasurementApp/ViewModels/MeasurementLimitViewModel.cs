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
    class MeasurementLimitViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public ObservableCollection<MeasurementLimit> MeasurementLimits { get; set; }

        public ObservableCollection<City> Cities => Models.City.AllCities;

        public ObservableCollection<City> CitiesForFind { get; set; }

        public City AllCity;

        public MeasurementLimitViewModel()
        {
            AllCity = new City("Все города");
            CityForFind = new City("Для поиска в лимитах");

            CitiesForFind = new ObservableCollection<City>();
            CitiesForFind.Insert(0, AllCity);

            foreach (var c in City.AllCities)
            {
                CitiesForFind.Add(c);
            }

            MeasurementLimits = new ObservableCollection<MeasurementLimit>();
            foreach (var m in MeasurementLimit.AllMeasurementLimits)
            {
                MeasurementLimits.Add(m);
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
                        for (int i = MeasurementLimits.Count() - 1; i >= 0; i--)
                            if (MeasurementLimits[i].City.Id == city.Id)
                                MeasurementLimits.Remove(MeasurementLimits[i]);

                        for (int i = MeasurementLimit.AllMeasurementLimits.Count() - 1; i >= 0; i--)
                            if (MeasurementLimit.AllMeasurementLimits[i].City.Id == city.Id)
                                MeasurementLimit.AllMeasurementLimits.Remove(MeasurementLimit.AllMeasurementLimits[i]);

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

        private MeasurementLimit _selectedItem;
        public MeasurementLimit SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        private int _beginHour = 8;

        /// <summary>
        /// начало периода
        /// </summary>
        [Required]
        [DisplayName("Начало периода")]
        public int BeginHour
        {
            get => _beginHour;
            set
            {
                _beginHour = value;
                OnPropertyChanged(nameof(BeginHour));
            }
        }

        private int _endHour = 20;

        /// <summary>
        /// Конец периода
        /// </summary>
        [Required]
        [DisplayName("Конец периода")]
        public int EndHour
        {
            get { return _endHour; }
            set
            {
                _endHour = value;
                OnPropertyChanged(nameof(BeginHour));
            }

        }

        private int _limit = 10;
        /// <summary>
        /// Количество замеров в период
        /// </summary>
        [Required]
        [DisplayName("Ограничение на замеры")]
        public int Limit
        {
            get { return _limit; }
            set
            {
                _limit = value;
                OnPropertyChanged(nameof(Limit));
            }
        }

        private City _city = Models.City.AllCities.FirstOrDefault();

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

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(obj =>
                {
                    if (Errors.Count < 1)
                    {
                        var limit = new MeasurementLimit(BeginHour, EndHour, Limit, City);

                        MeasurementLimit.AllMeasurementLimits.Insert(0, limit);

                        MeasurementLimits.Clear();
                        foreach (var c in MeasurementLimit.AllMeasurementLimits)
                        {
                            MeasurementLimits.Add(c);
                        }

                        SelectedItem = limit;
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

        private RelayCommand _findCommand;
        public RelayCommand FindCommand
        {
            get
            {
                return _findCommand ??= new RelayCommand(obj =>
                {
                    MeasurementLimits.Clear();

                    var list =
                        MeasurementLimit.AllMeasurementLimits
                            .Where(m => CityForFind == null || CityForFind.Id == AllCity.Id ||
                                        m.City.Id == CityForFind.Id);

                    foreach (var m in list)
                    {
                        MeasurementLimits.Add(m);
                    }
                    
                    SelectedItem = null;

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
                        if (obj is MeasurementLimit limit)
                        {
                            MeasurementLimits.Remove(limit);
                            MeasurementLimit.AllMeasurementLimits.Remove(limit);
                        }
                    },
                    (obj) => MeasurementLimit.AllMeasurementLimits.Count > 0);
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
                    case "BeginHour":

                        if(this.City == null)
                            error = error + "Сначала нужно выбрать город.\n";

                        var mesLimBegin =
                            MeasurementLimit.AllMeasurementLimits.FirstOrDefault(
                                m =>
                                {
                                    if (m.CityId == this.City?.Id)
                                    {
                                        //Если время между периодом уже существующего лимита
                                        if (m.BeginHour <= BeginHour && m.EndHour >= BeginHour)
                                            return true;
                                    }

                                    return false;

                                });

                        if (mesLimBegin != null)
                            error = error + "В данном городе есть действующий лимит на это время. Измените его.\n";

                        if (BeginHour < 8)
                            error = error + "Начала периода не может быть ранее 8 часов. \n";

                        if (BeginHour >= EndHour)
                            error = error + "Начала периода не может быть позже или равно конца периода. \n";

                        if (!string.IsNullOrEmpty(error))
                            Errors["BeginHour"] = error;
                        else
                            if (Errors.ContainsKey("BeginHour"))
                                Errors.Remove("BeginHour");

                        break;

                    case "EndHour":

                        if (this.City == null)
                            error = error + "Сначала нужно выбрать город.\n";

                        var mesLimEnd =
                            MeasurementLimit.AllMeasurementLimits.FirstOrDefault(
                                m =>
                                {
                                    if (m.CityId == this.City?.Id)
                                    {
                                        if (m.BeginHour <= EndHour && m.EndHour >= EndHour)
                                            return true;
                                    }

                                    return false;  
                                });
                        if (mesLimEnd != null)
                            error = error + "В данном городе есть действующий лимит на это время. Измените его.\n";

                        if (BeginHour < 8)
                            error = error + "Начала периода не может быть позднее 20 часов. \n";

                        if (BeginHour >= EndHour)
                            error = error + "Начала периода не может быть позже или равно конца периода. \n";

                        if (!string.IsNullOrEmpty(error))
                            Errors["EndHour"] = error;
                        else
                            if (Errors.ContainsKey("EndHour"))
                                Errors.Remove("EndHour");
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
