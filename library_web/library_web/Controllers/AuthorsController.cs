using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace library_web.Controllers
{
    public class AuthorsController : Controller
    {
        // cong https cua API (xem Properties/launchSettings.json cua project API)
        private const string ApiBase = "https://localhost:7260";
        private const string ApiUser = "mvc2@example.com";
        private const string ApiPassword = "mvc123";

        private readonly IHttpClientFactory httpClientFactory;

        public AuthorsController(IHttpClientFactory httpClientFactory)
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

        // GET: /Authors  (liet ke tat ca)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<authorDTO> response = new List<authorDTO>();
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Authors/get-all-author");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // GET: /Authors/listAuthor/5  (liet ke theo Id)
        [HttpGet]
        public async Task<IActionResult> listAuthor(int id)
        {
            authorNoIdDTO response = new authorNoIdDTO();
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Authors/get-author-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                if (httpResponseMess.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("Khong tim thay tac gia co Id = " + id);
                }
                response = await httpResponseMess.Content.ReadFromJsonAsync<authorNoIdDTO>() ?? new authorNoIdDTO();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            ViewBag.Id = id;
            return View(response);
        }

        // GET: /Authors/addAuthor
        [HttpGet]
        public IActionResult addAuthor()
        {
            return View();
        }

        // POST: /Authors/addAuthor
        [HttpPost]
        public async Task<IActionResult> addAuthor(addAuthorDTO addAuthorDTO)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{ApiBase}/api/Authors/add-author"),
                    Content = new StringContent(JsonSerializer.Serialize(addAuthorDTO), Encoding.UTF8,
                        System.Net.Mime.MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Authors");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(addAuthorDTO);
        }

        // GET: /Authors/editAuthor/5
        [HttpGet]
        public async Task<IActionResult> editAuthor(int id)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Authors/get-author-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                if (httpResponseMess.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("Khong tim thay tac gia co Id = " + id);
                }
                var author = await httpResponseMess.Content.ReadFromJsonAsync<authorNoIdDTO>() ?? new authorNoIdDTO();
                ViewBag.Id = id;
                return View(author);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Authors");
            }
        }

        // POST: /Authors/editAuthor/5
        [HttpPost]
        public async Task<IActionResult> editAuthor([FromRoute] int id, authorNoIdDTO author)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"{ApiBase}/api/Authors/update-author-by-id/" + id),
                    Content = new StringContent(JsonSerializer.Serialize(author), Encoding.UTF8,
                        System.Net.Mime.MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Authors");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            ViewBag.Id = id;
            return View(author);
        }

        // GET: /Authors/delAuthor/5
        [HttpGet]
        public async Task<IActionResult> delAuthor([FromRoute] int id)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.DeleteAsync($"{ApiBase}/api/Authors/delete-author-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index", "Authors");
        }
    }
}