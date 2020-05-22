using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using MeasurementApp.Annotations;
using MeasurementApp.Validators;

namespace MeasurementApp.Models
{
    [MeasurementLimitValidation]
    class MeasurementLimit: IDisposable, INotifyPropertyChanged, IDataErrorInfo, IValidatableObject
    {
        
        private static int _id;

        /// <summary>
        /// Список всех лимитов
        /// </summary>
        public static ObservableCollection<MeasurementLimit> AllMeasurementLimits { get; }

        static MeasurementLimit()
        {
            _id = 0;
            AllMeasurementLimits = new ObservableCollection<MeasurementLimit>();
        }

        public MeasurementLimit()
        {
            //при создании устанавливаю, что нет используемых лимитов
            //_usedLimitsCount = 0;
            Id = ++_id;
            //AllMeasurementLimits.Add(this);
        }

        public MeasurementLimit(int beginHour, int endHour, int limit, City city) : this()
        {
            _beginHour = beginHour;
            _endHour = endHour;
            _limit = limit;
            _cityId = city.Id;
            _city = city;
        }
        
        public int Id { get; private set; }

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
                var mesLim =
                    MeasurementLimit.AllMeasurementLimits.FirstOrDefault(
                        m =>
                        {
                            if (m.CityId == this.City?.Id)
                            {
                                //Если время между периодом уже существующего лимита
                                if (this.Id != m.Id && (m.BeginHour <= BeginHour && m.EndHour >= BeginHour))// || m.BeginHour <= EndHour && m.EndHour >= EndHour))
                                    return true;
                            }

                            return false;

                        });

                if (mesLim != null)
                    return; 
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

        private int? _cityId;

        /// <summary>
        /// ид города
        /// </summary>
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

        [DisplayName("Город")]
        public City GetCity => City.AllCities.FirstOrDefault(c => c.Id == CityId);

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

                        if (this.City == null)
                            error = error + "Сначала нужно выбрать город.\n";

                        var mesLimBegin =
                            MeasurementLimit.AllMeasurementLimits.FirstOrDefault(
                                m =>
                                {
                                    if (m.CityId == this.City?.Id)
                                    {
                                        //Если время между периодом уже существующего лимита
                                        if (m.Id != Id && m.BeginHour <= BeginHour && m.EndHour >= BeginHour)
                                            return true;
                                    }

                                    return false;

                                });

                        if (mesLimBegin != null )
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
                                        if (m.Id != Id && m.BeginHour <= EndHour && m.EndHour >= EndHour)
                                            return true;
                                    }

                                    return false;
                                });
                        if (mesLimEnd != null)
                            error = error + "В данном городе есть действующий лимит на это время. Измените его.\n";

                        if (EndHour > 20)
                            error = error + "Конец периода не может быть позднее 20 часов. \n";

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

        private bool ModelISValidate()
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            if (!Validator.TryValidateObject(this, context, results, true))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in results)
                {
                    sb.Append(error.ErrorMessage+"\n");
                }

                MessageBox.Show(sb.ToString());
                return false;
            }

            return true;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if (this.City == null)
                errors.Add(new ValidationResult($"В начале нужно выбрать город."));


            var mesLim =
                MeasurementLimit.AllMeasurementLimits.FirstOrDefault(
                    m =>
                    {
                        if (m.CityId == this.City?.Id)
                        {
                            //Если время между периодом уже существующего лимита
                            if (this.Id != m.Id && (m.BeginHour <= BeginHour && m.EndHour >= BeginHour))//;// || m.BeginHour <= EndHour && m.EndHour >= EndHour))
                                return true;
                        }

                        return false;

                    });
            if (mesLim != null ) 
                errors.Add(new ValidationResult($"В данном городе есть действующий лимит на это время.Измените его."));

            if (BeginHour < 8)
                errors.Add(new ValidationResult("Начала периода не может быть ранее 8 часов.")); 

            if (EndHour > 20)
                errors.Add(new ValidationResult("Конец периода не может быть позднее 20 часов."));

            if (BeginHour >= EndHour)
                errors.Add(new ValidationResult("Начала периода не может быть позже или равно конца периода."));

            return errors;
        }


        public void Dispose()
        {
            AllMeasurementLimits.Remove(this);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
