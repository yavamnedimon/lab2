using System;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        
        string filePath = "lab2.txt";
        File.Create(filePath).Close();
        StreamWriter sw = new StreamWriter(filePath);
        sw.WriteLine($"hfhjdshj\ndghsajdsa");
        sw.Close();

        var charChannel = Channel.CreateUnbounded<char>();

        
        _ = ReadFileAsync(filePath, charChannel.Writer);

        // преобразование канала символов в канал строк
        var stringChannel = ConvertToLines(charChannel.Reader);

        // Печать строк из канала
        await foreach (var line in stringChannel)
        {
            Console.WriteLine(line);
        }
    }

    static async Task ReadFileAsync(string filePath, ChannelWriter<char> writer)
    {
        try
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    char character = (char)reader.Read();
                    await writer.WriteAsync(character);
                }
            }
        }
        finally
        {
            writer.Complete();
        }
    }

    static async IAsyncEnumerable<string> ConvertToLines(ChannelReader<char> reader)
    {
        var line = string.Empty;

        await foreach (var character in reader.ReadAllAsync())
        {
            if (character == '\n')
            {
                yield return line;
                line = string.Empty;
            }
            else
            {
                line += character;
            }
        }

        
        if (!string.IsNullOrEmpty(line))
        {
            yield return line;
        }
    }
}
