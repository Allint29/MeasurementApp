using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeasurementApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeasurementApp.Models.Tests
{
    [TestClass()]
    public class MeasurementTests
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
        /// Тест на превышение лимита замеров на дату в одном городе
        /// </summary>
        [TestMethod()]
        public void SetDateForMeasurementTest()
        {
            var city1 = new City("Moscow");

            City.AllCities.Add(city1);

            var client1 = new Client("Ivanov", "Ivan", 9271112233, city1, "Vernadskogo, 23");
            var client2 = new Client("Petrov", "Petr", 9271112232, city1, "Vernadskogo, 24");
            var client3 = new Client("Alexandrov", "Alex", 9271112231, city1, "Vernadskogo, 25");
            var client4 = new Client("Shevelev", "Turk", 9271112230, city1, "Vernadskogo, 26");

            Client.AllClients.Add(client1);
            Client.AllClients.Add(client2);
            Client.AllClients.Add(client3);
            Client.AllClients.Add(client4);

            //три лимита на данный период
            var measLimit1 = new MeasurementLimit(8, 20, 3, city1);

            MeasurementLimit.AllMeasurementLimits.Add(measLimit1);

            //устанавливаю заявки на замеры в городе
            var meas1 = new Measurement(measLimit1, client1, null);
            var meas2 = new Measurement(measLimit1, client2, null);
            var meas3 = new Measurement(measLimit1, client3, null);
            var meas4 = new Measurement(measLimit1, client4, null);

            Measurement.AllMeasurements.Add(meas1);
            Measurement.AllMeasurements.Add(meas2);
            Measurement.AllMeasurements.Add(meas3);
            Measurement.AllMeasurements.Add(meas4);
            //устанавливаю даты на замер

            bool isGoodInstance1 = meas1.SetDateForMeasurement(DateTime.Today, false);
            bool isGoodInstance2 = meas2.SetDateForMeasurement(DateTime.Today, false);
            bool isGoodInstance3 = meas3.SetDateForMeasurement(DateTime.Today, false);

            //лимиты исчерпаны на данную дату в данном городе - следующая установка даты не пройдет
            bool isBadInstance1 = !meas4.SetDateForMeasurement(DateTime.Today, false);

            ClearData();

            Assert.IsTrue(isGoodInstance1 && isGoodInstance2 && isGoodInstance3 && isBadInstance1);
        }
    }
}