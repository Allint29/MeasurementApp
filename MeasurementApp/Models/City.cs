using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using MeasurementApp.Annotations;
using MeasurementApp.Validators;
using Microsoft.Win32.SafeHandles;

namespace MeasurementApp.Models
{
    [CityValidation]
    public class City: IDisposable, INotifyPropertyChanged, IValidatableObject
    {
        public int Id { get; private set; }

        private static object locker;

        private static int _id;

        private string _name;
        /// <summary>
        /// Название города
        /// </summary>
        [Required]
        [DisplayName("Название города")]
        public string Name 
        {
            get
            {
                return _name;
            }
            set
            {
                _name =  value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Список всех городов
        /// </summary>
        public static ObservableCollection<City> AllCities { get; }

        /// <summary>
        /// Статический конструктор для инициализации всех городов
        /// </summary>
        static City()
        {
            locker = new object();
            _id = 0;
            AllCities = new ObservableCollection<City>();
        }

        public City()
        {
            Id = ++_id;
        }

        public City(string name): this()
        {
            Name = name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
        
            foreach (var сity in City.AllCities)
            {
                if  (сity.Id != Id && сity.Name == Name)
                {
                    errors.Add(new ValidationResult($"Город с именем \"{Name}\" уже существует"));
                }
            }
        
            return errors;
        }

        
        private bool _disposed = false;

        private readonly SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            if(AllCities.Contains(this))
                AllCities.Remove(this);

            Dispose(true);
            GC.SuppressFinalize(this);

        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                _safeHandle?.Dispose();
            }

            _disposed = true;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
