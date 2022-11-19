using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplicationCats.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KittenController : ControllerBase
    {
        // Поля
        private int _status = 0;
        private IMemoryCache _memoryCache;
        static HttpClient Client = new HttpClient();
        string _contentType = "image/jpeg";

        // Конструктор
        public KittenController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // HTTP Метод GET
        [HttpGet]
        [Route("GetPic")]
        public async Task<FileContentResult> GetPicture(string url)
        {
            HttpResponseMessage response = await Client.GetAsync(url);
            _status = (int)response.StatusCode;
            try
            {
                return File(await GetFromCache($"https://http.cat/{_status}.jpg"), _contentType);
            }
            catch (Exception)
            {
                throw;
            } 
        }

        // Загрузка картинки в кэш на 10 секунд
        private async Task CacheSet(string url, byte[] image)
        {
            await Task.Run(() => _memoryCache.Set(url, image, TimeSpan.FromSeconds(10)));
        }

        // Выгрузка картинки из кэша
        private async Task<byte[]> GetFromCache(string url)
        {
            if (_memoryCache.TryGetValue(url, out byte[] val))
                return val;

            CacheSet(url, await DownloadImage(url));
            return await GetFromCache(url);
        }

        // Скачивание картинки при отсутствии в кэше нужной
        private async Task<byte[]> DownloadImage(string url)
        {
            var data = await new HttpClient().GetAsync(url);
            byte[] image = await data.Content.ReadAsByteArrayAsync();
            return image;
        }
    }
}
