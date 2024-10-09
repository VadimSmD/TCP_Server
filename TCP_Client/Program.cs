using System.Net.Sockets;

using TcpClient tcpClient = new TcpClient();
string message = Console.ReadLine();
Console.WriteLine("Клиент запущен");
await tcpClient.ConnectAsync("127.0.0.1", 8888);

if (tcpClient.Connected) {
    Console.WriteLine($"Подключение с {tcpClient.Client.RemoteEndPoint} установлено");
    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
    NetworkStream stream = tcpClient.GetStream();

    stream.Write(data, 0, data.Length);

    stream = tcpClient.GetStream();
    data = new Byte[256];
    String responseData = String.Empty;
    Int32 bytes = stream.Read(data, 0, data.Length);
    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    Console.WriteLine(responseData);
    Console.ReadLine();

}
    
else { Console.WriteLine("Не удалось подключиться"); }
    