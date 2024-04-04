using CarLifeSimulation;
using System;

public class Money
{
    private int nominal;
    private int num;

    public Money(int nominal, int num)
    {
        if (nominal < 0)
            throw new ArgumentException("Nominal can't be negative");

        if (num < 0)
            throw new ArgumentException("Number can't be negative");

        this.nominal = nominal;
        this.num = num;
    }

    public void Print()
    {
        Console.WriteLine($"Nominal: {nominal}, Number: {num}");
    }

    public bool CanBuy(int price)
    {
        return price <= nominal * num;
    }

    public int CalculateItems(int price)
    {
        if (price == 0)
            throw new DivideByZeroException("Can't divide by zero");

        try
        {
            return checked((nominal * num) / price);
        }
        catch (DivideByZeroException)
        {
            throw new DivideByZeroException("Can't divide by zero");
        }
        catch (OverflowException)
        {
            throw new OverflowException("Arithmetic operation resulted in an overflow.");
        }
    }

    public int Nominal
    {
        get { return nominal; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Nominal can't be negative");

            nominal = value;
        }
    }

    public int Num
    {
        get { return num; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Number can't be negative");

            num = value;
        }
    }

    public int TotalAmount
    {
        get { return nominal * num; }
    }
}

public class DivideByZeroException : Exception
{
    public DivideByZeroException() : base() { }
    public DivideByZeroException(string message) : base(message) { }
}

public class OverflowException : Exception
{
    public OverflowException() : base() { }
    public OverflowException(string message) : base(message) { }
}

namespace CarLifeSimulation
{
    public delegate void CarEventHandler(object sender, CarEventArgs e);

    public class Car
    {
        private string brand;
        private int fuelLevel;
        private int distanceTraveled;

        public event CarEventHandler OutOfFuel;
        public event CarEventHandler StoppedAtGasStation;
        public event CarEventHandler BrokenDown;

        private Random rnd = new Random();

        public Car(string brand)
        {
            this.brand = brand;
            fuelLevel = 100;
            distanceTraveled = 0;
        }

        public void Drive(int distance)
        {
            for (int i = 0; i < distance; i++)
            {
                if (fuelLevel <= 0)
                {
                    OnOutOfFuel(new CarEventArgs(fuelLevel, distanceTraveled));
                    return;
                }

                fuelLevel -= rnd.Next(5, 15);
                distanceTraveled++;

                if (rnd.Next(1, 100) < 5)
                {
                    OnBrokenDown(new CarEventArgs(fuelLevel, distanceTraveled));
                    return;
                }

                if (rnd.Next(1, 100) < 10)
                {
                    OnStoppedAtGasStation(new CarEventArgs(fuelLevel, distanceTraveled));
                    fuelLevel = 100; 
                }
            }
        }

        protected virtual void OnOutOfFuel(CarEventArgs e)
        {
            Console.WriteLine($"Автомобіль {brand} залишився без пального на {e.DistanceTraveled} км!");
            OutOfFuel?.Invoke(this, e);
        }

        protected virtual void OnStoppedAtGasStation(CarEventArgs e)
        {
            Console.WriteLine($"Автомобіль {brand} зупинився на заправці на {e.DistanceTraveled} км.");
            StoppedAtGasStation?.Invoke(this, e);
        }

        protected virtual void OnBrokenDown(CarEventArgs e)
        {
            Console.WriteLine($"Автомобіль {brand} зламався після {e.DistanceTraveled} км!");
            BrokenDown?.Invoke(this, e);
        }
    }

    public class CarEventArgs : EventArgs
    {
        public int FuelLevel { get; private set; }
        public int DistanceTraveled { get; private set; }

        public CarEventArgs(int fuelLevel, int distanceTraveled)
        {
            FuelLevel = fuelLevel;
            DistanceTraveled = distanceTraveled;
        }
    }

    public abstract class Service
    {
        protected Car car;

        public Service(Car car)
        {
            this.car = car;
        }

        public void On()
        {
            car.OutOfFuel += HandleEvent;
            car.StoppedAtGasStation += HandleEvent;
            car.BrokenDown += HandleEvent;
        }

        public void Off()
        {
            car.OutOfFuel -= HandleEvent;
            car.StoppedAtGasStation -= HandleEvent;
            car.BrokenDown -= HandleEvent;
        }

        public abstract void HandleEvent(object sender, CarEventArgs e);
    }

    public class TowTruck : Service
    {
        public TowTruck(Car car) : base(car) { }

        public override void HandleEvent(object sender, CarEventArgs e)
        {
            if (e.FuelLevel <= 0)
            {
                Console.WriteLine($"Буксир відвезе автомобіль {car} до ближчої заправки.");
            }
            else if (e.FuelLevel > 0 && e.FuelLevel < 50)
            {
                Console.WriteLine($"Буксир відвезе автомобіль {car} до автосервісу.");
            }
            else if (e.DistanceTraveled % 100 == 0)
            {
                Console.WriteLine($"Буксир допоможе автомобілю {car} на {e.DistanceTraveled} км.");
            }
        }
    }

    public class GasStation : Service
    {
        public GasStation(Car car) : base(car) { }

        public override void HandleEvent(object sender, CarEventArgs e)
        {
            if (e.FuelLevel <= 0)
            {
                Console.WriteLine($"Автозаправка надасть пальне автомобілю {car}.");
                car.Drive(10);
            }
        }
    }

    public class Mechanic : Service
    {
        public Mechanic(Car car) : base(car) { }

        public override void HandleEvent(object sender, CarEventArgs e)
        {
            if (e.DistanceTraveled % 50 == 0)
            {
                Console.WriteLine($"Механік перевірить автомобіль {car} на {e.DistanceTraveled} км.");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Lab#10");

        Console.WriteLine("1. Task 1");
        Console.WriteLine("2. Task 2");
        Console.WriteLine("3. Exit");
        Console.Write("Enter your choice: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                try
                {
                    Console.Write("Enter nominal: ");
                    int nominal = int.Parse(Console.ReadLine());

                    Console.Write("Enter number: ");
                    int num = int.Parse(Console.ReadLine());

                    Money money = new Money(nominal, num);

                    money.Print();

                    Console.Write("Enter price to calculate items: ");
                    int price = int.Parse(Console.ReadLine());

                    int items = money.CalculateItems(price);
                    Console.WriteLine($"You can buy {items} items.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (OverflowException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Please enter a valid number.");
                }
                break;
            case "2":
                Console.WriteLine("Моделювання життя автомобіля");

                Car myCar = new Car("BMW");

                TowTruck towTruck = new TowTruck(myCar);
                GasStation gasStation = new GasStation(myCar);
                Mechanic mechanic = new Mechanic(myCar);

                towTruck.On();
                gasStation.On();
                mechanic.On();

                myCar.Drive(200);
                break;

            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}
