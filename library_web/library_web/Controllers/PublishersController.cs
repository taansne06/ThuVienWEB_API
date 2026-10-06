using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace library_web.Controllers
{
    public class PublishersController : Controller
    {
        // cong https cua API (xem Properties/launchSettings.json cua project API)
        private const string ApiBase = "https://localhost:7260";
        private const string ApiUser = "mvc2@example.com";
        private const string ApiPassword = "mvc123";

        private readonly IHttpClientFactory httpClientFactory;

        public PublishersController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // dang nhap API de lay token, gan vao header cho moi request
        private async Task<HttpClient> CreateApiClientAsync()
        {
            var client = httpClientFactory.CreateClient();
            var loginResponse = await client.PostAsJsonAsync($"{ApiBase}/api/User/Login",
                new { username = ApiUser, password = ApiPassword });
            if (!loginResponse.IsSuccessStatusCode)
            {
                var body = await loginResponse.Content.ReadAsStringAsync();
                throw new Exception($"Dang nhap API that bai ({(int)loginResponse.StatusCode}): {body}");
            }
            var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDTO>();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", login!.JwtToken);
            return client;
        }

        // GET: /Publishers  (liet ke tat ca)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<publisherDTO> response = new List<publisherDTO>();
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Publishers/get-all-publisher");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // GET: /Publishers/listPublisher/5  (liet ke theo Id)
        [HttpGet]
        public async Task<IActionResult> listPublisher(int id)
        {
            publisherNoIdDTO response = new publisherNoIdDTO();
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Publishers/get-publisher-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                response = await httpResponseMess.Content.ReadFromJsonAsync<publisherNoIdDTO>() ?? new publisherNoIdDTO();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            ViewBag.Id = id;
            return View(response);
        }

        // GET: /Publishers/addPublisher
        [HttpGet]
        public IActionResult addPublisher()
        {
            return View();
        }

        // POST: /Publishers/addPublisher
        [HttpPost]
        public async Task<IActionResult> addPublisher(addPublisherDTO addPublisherDTO)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{ApiBase}/api/Publishers/add-publisher"),
                    Content = new StringContent(JsonSerializer.Serialize(addPublisherDTO), Encoding.UTF8,
                        System.Net.Mime.MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Publishers");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(addPublisherDTO);
        }

        // GET: /Publishers/editPublisher/5
        [HttpGet]
        public async Task<IActionResult> editPublisher(int id)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Publishers/get-publisher-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                var publisher = await httpResponseMess.Content.ReadFromJsonAsync<publisherNoIdDTO>() ?? new publisherNoIdDTO();
                ViewBag.Id = id;
                return View(publisher);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Publishers");
            }
        }

        // POST: /Publishers/editPublisher/5
        [HttpPost]
        public async Task<IActionResult> editPublisher([FromRoute] int id, publisherNoIdDTO publisher)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"{ApiBase}/api/Publishers/update-publisher-by-id/" + id),
                    Content = new StringContent(JsonSerializer.Serialize(publisher), Encoding.UTF8,
                        System.Net.Mime.MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Publishers");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            ViewBag.Id = id;
            return View(publisher);
        }

        // GET: /Publishers/delPublisher/5
        [HttpGet]
        public async Task<IActionResult> delPublisher([FromRoute] int id)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.DeleteAsync($"{ApiBase}/api/Publishers/delete-publisher-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index", "Publishers");
        }
    }
}