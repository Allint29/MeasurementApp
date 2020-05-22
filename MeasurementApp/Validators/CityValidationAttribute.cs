using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MeasurementApp.Models;

namespace MeasurementApp.Validators
{
    class CityValidationAttribute: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is City city)
            {
                foreach (var c in City.AllCities)
                {
                    if (c.Name == city.Name && city.Id != c.Id)
                    {
                        //var city = City.AllCities.FirstOrDefault(c => c.Id == mesurement.CityId);
                        this.ErrorMessage = $"Город с таким именем уже существует";
                        return false;
                    }
                }
                return true;
            }

            this.ErrorMessage = $"Не возможно привести данный объект к типу City";
            return false;
        }

    }
}
