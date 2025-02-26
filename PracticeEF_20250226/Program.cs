using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace PracticeEF_20250226
{
    /*
     Описать программу для управления студентами и их группами.
    Каждый студент может вступить в любую группу. 
    Группа содержит в свою очередь множество студентов. 
    Получить группы студента. 
    Получить студентов через группу. 
    Добавить возможность удаление и редактирования студента по Id.
     */
    internal class Program
    {
        public static string connectingString = "Data Source=MIRUAHUA;Initial Catalog=TaskFeb26;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        static void Main(string[] args)
        {
            List<Group> groups = new List<Group>()
            {
                new Group(){ GroupName = "Math"},
                new Group(){GroupName = "C#"},
                new Group(){ GroupName = "JavaScript"},
                new Group(){ GroupName = "Databases"},
                new Group(){GroupName = ".Net"},
                new Group(){ GroupName = "Python"},
                new Group(){GroupName = "Java"},
                new Group(){ GroupName = "Microservices"},
                new Group(){ GroupName = "Cloud"},
                new Group(){GroupName = "Algorithms"}
            };
            CreateGroups(groups);
            List<Student> students = new List<Student>()
            {
                new Student(){StudentName = "Fujita Akira" },
                new Student(){StudentName = "Hayashi Yoko" },
                new Student(){StudentName = "Nakayama Midori" },
                new Student(){StudentName = "Takeda Taro" },
                new Student(){StudentName = "Sato Yoshimi" },
                new Student(){StudentName = "Takao Tsuyoshi" },
                new Student(){StudentName = "Matsuo Ken" },
                new Student(){StudentName = "Kawamori Yumi" },
                new Student(){StudentName = "Yamanashi Ken" },
                new Student(){StudentName = "Ito Akira" },
                new Student(){StudentName = "Kawabata Taro" },
                new Student(){StudentName = "Kobayashi Yuko" },
                new Student(){StudentName = "Takemori Yukiko" },
                new Student(){StudentName = "Fujiwara Moriko" },
                new Student(){StudentName = "Moriyama Yuka" }
            };
            CreateStudents(students);

            List<Record> records = new List<Record>()
            {
                new Record(){GroupId = 1, StudentId = 5},
                new Record(){GroupId = 1, StudentId = 7},
                new Record(){GroupId = 1, StudentId = 13},
                new Record(){GroupId = 2, StudentId = 5},
                new Record(){GroupId = 2, StudentId = 2},
                new Record(){GroupId = 3, StudentId = 4},
                new Record(){GroupId = 3, StudentId = 10},
                new Record(){GroupId = 3, StudentId = 15},
                new Record(){GroupId = 3, StudentId = 11},
                new Record(){GroupId = 4, StudentId = 7},
                new Record(){GroupId = 4, StudentId = 10},
                new Record(){GroupId = 4, StudentId = 11},
                new Record(){GroupId = 4, StudentId = 12},
                new Record(){GroupId = 4, StudentId = 13},
                new Record(){GroupId = 5, StudentId = 1},
                new Record(){GroupId = 5, StudentId = 2},
                new Record(){GroupId = 5, StudentId = 3},
                new Record(){GroupId = 5, StudentId = 4},
                new Record(){GroupId = 6, StudentId = 5},
                new Record(){GroupId = 6, StudentId = 1},
                new Record(){GroupId = 6, StudentId = 10},
                new Record(){GroupId = 6, StudentId = 15},
                new Record(){GroupId = 7, StudentId = 5},
                new Record(){GroupId = 7, StudentId = 8},
                new Record(){GroupId = 7, StudentId = 7},
                new Record(){GroupId = 7, StudentId = 10},
                new Record(){GroupId = 7, StudentId = 11},
                new Record(){GroupId = 7, StudentId = 12},
                new Record(){GroupId = 8, StudentId = 2},
                new Record(){GroupId = 8, StudentId = 3},
                new Record(){GroupId = 8, StudentId = 4},
                new Record(){GroupId = 9, StudentId = 9},
                new Record(){GroupId = 9, StudentId = 10},
                new Record(){GroupId = 10, StudentId = 5},
                new Record(){GroupId = 10, StudentId = 10},
                new Record(){GroupId = 10, StudentId = 11},
                new Record(){GroupId = 10, StudentId = 12},
                new Record(){GroupId = 10, StudentId = 13},
                new Record(){GroupId = 10, StudentId = 14},
                new Record(){GroupId = 10, StudentId = 15}
            };
            CreateRecords(records);

            GetAllStudents();
            Console.WriteLine("--------------------");
            GetStudentGroups(5, connectingString);
            Console.WriteLine("--------------------");
            GetGroupStudents(10, connectingString);
            Console.WriteLine("--------------------");
            UpdateStudent(10, "Takahashi Taro");
            Console.WriteLine("--------------------");
            DeleteStudent(15);

        }

        public static void GetAllStudents()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var students = db.Students.ToList();
                foreach (Student st in students)
                {
                    Console.WriteLine(st.ToString());
                }
            }
        }
        public static void DeleteStudent(int studentId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var student = db.Students.FirstOrDefault(e => e.StudentId == studentId);
                if (student != null)
                {
                    db.Students.Remove(student);
                    db.SaveChanges();
                }
                Console.WriteLine("\nAfter update:");
                GetAllStudents();
            }
        }
        public static void UpdateStudent(int studentId, string newName)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var student = db.Students.FirstOrDefault(e => e.StudentId == studentId);
                if (student != null)
                {
                    student.StudentName = newName;
                    db.SaveChanges();
                }
                Console.WriteLine("\nAfter update:");
                GetAllStudents();
            }
        }
        public static void CreateGroups(List<Group> groups)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Groups.AddRange(groups);
                db.SaveChanges();
            }
        }
        public static void CreateStudents(List<Student> students)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Students.AddRange(students);
                db.SaveChanges();
            }
        }
        public static void CreateRecords(List<Record> records)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Records.AddRange(records);
                db.SaveChanges();
            }
        }

        public static void GetGroupStudents(int groupId, string connectionString)
        {
            Console.WriteLine($"Students in group {groupId}:");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    SELECT 
                    	Students.StudentId,
                    	Students.StudentName
                    FROM Students
                    JOIN Records ON Students.StudentId = Records.StudentId
                    JOIN Groups ON Records.GroupId = Groups.GroupId
                    WHERE Groups.GroupId = @groupId
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.Parameters.Add(new SqlParameter("@groupId", groupId));
                command.ExecuteNonQuery();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        string columnName1 = reader.GetName(0);
                        string columnName2 = reader.GetName(1);
                        Console.WriteLine($"> {columnName1} | {columnName2} ");
                        while (reader.Read())
                        {
                            int studentId = reader.GetInt32(0);
                            string studentName = reader.GetString(1);

                            Console.WriteLine($"> {studentId} | {studentName}");
                        }
                    }
                }

            }
        }
        public static void GetStudentGroups(int studentId, string connectionString)
        {
            Console.WriteLine($"Groups for student {studentId}:");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandtext = $"""
                    SELECT 
                    	Groups.GroupId,
                    	Groups.GroupName
                    FROM Students
                    JOIN Records ON Students.StudentId = Records.StudentId
                    JOIN Groups ON Records.GroupId = Groups.GroupId
                    WHERE Students.StudentId = @studentId
                    """;
                SqlCommand command = new SqlCommand(commandtext, connection);
                command.Parameters.Add(new SqlParameter("@studentId", studentId));
                command.ExecuteNonQuery();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        string columnName1 = reader.GetName(0);
                        string columnName2 = reader.GetName(1);
                        Console.WriteLine($"> {columnName1} | {columnName2} ");
                        while (reader.Read())
                        {
                            int groupId = reader.GetInt32(0);
                            string groupName = reader.GetString(1);

                            Console.WriteLine($"> {groupId} | {groupName}");
                        }
                    }
                }

            }
        }
    }

    
    public class ApplicationContext : DbContext
    {
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Record> Records => Set<Record>();
        public ApplicationContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=MIRUAHUA;Initial Catalog=TaskFeb26;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }

    public class Record
    {
        public  int RecordId {  get; set; }
        public int StudentId { get; set; }
        public int GroupId { get; set; }
    }
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public override string ToString()
        {
            return $"{StudentId}. {StudentName} ";
        }
    }
    public class Group
    {
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public override string ToString()
        {
            return $" Group: {GroupId} {GroupName} ";
        }
    }
}
