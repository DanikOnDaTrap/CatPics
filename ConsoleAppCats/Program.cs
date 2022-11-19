
using System.Runtime.CompilerServices;

int _status = 0;

using (var httpClient = new HttpClient())
{
    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://ya.ru/"))
    {
        var response = await httpClient.SendAsync(request);
        _status = (int)response.StatusCode;
       // Console.WriteLine(_status);
    }
}

using (var httpClient = new HttpClient())
{
    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://http.cat/" + _status))
    {
        var response = await httpClient.SendAsync(request);
        //Console.WriteLine(response);
        Console.WriteLine(response.Content.ReadAsByteArrayAsync());
    }
}

