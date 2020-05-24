using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeasurementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MeasurementApp.Models;

namespace MeasurementApp.ViewModels.Tests
{
    [TestClass()]
    public class MeasurementViewModelTests
    {
        /// <summary>
        /// метод очищает все репозитории от созданных теставми данных
        /// </summary>
        private void ClearData()
        {
            Measurement.AllMeasurements.Clear();
            MeasurementLimit.AllMeasurementLimits.Clear();
            Client.AllClients.Clear();
            City.AllCities.Clear();
        }


        /// <summary>
        /// метод проверяет добавление заявки на замер в список нераспределенных заявок - без назначенных дат
        /// </summary>
        [TestMethod()]
        public void AddTest()
        {
            var city = new City("Moscow");
            City.AllCities.Add(city);

            var meas = new MeasurementViewModel();

            var client = new Client("Ivanov", "Ivan", 9271112233, city,"Vernadskogo, 23");
            var measLimit = new MeasurementLimit(8,20,2, city);
            
            meas.Clients.Add(client);
            meas.Limits.Add(measLimit);

            meas.SelectedClient = meas.Clients.FirstOrDefault(c=> c.Id == client.Id);
            meas.SelectedLimit = meas.Limits.FirstOrDefault(l=>l.Id==measLimit.Id) ;

            meas.Add();

            bool isMeasSave = meas.MeasurementsWithoutDate.FirstOrDefault(m => m.Client.Id == client.Id) != null;

            ClearData();

            Assert.IsTrue(isMeasSave);
        }


        [TestMethod()]
        public void IsSelectedClientAndLimitTest()
        {
            var city = new City("Moscow");
            City.AllCities.Add(city);

            var meas = new MeasurementViewModel();

            var client = new Client("Ivanov", "Ivan", 9271112233, city, "Vernadskogo, 23");
            var measLimit = new MeasurementLimit(8, 20, 2, city);

            meas.Clients.Add(client);
            meas.Limits.Add(measLimit);

            meas.SelectedClient = meas.Clients.FirstOrDefault(c => c.Id == client.Id);
            meas.SelectedLimit = meas.Limits.FirstOrDefault(l => l.Id == measLimit.Id);

            bool isSelectedClient = meas.SelectedClient != null;
            bool isSelectedLimit = meas.SelectedLimit != null;

            ClearData();

            Assert.IsTrue(isSelectedClient && isSelectedLimit);
        }

        [TestMethod()]
        public void FindTest()
        {
            var city1 = new City("Moscow");
            var city2 = new City("NewYork");

            City.AllCities.Add(city1);
            City.AllCities.Add(city2);

            var client1 = new Client("Ivanov", "Ivan", 9271112233, city1, "Vernadskogo, 23");
            var client2 = new Client("Shevelev", "Alex", 9271112234, city1, "Vernadskogo, 24");
            var client3 = new Client("Petrov", "Petr", 9271112235, city2, "Vernadskogo, 25");
            
            Client.AllClients.Add(client1);
            Client.AllClients.Add(client2);
            Client.AllClients.Add(client3);

            var measLimit1 = new MeasurementLimit(8, 20, 2, city1);
            var measLimit2 = new MeasurementLimit(8, 20, 2, city2);

            MeasurementLimit.AllMeasurementLimits.Add(measLimit1);
            MeasurementLimit.AllMeasurementLimits.Add(measLimit2);

            var meas = new MeasurementViewModel();

            meas.CityForFind = city1;

            meas.LastName = "";
            meas.Find();

            bool isFindedTwoClientsOneLimit = meas.Clients.Count() == 2 && meas.Limits.Count() == 1;

            meas.LastName = "Ivan";
            meas.Find();

            bool isFindedOneClientOneLimit = meas.Clients.Count() == 1 && meas.Limits.Count() == 1;

            meas.CityForFind = meas.AllCity;
            meas.LastName = "";
            meas.Find();

            bool isFindedThreeClientTwoLimit = meas.Clients.Count() == 3 && meas.Limits.Count() == 2;

            ClearData();

            Assert.IsTrue(isFindedTwoClientsOneLimit && isFindedOneClientOneLimit && isFindedThreeClientTwoLimit);
        }

        [TestMethod()]
        public void IsSelectedMeasurementTest()
        {
            var city1 = new City("Moscow");

            City.AllCities.Add(city1);

            var client1 = new Client("Ivanov", "Ivan", 9271112233, city1, "Vernadskogo, 23");
            
            Client.AllClients.Add(client1);
            
            var measLimit1 = new MeasurementLimit(8, 20, 2, city1);

            MeasurementLimit.AllMeasurementLimits.Add(measLimit1);

            var meas = new MeasurementViewModel();

            meas.SelectedClient = meas.Clients.FirstOrDefault(c => c.Id == client1.Id);
            meas.SelectedLimit = meas.Limits.FirstOrDefault(l => l.Id == measLimit1.Id);

            meas.Add();

            meas.SelectedDate = DateTime.Today;
            meas.SelectedMeasureWithoutDate = meas.MeasurementsWithoutDate.FirstOrDefault();

            meas.SetDate();

            meas.SelectedMeasureWithDate = null;
            meas.SelectedMeasureWithDate = meas.MeasurementsWithDate.FirstOrDefault();

            bool selected = meas.SelectedMeasureWithDate != null;

            ClearData();

            Assert.IsTrue(selected);
        }

