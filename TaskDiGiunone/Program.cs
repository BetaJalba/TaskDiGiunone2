using TaskDiGiunone;

CTerme piscina = new(5);
List<Task> list = new();

Random rand = new Random();

for (int i = 0; i < 50; i++)
{
    list.Add(Task.Run(() => piscina.AccediPiscina(rand.Next(2) == 0 ? true : false)));
}

await Task.WhenAll(list);

Console.ForegroundColor = ConsoleColor.White;