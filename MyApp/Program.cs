using System;
using System.Collections.Generic;
using System.Linq;

class Passenger
{
    public string Name { get; set; }
    public string FlightNumber { get; set; }
    public bool HasTicket { get; set; } = false;
    public bool PassedSecurity { get; set; } = false;
    public bool IsOnBoard { get; set; } = false;
}

class Flight
{
    public string FlightNumber { get; set; }
    public string Destination { get; set; }
    public int DepartureTime { get; set; }
    public string Status { get; set; } = "OnTime"; // OnTime, Boarding, Departed
    public int Capacity { get; set; }
    public List<Passenger> PassengersOnBoard { get; set; } = new List<Passenger>();
}

class Airport
{
    private List<Flight> flights = new List<Flight>();
    private List<Passenger> passengers = new List<Passenger>();
    private Queue<Passenger> registrationQueue = new Queue<Passenger>();
    private Queue<Passenger> securityQueue = new Queue<Passenger>();

    private int time = 0;
    private Random rnd = new Random();

    private int registrationDesks = 3;
    private int securityPoints = 2;
    private int boardingSpeed = 5;

    public Airport()
    {
        flights.Add(new Flight { FlightNumber = "PS101", Destination = "Kyiv", DepartureTime = 5, Capacity = 50 });
        flights.Add(new Flight { FlightNumber = "PS202", Destination = "London", DepartureTime = 8, Capacity = 40 });
        flights.Add(new Flight { FlightNumber = "PS303", Destination = "New York", DepartureTime = 12, Capacity = 60 });
    }

    public void Run()
    {
        Console.WriteLine("Аеропорт-симуляція запущена!");
        Console.WriteLine("Введіть 'help' для списку команд.");

        bool running = true;
        while (running)
        {
            Console.Write("\nКоманда: ");
            string cmd = Console.ReadLine()?.Trim().ToLower();

            switch (cmd)
            {
                case "tick":
                    Tick();
                    break;

                case "add":
                    AddPassenger();
                    break;

                case "status":
                    PrintStatus();
                    break;

                case "help":
                    ShowCommands();
                    break;

                case "exit":
                    running = false;
                    Console.WriteLine("Симуляцію завершено.");
                    break;

                default:
                    Console.WriteLine("Невідома команда. Введіть 'help' для списку команд.");
                    break;
            }
        }
    }

    private void Tick()
    {
        time++;

        // Нові випадкові пасажири
        if (rnd.NextDouble() < 0.5)
        {
            var f = flights[rnd.Next(flights.Count)];
            var p = new Passenger
            {
                Name = "Passenger" + rnd.Next(1000),
                FlightNumber = f.FlightNumber
            };
            passengers.Add(p);
            registrationQueue.Enqueue(p);
            Console.WriteLine($"🧍 Новий пасажир {p.Name} для рейсу {p.FlightNumber}");
        }

        // Реєстрація
        for (int i = 0; i < registrationDesks; i++)
        {
            if (registrationQueue.Count > 0)
            {
                var p = registrationQueue.Dequeue();
                p.HasTicket = true;
                securityQueue.Enqueue(p);
                Console.WriteLine($"✅ {p.Name} зареєструвався на рейс {p.FlightNumber}");
            }
        }

        // Контроль безпеки
        for (int i = 0; i < securityPoints; i++)
        {
            if (securityQueue.Count > 0)
            {
                var p = securityQueue.Dequeue();
                p.PassedSecurity = true;
                Console.WriteLine($"🔒 {p.Name} пройшов контроль безпеки");
            }
        }

        // Рейси
        foreach (var f in flights)
        {
            if (time == f.DepartureTime - 2 && f.Status == "OnTime")
                f.Status = "Boarding";

            if (time == f.DepartureTime && f.Status != "Departed")
            {
                f.Status = "Departed";
                Console.WriteLine($"✈ Рейс {f.FlightNumber} до {f.Destination} вилетів!");
                foreach (var p in f.PassengersOnBoard)
                    passengers.Remove(p);
            }

            if (f.Status == "Boarding")
            {
                var ready = passengers.Where(p => p.FlightNumber == f.FlightNumber && p.HasTicket && p.PassedSecurity && !p.IsOnBoard).Take(boardingSpeed).ToList();
                foreach (var p in ready)
                {
                    p.IsOnBoard = true;
                    f.PassengersOnBoard.Add(p);
                    Console.WriteLine($"🛫 {p.Name} сів на рейс {f.FlightNumber}");
                }
            }
        }

        PrintStatus();
    }

    private void AddPassenger()
    {
        var f = flights[rnd.Next(flights.Count)];
        var p = new Passenger
        {
            Name = "UserPassenger" + rnd.Next(1000),
            FlightNumber = f.FlightNumber
        };
        passengers.Add(p);
        registrationQueue.Enqueue(p);
        Console.WriteLine($"🧍 Доданий пасажир {p.Name} для рейсу {p.FlightNumber}");
    }

    private void PrintStatus()
    {
        Console.WriteLine($"\n⏰ Час: {time}");
        foreach (var f in flights)
        {
            if (f.Status == "OnTime") Console.ForegroundColor = ConsoleColor.Green;
            else if (f.Status == "Boarding") Console.ForegroundColor = ConsoleColor.Yellow;
            else if (f.Status == "Departed") Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"Рейс {f.FlightNumber} -> {f.Destination} | Статус: {f.Status} | Пасажирів на борту: {f.PassengersOnBoard.Count}/{f.Capacity}");
            Console.ResetColor();
        }

        Console.WriteLine($"Черга на реєстрацію: {registrationQueue.Count}");
        Console.WriteLine($"Черга на контроль: {securityQueue.Count}");
        Console.WriteLine($"Очікують у зоні вильоту: {passengers.Count(p => p.HasTicket && p.PassedSecurity && !p.IsOnBoard)}");
    }

    private void ShowCommands()
    {
        Console.WriteLine("\n📜 Доступні команди:");
        Console.WriteLine("tick   - один крок симуляції");
        Console.WriteLine("add    - додати пасажира вручну");
        Console.WriteLine("status - показати стан рейсів і черг");
        Console.WriteLine("help   - показати список команд");
        Console.WriteLine("exit   - вийти з симуляції");
    }
}

class Program
{
    static void Main()
    {
        Airport airport = new Airport();
        airport.Run();
    }
}
