using ParkingSystem;
using System;
using System.Globalization;

namespace SpaceshipParkingUnitTest
{
    public class UnitTest1
    {
        SpaceshipParkingCost parkingHouse = new SpaceshipParkingCost(3, 15, 15, 50);

        [Theory]
        [InlineData(0,"ABS200")]
        [InlineData(46, "ABS200")]
        public void TestRegisterParkingInvalidSpot(int parkingSpot, string registrationNr)
        {
            ParkingRegistrationData parkData = new ParkingRegistrationData(parkingSpot, registrationNr, DateTime.Now);

            var exception = Assert.Throws<ArgumentException>(() => parkingHouse.RegisterParking(parkData));

            Assert.Equal("Invalid parking spot number.", exception.Message);
        }

        [Fact]
        public void TestRegisterParkingUnavailableSpot()
        {
            ParkingRegistrationData parkData1 = new ParkingRegistrationData(1, "CHA200", DateTime.Now);
            ParkingRegistrationData parkData2 = new ParkingRegistrationData(1, "ABS200", DateTime.Now);

            parkingHouse.RegisterParking(parkData1);
            var exception = Assert.Throws<ArgumentException>(() => parkingHouse.RegisterParking(parkData2));

            Assert.Equal("Parking spot already taken.", exception.Message);
        }

        [Fact]
        public void TestRegisterParkingAlreadyParked()
        {
            ParkingRegistrationData parkData1 = new ParkingRegistrationData(1, "ABS200", DateTime.Now);
            ParkingRegistrationData parkData2 = new ParkingRegistrationData(5, "ABS200", DateTime.Now);

            parkingHouse.RegisterParking(parkData1);
            var exception = Assert.Throws<ArgumentException>(() => parkingHouse.RegisterParking(parkData2));

            Assert.Equal("Spaceship is already registered as parked elsewhere.", exception.Message);
        }

        [Theory]
        [InlineData(1, "ABS200")]
        [InlineData(30, "ABS200")]
        [InlineData(45, "ABS200")]
        public void TestRegisterParking(int parkingSpot, string registrationNr)
        {
            DateTime dateTime = DateTime.Now;
            ParkingRegistrationData parkData = new ParkingRegistrationData(parkingSpot, registrationNr, dateTime);

            parkingHouse.RegisterParking(parkData);
            ParkingRegistrationData actualData = parkingHouse.SpaceshipParkingLot[parkingSpot-1];

            Assert.Equal(parkingSpot, actualData.ParkingSpot);
            Assert.Equal(registrationNr, actualData.RegistrationNumber);
            Assert.Equal(dateTime, actualData.RegistrationDate);
        }

        [Theory]
        [InlineData(0, "ABS200")]
        [InlineData(46, "ABS200")]
        public void TestRegisterPickupInvalidSpot(int parkingSpot, string registrationNr)
        {
            ParkingRegistrationData pickupData = new ParkingRegistrationData(parkingSpot, registrationNr, DateTime.Now);

            var exception = Assert.Throws<ArgumentException>(() => parkingHouse.RegisterPickup(pickupData));

            Assert.Equal("Invalid parking spot number.", exception.Message);
        }

        [Fact]
        public void TestRegisterPickupEmptySpot()
        {
            ParkingRegistrationData pickupData = new ParkingRegistrationData(20, "ABS200", DateTime.Now);

            var exception = Assert.Throws<ArgumentException>(() => parkingHouse.RegisterPickup(pickupData));

            Assert.Equal("There is no spaceship to pickup on this parking spot.", exception.Message);
        }

        [Fact]
        public void TestRegisterPickupAlreadyPickedUp()
        {
            ParkingRegistrationData parkedData = new ParkingRegistrationData(2, "TAH100", DateTime.Now);
            parkingHouse.RegisterParking(parkedData);
            ParkingRegistrationData pickupData = new ParkingRegistrationData(2, "TAH100", DateTime.Now);
            parkingHouse.RegisterPickup(pickupData);

            var exception = Assert.Throws<ArgumentException>(() => parkingHouse.RegisterPickup(pickupData));

            Assert.Equal("There is no spaceship to pickup on this parking spot.", exception.Message);
        }

        [Fact]
        public void TestRegisterPickupWrongRegistrationNr()
        {
            ParkingRegistrationData parkedData = new ParkingRegistrationData(2, "TAH100", DateTime.Now);
            parkingHouse.RegisterParking(parkedData);
            ParkingRegistrationData pickupData = new ParkingRegistrationData(2, "ABS200", DateTime.Now);

            var exception = Assert.Throws<ArgumentException>(() => parkingHouse.RegisterPickup(pickupData));

            Assert.Equal("There is no spaceship with this registration number on this parking spot.", exception.Message);
        }

        [Theory]
        [InlineData(1, "ABS200", "2024-04-02 09:00:00", "2024-04-02 08:00:00", 15)]
        [InlineData(2, "200ABS", "2024-04-02 09:00:00", "2024-04-02 08:30:00", 7.5)]
        [InlineData(10, "H2h103", "2024-04-03 08:00:00", "2024-04-02 08:00:00", 360)] 
        [InlineData(15, "10H2W1", "2024-04-03 08:00:01", "2024-04-02 08:00:00", 50)]
        [InlineData(20, "10H2W1", "2024-04-05 08:00:00", "2024-04-02 08:00:00", 150)]
        [InlineData(25, "10H2W1", "2024-04-02 08:00:00", "2024-04-02 08:00:00", 0)]
        public void TestRegisterPickup(int parkingSpot, string registrationNr, string newTime, string oldTime, double expectedCost)
        {
            DateTime parkedTime = DateTime.ParseExact(oldTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime pickupTime = DateTime.ParseExact(newTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ParkingRegistrationData parkedData = new ParkingRegistrationData(parkingSpot, registrationNr, parkedTime);
            ParkingRegistrationData pickupData = new ParkingRegistrationData(parkingSpot, registrationNr, pickupTime);
            
            parkingHouse.RegisterParking(parkedData);

            Assert.Equal(expectedCost, parkingHouse.RegisterPickup(pickupData));
        }

        [Fact]
        public void TestPickupSpaceshipAmongMany()
        {
            ParkingRegistrationData parkedData1 = new ParkingRegistrationData(1, "TAH200", new DateTime(2024, 4, 2, 14, 0, 0));
            ParkingRegistrationData parkedData2 = new ParkingRegistrationData(2, "ABS200", new DateTime(2024, 4, 2, 10, 30, 0));
            ParkingRegistrationData parkedData3 = new ParkingRegistrationData(3, "TAH100", new DateTime(2024, 4, 2, 12, 0, 0));
            ParkingRegistrationData parkedData4 = new ParkingRegistrationData(4, "10H2W1", new DateTime(2024, 4, 2, 8, 0, 0));
            parkingHouse.RegisterParking(parkedData1);
            parkingHouse.RegisterParking(parkedData2);
            parkingHouse.RegisterParking(parkedData3);
            parkingHouse.RegisterParking(parkedData4);

            ParkingRegistrationData pickupData = new ParkingRegistrationData(3, "TAH100", new DateTime(2024, 4, 2, 15, 0, 0));

            Assert.Equal(45, parkingHouse.RegisterPickup(pickupData));

        }
    }

}