        [TestMethod()]
        public void SetDateTest()
        {
            var city1 = new City("Moscow");

            City.AllCities.Add(city1);

            var client1 = new Client("Ivanov", "Ivan", 9271112233, city1, "Vernadskogo, 23");

            Client.AllClients.Add(client1);

            var measLimit1 = new MeasurementLimit(8, 20, 2, city1);

            MeasurementLimit.AllMeasurementLimits.Add(measLimit1);

            var meas = new MeasurementViewModel();

            meas.SelectedClient = meas.Clients.FirstOrDefault(c => c.Id == client1.Id);
            meas.SelectedLimit = meas.Limits.FirstOrDefault(l => l.Id == measLimit1.Id);

            meas.Add();

            bool isNoDate = meas.MeasurementsWithoutDate
                .FirstOrDefault(m => m.Client.Id == client1.Id && (m.MeasurementDate == null || m.MeasurementDate == DateTime.MinValue)) != null 
                            && !meas.MeasurementsWithDate.Any();
            var date = DateTime.Today;

            meas.SelectedDate = date;
            meas.SelectedMeasureWithoutDate = meas.MeasurementsWithoutDate.FirstOrDefault();

            meas.SetDate();

            bool isYesDate = meas.MeasurementsWithDate
                                .FirstOrDefault(m => m.Client.Id == client1.Id && m.MeasurementDate == date) != null
                            && !meas.MeasurementsWithoutDate.Any();
            
            ClearData();

            Assert.IsTrue(isNoDate && isYesDate);
        }

        [TestMethod()]
        public void RemoveWithOutDateTest()
        {

            var city1 = new City("Moscow");

            City.AllCities.Add(city1);

            var client1 = new Client("Ivanov", "Ivan", 9271112233, city1, "Vernadskogo, 23");

            Client.AllClients.Add(client1);

            var measLimit1 = new MeasurementLimit(8, 20, 2, city1);

            MeasurementLimit.AllMeasurementLimits.Add(measLimit1);

            var meas = new MeasurementViewModel();

            meas.SelectedClient = meas.Clients.FirstOrDefault(c => c.Id == client1.Id);
            meas.SelectedLimit = meas.Limits.FirstOrDefault(l => l.Id == measLimit1.Id);

            meas.Add();

            meas.SelectedMeasureWithoutDate = meas.MeasurementsWithoutDate.FirstOrDefault();

            meas.RemoveWithOutDate(meas.SelectedMeasureWithoutDate);

            bool isRemoved = !meas.MeasurementsWithoutDate.Any() && !meas.MeasurementsWithDate.Any();

            ClearData();

            Assert.IsTrue(isRemoved);
        }
        
        [TestMethod()]
        public void RemoveWithDateTest()
        {
            var city1 = new City("Moscow");

            City.AllCities.Add(city1);

            var client1 = new Client("Ivanov", "Ivan", 9271112233, city1, "Vernadskogo, 23");

            Client.AllClients.Add(client1);

            var measLimit1 = new MeasurementLimit(8, 20, 2, city1);

            MeasurementLimit.AllMeasurementLimits.Add(measLimit1);

            var meas = new MeasurementViewModel();

            meas.SelectedClient = meas.Clients.FirstOrDefault(c => c.Id == client1.Id);
            meas.SelectedLimit = meas.Limits.FirstOrDefault(l => l.Id == measLimit1.Id);

            meas.Add();

            var date = DateTime.Today;

            meas.SelectedDate = date;
            meas.SelectedMeasureWithoutDate = meas.MeasurementsWithoutDate.FirstOrDefault();

            meas.SetDate();

            meas.SelectedMeasureWithDate = meas.MeasurementsWithDate.FirstOrDefault(m=>m.MeasurementDate == date);

            bool isAddedToList = meas.SelectedMeasureWithDate != null && !meas.MeasurementsWithoutDate.Any();

            meas.RemoveWithDate(meas.SelectedMeasureWithDate);

            bool isRemovedFromList = !meas.MeasurementsWithDate.Any() && meas.MeasurementsWithoutDate.Any();

            ClearData();

            Assert.IsTrue(isAddedToList && isAddedToList);
        }

        [TestMethod()]
        public void AllMeasurementLimits_CollectionChangedTest()
        {
            var meas = new MeasurementViewModel();

            bool isEmptyMeasLimitList = !meas.Limits.Any();

            MeasurementLimit.AllMeasurementLimits.Add(new MeasurementLimit(8,20,10,new City("Moscow")));

            bool isFullMeasLimitList = meas.Limits.Any();

            ClearData();

            Assert.IsTrue(isEmptyMeasLimitList && isFullMeasLimitList);
        }

        [TestMethod()]
        public void AllCitiesOnCollectionChangedTest()
        {
            var meas = new MeasurementViewModel();

            bool isEmptyCitiesList = !meas.Cities.Any();

            City.AllCities.Add(new City("Moscow"));

            bool isFullCitiesList = meas.Cities.Any();

            ClearData();
            Assert.IsTrue(isEmptyCitiesList && isFullCitiesList);
        }
    }
}