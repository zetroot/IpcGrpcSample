using System;
using System.Threading.Tasks;

namespace IpcGrpcSample.CoreClient
{
    class Program
    {
        static ExtractorClient extractor = new ExtractorClient();
        static ThermocyclerClient thermocycler = new ThermocyclerClient();

        static async Task Main(string[] args)
        {
            

            while (true)
            {
                Console.WriteLine("0 - Экстрактор, 1 - термоциклер, 2 - выход. Ваш выбор?");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "0":
                        var result = await extractor.StartAsync();
                        if (result) 
                            Console.WriteLine("Успешный запуск"); 
                        else 
                            Console.WriteLine("Провал запуска");
                        break;
                    case "1":
                        await ThermocyclerStartAsync();
                        break;
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Выбор не распознан");
                        break;
                }
            }
        }

        private static async Task ThermocyclerStartAsync()
        {
            Console.Write("Название эксперимента:");
            var experimentName = Console.ReadLine();

            Console.Write("Количество циклов:");
            var cyclesString = Console.ReadLine();
            if(int.TryParse(cyclesString, out int cyclesCount))
            {
                var datastream = thermocycler.StartAsync(experimentName, cyclesCount);
                await foreach(var chunk in datastream)
                    chunk.Switch(s => Console.WriteLine(s), temp => Console.WriteLine($"Температура {temp}"));
            }
        }
    }
}
