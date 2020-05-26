using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using MeasurementApp.Annotations;

namespace MeasurementApp.Models
{
    /// <summary>
    /// Модель замера может иметь одного клиента и одну дату
    /// </summary>
    public class Measurement : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Уникальный номер
        /// </summary>
        public int Id { get; private set; }

        private static int _id;

        public Client Client { get; private set; }

        public MeasurementLimit MeasurementLimit { get; private set; }


        private DateTime? _measurementDate;
        /// <summary>
        /// Дата замера
        /// </summary>
        public DateTime? MeasurementDate 
        { 
            get=>_measurementDate;
            set
            {
                _measurementDate = value;
                OnPropertyChanged(nameof(_measurementDate));
            }
        }

        public static ObservableCollection<Measurement> AllMeasurements;

        static Measurement()
        {
            _id = 0;
            AllMeasurements = new ObservableCollection<Measurement>();
        }

        public Measurement()
        {
            Id = ++_id;
        }


        public Measurement (MeasurementLimit mesLimit, Client client, DateTime? date = null): this()
        {

            var city = City.AllCities.FirstOrDefault(c => c.Id == mesLimit.CityId);

            if (city == null)
            {
                MessageBox.Show(
                    $"Ошибка при определении города для замера.");
                return;
            }

            Client = client;
            MeasurementLimit = mesLimit;
            MeasurementDate = date;

            //AllMeasurements.Add(newMes);
        }

        /// <summary>
        /// 
        /// Метод присвоения даты замера
        /// </summary>
        /// <param name="date"></param>
        /// <param name="showMessage">если не нужно показывать окно - false</param>
        public bool SetDateForMeasurement(DateTime? date, bool showMessage = true)
        {
            int remainingMeasurementLimit = MeasurementLimit.Limit;

            foreach (var m in AllMeasurements.Where(m => m.MeasurementLimit.Id == MeasurementLimit.Id && m.MeasurementDate == date))
                remainingMeasurementLimit--;

            if (remainingMeasurementLimit < 1)
            {
                if(showMessage)
                    MessageBox.Show($"Количество лимитов на замер в г.{MeasurementLimit.City?.Name} = {remainingMeasurementLimit}");
                return false;
            }

            this.MeasurementDate = date;
            return true;
        }

        public void Dispose()
        {
            if(AllMeasurements.Contains(this))
                AllMeasurements.Remove(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
