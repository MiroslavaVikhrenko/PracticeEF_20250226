using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace Notes
{
    internal class Program
    {
        /*
         Создайте простое приложение для учета задач. В этом приложении будет одна сущность Task.

1) Реализуйте возможность добавления новой задачи.
2) Реализуйте возможность получения списка всех задач.
3) Добавьте возможность фильтрации задач по статусу.
4) Реализуйте возможность обновления информации о задаче (например, изменение названия, описания или статуса).
5) Реализуйте возможность удаления задачи.

Реализуйте простой консольный интерфейс для взаимодействия с пользователем. 
Добавьте валидацию данных (например, название задачи не должно быть пустым).
         */
        static void Main(string[] args)
        {
            AddNote();
            PrintAllNotes();
            FilterByStatus("New");
            UpdateNote("InProgress", 1);
            DeleteNote(1);
        }

        public static void DeleteNote(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var note = db.Notes.FirstOrDefault(e => e.Id == id);
                if (note != null)
                {
                    db.Notes.Remove(note);
                    db.SaveChanges();
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAFTER UPDATE:");
                Console.ResetColor();
                PrintAllNotes();
            }
        }
        public static void UpdateNote(string newStatus, int id)
        {
            Status st;
            if (Enum.TryParse(newStatus, out st))
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var note = db.Notes.FirstOrDefault(e => e.Id == id);
                    if (note != null)
                    {
                        note.Status = st;
                        db.SaveChanges();
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nAFTER UPDATE:");
                    Console.ResetColor();
                    PrintAllNotes();
                }
            }
            else
            {
                Console.WriteLine("Incorrect status");
            }
        }
        public static void FilterByStatus(string status)
        {
            Status st;
            if (Enum.TryParse(status, out st))
            {
                Console.WriteLine("-----------");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"All notes in {status} status:");
                Console.ResetColor();
                using (ApplicationContext db = new ApplicationContext())
                {
                    var notes = db.Notes.ToList();
                    foreach (Note note in notes)
                    {
                        if (note.Status == st)
                        {
                            Console.WriteLine("-----------");
                            Console.WriteLine(note.ToString());
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Incorrect status");
            }
        }
        public static void PrintAllNotes()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"All notes:");
                Console.ResetColor();
                var notes = db.Notes.ToList();
                foreach (Note note in notes)
                {
                    Console.WriteLine("-----------");
                    Console.WriteLine(note.ToString());
                }
            }
        }
        public static void AddNote()
        {
            Note newNote = GetInput();
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Notes.Add(newNote);
                db.SaveChanges();
            }
        }
        public static Note GetInput()
        {
            while (true)
            {
                Console.WriteLine("Enter note's name:");
                string name = Console.ReadLine();               
                Console.WriteLine("Enter note's description:");
                string desc = Console.ReadLine();
                if (!Helper.ValidateStringInput(name) || !Helper.ValidateStringInput(desc))
                {
                    continue;
                    Console.WriteLine("Incorrect input! Try again");
                }
                else
                {
                    Console.WriteLine("Enter note's status:");
                    string status = Console.ReadLine();
                    if (Helper.IsValidStatus(status))
                    {
                        Status st;
                        if (Enum.TryParse(status, out st))
                        {
                            Note newNote = new Note() { CreatedAt = DateTime.Now, Description = desc, Name = name, Status = st };
                            return newNote;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input! Try again");
                        continue;
                    }
                }          
            }     
        }
    }

    public class Helper
    {
        public static bool IsValidStatus(string input)
        {
            return Enum.TryParse(input, true, out Status _);
        }
        public static bool ValidateStringInput(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return true;
        }
    }

    public class Note
    {
        /*
         Id (int) 
Название (string) 
Описание (string) 
Дата Создания (DateTime) 
Статус (string) (например, "Новая", "В процессе", "Завершена")
         */

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }

        public override string ToString()
        {
            return $"{Id}.{Name}:\n{Description},\nCreated on: {CreatedAt.DayOfWeek},\nStatus: {Status}";
        }
    }

    public enum Status
    {
        New,
        InProgress,
        Complete
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Note> Notes => Set<Note>();
        public ApplicationContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=MIRUAHUA;Initial Catalog=Notes;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }
}
