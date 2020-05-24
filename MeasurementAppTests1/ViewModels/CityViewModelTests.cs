using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeasurementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeasurementApp.Models;

namespace MeasurementApp.ViewModels.Tests
{
    [TestClass()]
    public class CityViewModelTests
    {
        [TestMethod()]
        public void AddTest()
        {
            string standart = "Saratov";

            CityViewModel cityViewModel = new CityViewModel();

            cityViewModel.Add(standart);

            var c = cityViewModel.Cities.FirstOrDefault(c => string.Equals(c.Name, standart));

            City.AllCities.Clear();

            Assert.IsTrue(c != null);

        }

        [TestMethod()]
        public void NameNotEmptyTest()
        {
            CityViewModel cityViewModel = new CityViewModel();

            cityViewModel.NameCity = "Moscow";
            bool notEmpty = cityViewModel.NameNotEmpty(cityViewModel.NameCity);

            cityViewModel.NameCity = null;
            bool nullString = !cityViewModel.NameNotEmpty(cityViewModel.NameCity);

            cityViewModel.NameCity = "";
            bool empty = !cityViewModel.NameNotEmpty(cityViewModel.NameCity);

            City.AllCities.Clear();

            Assert.IsTrue(notEmpty && nullString && empty);
        }

        [TestMethod()]
        public void FindTest()
        {
            City.AllCities.Add(new City("NewYork"));
            City.AllCities.Add(new City("Saratov"));

            CityViewModel cityViewModel = new CityViewModel();

            cityViewModel.Find("new");

            int citiesInList = cityViewModel.Cities.Count();

            City.AllCities.Clear();

            Assert.IsTrue(citiesInList == 1);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            CityViewModel cityViewModel = new CityViewModel();

            cityViewModel.Cities.Add(new City("NewYork"));
            cityViewModel.Cities.Add(new City("Saratov"));

            bool isTwoCity = cityViewModel.Cities.Count() == 2;

            cityViewModel.SelectedItem = cityViewModel.Cities.FirstOrDefault(c => string.Equals(c.Name, "NewYork"));

            if (cityViewModel.SelectedItem != null)
                cityViewModel.Remove(cityViewModel.SelectedItem);

            bool isNoNewYork = cityViewModel.Cities.FirstOrDefault(c => string.Equals(c.Name, "NewYork")) == null;

            bool isOneCity = cityViewModel.Cities.Count() == 1;

            cityViewModel.SelectedItem = null;
            City.AllCities.Clear();

            Assert.IsTrue(isTwoCity && isNoNewYork && isOneCity);
        }

        [TestMethod()]
        public void IsSelectedItemTest()
        {
            CityViewModel cityViewModel = new CityViewModel();

            cityViewModel.Cities.Add(new City("NewYork"));

            cityViewModel.SelectedItem = cityViewModel.Cities.FirstOrDefault(c => string.Equals(c.Name, "NewYork"));

            bool isSelected = string.Equals(cityViewModel?.SelectedItem?.Name, "NewYork");

            cityViewModel.SelectedItem = null;
            City.AllCities.Clear();

            Assert.IsTrue(isSelected);
        }

        [TestMethod()]
        public void CityViewModelTest()
        {
            City.AllCities.Add(new City("NewYork"));

            CityViewModel cityViewModel = new CityViewModel();

            bool isNewYork = cityViewModel.Cities.FirstOrDefault(c => string.Equals(c.Name, "NewYork")) != null;

            bool isOneCity = cityViewModel.Cities.Count() == 1;

            City.AllCities.Clear();

            Assert.IsTrue(isNewYork && isOneCity);
        }
    }
}