
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

public class Figure
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Figure(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileIO
{
    private string filePath;

    public FileIO(string filePath)
    {
        this.filePath = filePath;
    }

    public Figure LoadFigure()
    {
        string extension = Path.GetExtension(filePath);

        switch (extension)
        {
            case ".txt":
                return LoadFromTxtFile();
            case ".json":
                return LoadFromJsonFile();
            case ".xml":
                return LoadFromXmlFile();
            default:
                throw new NotSupportedException("Неподдерживаемый формат файла.");
        }
    }

    public void SaveFigure(Figure figure)
    {
        string extension = Path.GetExtension(filePath);

        switch (extension)
        {
            case ".txt":
                SaveToTxtFile(figure);
                break;
            case ".json":
                SaveToJsonFile(figure);
                break;
            case ".xml":
                SaveToXmlFile(figure);
                break;
            default:
                throw new NotSupportedException("Неподдерживаемый формат файла.");
        }
    }

    private Figure LoadFromTxtFile()
    {
        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length != 3)
            throw new InvalidDataException("Неподдерживаемый формат файла.");

        string name = lines[0];
        int width = int.Parse(lines[1]);
        int height = int.Parse(lines[2]);

        return new Figure(name, width, height);
    }

    private Figure LoadFromJsonFile()
    {
        string json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<Figure>(json);
    }

    private Figure LoadFromXmlFile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure));

        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            return (Figure)serializer.Deserialize(stream);
        }
    }

    private void SaveToTxtFile(Figure figure)
    {
        string[] lines = { figure.Name, figure.Width.ToString(), figure.Height.ToString() };

        File.WriteAllLines(filePath, lines);
    }

    private void SaveToJsonFile(Figure figure)
    {
        string json = JsonSerializer.Serialize(figure);

        File.WriteAllText(filePath, json);
    }

    private void SaveToXmlFile(Figure figure)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure));

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, figure);
        }
    }
}

public class Program
{
    static void Main()
    {
        Console.WriteLine("Введите путь к файлу:");
        string filePath = Console.ReadLine();

        FileIO fileIO = new FileIO(filePath);

        try
        {
            Figure figure = fileIO.LoadFigure();

            Console.WriteLine($"Прочитанная фигура: {figure.Name}, {figure.Width}x{figure.Height}");


            Console.WriteLine("Нажмите F1 для сохранения файла или Escape для выхода.");

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.F1)
                {
                    fileIO.SaveFigure(figure);
                    Console.WriteLine("Файл сохранен.");
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}