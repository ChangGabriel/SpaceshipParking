using System;
using System.Collections.Generic;
using System.Linq;


namespace ParkingSystem
{
    public class SpaceshipParkingCost
    {
        private const int HOURS_IN_DAY = 24;
        private readonly int totalParkingSpots;
        public ParkingRegistrationData[] SpaceshipParkingLot { get; private set; }
        public double HourlyPrice { get; set; }
        public int DailyPrice { get; set; }

        public SpaceshipParkingCost(int floors, int parkingSpotsPerFloor, double hourlygPrice, int dailyPrice)
        {
            totalParkingSpots = parkingSpotsPerFloor * floors;
            HourlyPrice = hourlygPrice;
            DailyPrice = dailyPrice;
            SpaceshipParkingLot = new ParkingRegistrationData[totalParkingSpots];    
        }
       
        public void RegisterParking(ParkingRegistrationData data)
        {
            int parkingSpot = data.ParkingSpot - 1;

            ValidateParkingSpot(parkingSpot);
            if (SpaceshipParkingLot[parkingSpot] != null)
            {
                throw new ArgumentException("Parking spot already taken.");
            }
            if (SpaceshipParkingLot.Any(pd => pd?.RegistrationNumber == data.RegistrationNumber))
            {
                throw new ArgumentException("Spaceship is already registered as parked elsewhere.");
            }

            SpaceshipParkingLot[parkingSpot] = data;
        }

        public double RegisterPickup(ParkingRegistrationData pickupData) 
        {
            DateTime pickupTime = pickupData.RegistrationDate;
            int parkingSpot = pickupData.ParkingSpot - 1;

            ValidateParkingSpot(parkingSpot);
            if (SpaceshipParkingLot[parkingSpot] == null)
            {
                throw new ArgumentException("There is no spaceship to pickup on this parking spot.");
            }

            ParkingRegistrationData oldParkingData = SpaceshipParkingLot[parkingSpot];
            if (!oldParkingData.RegistrationNumber.Equals(pickupData.RegistrationNumber))
            {
                throw new ArgumentException("There is no spaceship with this registration number on this parking spot.");
            }

            SpaceshipParkingLot[parkingSpot] = null!;
            TimeSpan rentalTime = pickupTime.Subtract(oldParkingData.RegistrationDate);
            if(rentalTime.TotalHours > HOURS_IN_DAY)
            {
                return DailyPrice * rentalTime.Days;
            }
            else
            {
                return HourlyPrice * rentalTime.TotalHours; 
            }
        }

        private void ValidateParkingSpot(int parkingSpot)
        {
            if (parkingSpot < 0 || parkingSpot >= totalParkingSpots)
            {
                throw new ArgumentException("Invalid parking spot number.");
            }
        }
    }

    public class ParkingRegistrationData
    {
        public int ParkingSpot { get; }
        public string RegistrationNumber { get; }
        public DateTime RegistrationDate { get; }

        public ParkingRegistrationData(int parkingSpot, string registrationNumber, DateTime registrationDate)
        {
            ParkingSpot = parkingSpot;
            RegistrationNumber = registrationNumber;
            RegistrationDate = registrationDate;
        }
    }
}