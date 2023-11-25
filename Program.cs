using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.Json;

namespace WorkWithFiles
{
    public class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к бинарному файлу базы данных студентов.");
                Environment.Exit(1);  // Выход с кодом ошибки
            }

            string binaryFilePath = args[0];

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string studentsDirectory = Path.Combine(desktopPath, "Students");

                if (!Directory.Exists(studentsDirectory))
                {
                    Directory.CreateDirectory(studentsDirectory);
                }

                DeserializeStudents(binaryFilePath, studentsDirectory);

                Console.WriteLine("Данные успешно загружены и разложены по группам.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                LogError(ex);
                Environment.Exit(1);  // Выход с кодом ошибки
            }
        }

        static void DeserializeStudents(string binaryFilePath, string outputDirectory)
        {
            try
            {
                string jsonString = File.ReadAllText(binaryFilePath);
                List<Student> students = JsonSerializer.Deserialize<List<Student>>(jsonString);

                foreach (var student in students)
                {
                    string groupFilePath = Path.Combine(outputDirectory, $"{student.Group}.txt");
                    using (StreamWriter sw = File.AppendText(groupFilePath))
                    {
                        sw.WriteLine($"{student.Name}, {student.DateOfBirth}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при десериализации: {ex.Message}");
                LogError(ex);
                throw;  // Пробросить исключение для дополнительной отладки
            }
        }

        static void LogError(Exception ex)
        {
            string logFilePath = Path.Combine(AppContext.BaseDirectory, "error_log.txt");
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine($"[Ошибка] {DateTime.Now}: {ex.Message}");
                sw.WriteLine($"[StackTrace]: {ex.StackTrace}");
                sw.WriteLine();
            }
        }
    }
}
    
