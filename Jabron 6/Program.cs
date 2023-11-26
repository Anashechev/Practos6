using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;

public class Figure
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Figure()
    {
    }

    public Figure(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileManager
{
    private string filePath;
    private string saveFilePath;

    public FileManager(string filePath, string saveFilePath)
    {
        this.filePath = filePath;
        this.saveFilePath = saveFilePath;
    }

    private string GetFileExtension(string path)
    {
        return Path.GetExtension(path).ToLower();
    }

    public Figure LoadFile()
    {
        Figure figure = null;

        string fileExtension = GetFileExtension(filePath);

        switch (fileExtension)
        {
            case ".txt":
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length == 3)
                {
                    figure = new Figure(lines[0], int.Parse(lines[1]), int.Parse(lines[2]));
                }
                break;
            case ".json":
                string json = File.ReadAllText(filePath);
                figure = JsonConvert.DeserializeObject<Figure>(json);
                break;
            case ".xml":
                XmlSerializer serializer = new XmlSerializer(typeof(Figure));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    figure = (Figure)serializer.Deserialize(reader);
                }
                break;
            default:
                Console.WriteLine("Неподдерживаемый формат файла.");
                break;
        }

        return figure;
    }

    public void SaveFile(Figure figure)
    {
        Console.WriteLine("Выберите формат сохранения файла:");
        Console.WriteLine("1. txt");
        Console.WriteLine("2. json");
        Console.WriteLine("3. xml");

        ConsoleKeyInfo keyInfo = Console.ReadKey();
        Console.WriteLine();

        switch (keyInfo.Key)
        {
            case ConsoleKey.D1:
                SaveAsTxt(figure);
                break;
            case ConsoleKey.D2:
                SaveAsJson(figure);
                break;
            case ConsoleKey.D3:
                SaveAsXml(figure);
                break;
            default:
                Console.WriteLine("Неправильный выбор формата сохранения.");
                break;
        }
    }

    private void SaveAsTxt(Figure figure)
    {
        string[] lines = { figure.Name, figure.Width.ToString(), figure.Height.ToString() };
        File.WriteAllLines(saveFilePath, lines);
    }

    private void SaveAsJson(Figure figure)
    {
        string json = JsonConvert.SerializeObject(figure);
        File.WriteAllText(saveFilePath, json);
    }

    private void SaveAsXml(Figure figure)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure));
        using (StreamWriter writer = new StreamWriter(saveFilePath))
        {
            serializer.Serialize(writer, figure);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь к файлу:");
        string filePath = Console.ReadLine();

        Console.WriteLine("Введите путь для сохранения файла:");
        string saveFilePath = Console.ReadLine();

        FileManager fileManager = new FileManager(filePath, saveFilePath);
        Figure figure = fileManager.LoadFile();

        if (figure != null)
        {
            Console.WriteLine($"Название: {figure.Name}");
            Console.WriteLine($"Ширина: {figure.Width}");
            Console.WriteLine($"Высота: {figure.Height}");
        }

        Console.WriteLine("Нажмите F1 для сохранения файла или Escape для выхода.");

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.F1)
            {
                fileManager.SaveFile(figure);
                Console.WriteLine("Файл сохранен.");
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}
