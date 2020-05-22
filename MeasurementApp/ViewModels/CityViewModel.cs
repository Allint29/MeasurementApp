using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using MeasurementApp.Annotations;
using MeasurementApp.Models;
using MeasurementApp.Tools;

namespace MeasurementApp.ViewModels
{
    class CityViewModel: INotifyPropertyChanged
    {

        public ObservableCollection<City> Cities { get; set; }

        public CityViewModel()
        {
            Cities = new ObservableCollection<City>();
            foreach (var c in City.AllCities)
            {
                Cities.Add(c);
            }

        }

        private City _selectedItem;
        public City SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }


        private string _nameCity;
        public string NameCity
        {
            get => _nameCity;
            set
            {
                _nameCity = value;
                OnPropertyChanged(nameof(NameCity));
            }
        }
        

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(obj =>
                {

                    //var cityName = obj as string;
                    var cityName = _nameCity;

                    if (!string.IsNullOrEmpty(cityName))
                    {
                        City city = new City(cityName);
                        var results = new List<ValidationResult>();
                        var context = new ValidationContext(city);
                        if (!Validator.TryValidateObject(city, context, results, true))
                        {
                            foreach (var error in results)
                            {
                                MessageBox.Show(error.ErrorMessage);
                            }
                        }
                        else
                        {
                            City.AllCities.Insert(0, city);
                            Cities.Clear();
                            foreach (var c in City.AllCities)
                            {
                                Cities.Add(c);
                            }

                            SelectedItem = city;
                        }
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

                    var cityName = _nameCity;

                    if (!string.IsNullOrEmpty(cityName))
                    {
                        
                       Cities.Clear();
                       foreach (var c in City.AllCities)
                       {
                           if(c.Name.Contains(cityName, StringComparison.InvariantCultureIgnoreCase))
                            Cities.Add(c);
                       }

                       SelectedItem = null;
                       
                    }
                    else
                    {
                        Cities.Clear();
                        foreach (var c in City.AllCities)
                        {
                            Cities.Add(c);
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
                        if (obj is City city)
                        {
                            Cities.Remove(city);
                            City.AllCities.Remove(city);
                        }
                    },
                    (obj) => City.AllCities.Count > 0);
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
}
