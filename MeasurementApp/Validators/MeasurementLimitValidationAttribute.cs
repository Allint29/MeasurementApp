using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MeasurementApp.Models;

namespace MeasurementApp.Validators
{
    class MeasurementLimitValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is MeasurementLimit measurement)
            {
                foreach (var m in MeasurementLimit.AllMeasurementLimits)
                {
                    if (m.CityId == measurement.CityId && (measurement.BeginHour >= m.BeginHour && measurement.BeginHour <= m.EndHour || measurement.EndHour >= m.BeginHour && measurement.EndHour <= m.EndHour))
                    {
                        var city = City.AllCities.FirstOrDefault(c => c.Id == measurement.CityId);
                        this.ErrorMessage = $"В данном городе существует период, который перекрывает текущий. Город {city.Name}, Существующий период {m.BeginHour.ToString()} - {m.EndHour.ToString()}";
                        return false;
                    }
                }
                return true;
            }

            this.ErrorMessage = $"Не возможно привести данный объект к типу MeasurementLimit";
            return false;
        }

    }
}
