using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dbFile = "knihovna.sqlite";

            if (!System.IO.File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
                Console.WriteLine("Databázový soubor vytvořen");
            }

            string connectionString = $"Data Source={dbFile};Version=3";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Knihy (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        Year INTEGER,
                        Available INTEGER
                )";

                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                Console.WriteLine("Tabulka Knihy byla vytvořena nebo již existuje");
            }

            while (true)
            {
                Console.WriteLine("1 - Přidat knihu");
                Console.WriteLine("2 - Zobrazit knihy");
                Console.WriteLine("3 - Změnit dostupnost");
                Console.WriteLine("4 - Odstranit knihu");
                Console.WriteLine("0 - Konec");

                Console.Write("Vyber možnost: ");
                string volba = Console.ReadLine();

                if (volba == "1")
                {
                    Console.Write("Název knihy: ");
                    string title = Console.ReadLine();

                    Console.Write("Autor: ");
                    string author = Console.ReadLine();

                    Console.Write("Rok vydání: ");
                    int year = int.Parse(Console.ReadLine());

                    Console.Write("Dostupnost (1 = dostupná, 0 = vypůjčená): ");
                    int available = int.Parse(Console.ReadLine());

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = @"INSERT INTO Knihy
                    (Title, Author, Year, Available)
                    VALUES (@title, @author, @year, @available)";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);

                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@author", author);
                        command.Parameters.AddWithValue("@year", year);
                        command.Parameters.AddWithValue("@available", available);

                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine("Kniha byla přidána.");
                }
                else if (volba == "2")
                {
                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM Knihy";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);

                        SQLiteDataReader reader = command.ExecuteReader();

                        Console.WriteLine("SEZNAM KNIH");

                        while (reader.Read())
                        {
                            Console.WriteLine("ID: " + reader["Id"]);
                            Console.WriteLine("Název: " + reader["Title"]);
                            Console.WriteLine("Autor: " + reader["Author"]);
                            Console.WriteLine("Rok vydání: " + reader["Year"]);

                            if (Convert.ToInt32(reader["Available"]) == 1)
                            {
                                Console.WriteLine("Stav: Dostupná");
                            }
                            else
                            {
                                Console.WriteLine("Stav: Vypůjčená");
                            }
                        }

                        reader.Close();
                    }
                }
                else if (volba == "3")
                {
                    Console.Write("Zadejte ID knihy: ");
                    int id = int.Parse(Console.ReadLine());

                    Console.Write("Nová dostupnost (1 = dostupná, 0 = vypůjčená): ");
                    int available = int.Parse(Console.ReadLine());

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "UPDATE Knihy SET Available = @available WHERE Id = @id";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);

                        command.Parameters.AddWithValue("@available", available);
                        command.Parameters.AddWithValue("@id", id);

                        int vysledek = command.ExecuteNonQuery();

                        if (vysledek > 0)
                        {
                            Console.WriteLine("Dostupnost byla změněna.");
                        }
                        else
                        {
                            Console.WriteLine("Kniha nebyla nalezena.");
                        }
                    }

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "SELECT * FROM Knihy";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);
                        SQLiteDataReader reader = command.ExecuteReader();

                        Console.WriteLine("\nAKTUALIZOVANÝ SEZNAM KNIH");

                        while (reader.Read())
                        {
                            Console.WriteLine("\nID: " + reader["Id"]);
                            Console.WriteLine("Název: " + reader["Title"]);
                            Console.WriteLine("Autor: " + reader["Author"]);
                            Console.WriteLine("Rok vydání: " + reader["Year"]);

                            if (Convert.ToInt32(reader["Available"]) == 1)
                            {
                                Console.WriteLine("Stav: Dostupná");
                            }
                            else
                            {
                                Console.WriteLine("Stav: Vypůjčená");
                            }
                        }

                        reader.Close();
                    }
                }
                else if (volba == "4")
                {
                    Console.Write("Zadejte ID knihy: ");
                    int id = int.Parse(Console.ReadLine());

                    using (var connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        string sql = "DELETE FROM Knihy WHERE Id = @id";

                        SQLiteCommand command = new SQLiteCommand(sql, connection);

                        command.Parameters.AddWithValue("@id", id);

                        int vysledek = command.ExecuteNonQuery();

                        if (vysledek > 0)
                        {
                            Console.WriteLine("Kniha byla odstraněna.");
                        }
                        else
                        {
                            Console.WriteLine("Kniha nebyla nalezena.");
                        }
                    }
                }
                else if (volba == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Neplatná volba.");
                }
            }
        }
    }
}
