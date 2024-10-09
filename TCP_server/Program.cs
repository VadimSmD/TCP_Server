using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

public class TCP_Server { 
public class Tickers
{
    public int Id { get; set; }
    public string Ticker { get; set; }
}
public class Prices
{
    public int Id { get; set; }
    public int Ticker_id { get; set; }
    public double Price { get; set; }
    public string Date { get; set; }
}
public class Condition
{
    public int Id { get; set; }
    public int Ticker_id { get; set; }
    public string State { get; set; }
}
public class ApplicationContext : DbContext
{
    public DbSet<Tickers> Tickers => Set<Tickers>();
    public DbSet<Condition> Condition => Set<Condition>();
    public DbSet<Prices> Prices => Set<Prices>();
    public ApplicationContext() => Database.EnsureCreated();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
}
    public static async Task Main(string[] args)
    {
        var tcpListener = new TcpListener(IPAddress.Any, 8888);
        try
        {
            tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений... ");
            while (true)
            {
                using var tcpClient = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine($"Входящее подключение: {tcpClient.Client.RemoteEndPoint}");
                Byte[] data = new Byte[256];
                String responseData = String.Empty;
                NetworkStream stream = tcpClient.GetStream();
                Int32 bytes = stream.Read(data, 0, data.Length);

                string stock = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                using (ApplicationContext db = new ApplicationContext())
                {
                    var tickers = db.Tickers.ToList().Where(a => a.Ticker == stock);
                    foreach (var t in tickers) {
                        var result = db.Condition.ToList().Where(lt => lt.Ticker_id == (t.Id));
                        foreach ( var t2 in result) {
                            Byte[] data_r = System.Text.Encoding.ASCII.GetBytes(t2.State);
                            NetworkStream stream_r = tcpClient.GetStream();
                            stream.Write(data_r, 0, data_r.Length);
                            break;
                        }
                        break;
                    }
                    
                }
            }
        }
        finally
        {
            tcpListener.Stop(); // останавливаем сервер
        }
    }
}