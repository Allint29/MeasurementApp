using MeasurementApp.Annotations;
using MeasurementApp.Models;
using MeasurementApp.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace MeasurementApp.ViewModels
{
    public class CityViewModel: INotifyPropertyChanged
    {
        public RelayCommand AddCommand { get; set; }

        public Action<object> OnAdd() => o => Add(_nameCity);

        public void Add(string name)
        {

            if (!string.IsNullOrEmpty(name))
            {
                City city = new City(name);
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

                    SelectedItem = null;
                    NameCity = null;
                }
            }
        }

        public Func<object, bool> CanAdd()=> o => NameNotEmpty(NameCity);
        public bool NameNotEmpty(string name) => !string.IsNullOrEmpty(NameCity);


        public RelayCommand FindCommand { get; set; }
        public Action<object> OnFind() => o => Find(NameCity);
        public void Find(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {

                Cities.Clear();
                foreach (var c in City.AllCities)
                {
                    if (c.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                        Cities.Add(c);
                }
            }
            else
            {
                Cities.Clear();
                foreach (var c in City.AllCities)
                {
                    Cities.Add(c);
                }
            }
            SelectedItem = null;
        }

        public RelayCommand RemoveCommand { get; set; }
        public Action<object> OnRemove() => o => Remove(SelectedItem);
        public void Remove(City city)
        {
            Cities.Remove(city);
            City.AllCities.Remove(city);

            SelectedItem = null;
        }

        public Func<object, bool> CanRemove()
        {
            return o => IsSelectedItem();
        }

        public bool IsSelectedItem() => SelectedItem != null && City.AllCities.Count > 0;

        public ObservableCollection<City> Cities { get; set; }

        public CityViewModel()
        {
            Cities = new ObservableCollection<City>();
            foreach (var c in City.AllCities)
            {
                Cities.Add(c);
            }

            AddCommand = new RelayCommand(OnAdd(), CanAdd());
            FindCommand = new RelayCommand(OnFind());
            RemoveCommand = new RelayCommand(OnRemove(), CanRemove());
        }

        private City _selectedItem;
        public City SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }


        private string _nameCity;
        [Required]
        public string NameCity
        {
            get => _nameCity;
            set
            {
                _nameCity = value;
               
                OnPropertyChanged(nameof(NameCity));
                AddCommand.RaiseCanExecuteChanged();
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
