using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using library_web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace library_web.Controllers
{
    public class BooksController : Controller
    {
        // cong https cua API (xem Properties/launchSettings.json cua project API)
        private const string ApiBase = "https://localhost:7260";
        private const string ApiUser = "mvc2@example.com";
        private const string ApiPassword = "mvc123";

        private readonly IHttpClientFactory httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
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

        // GET: /Books  (liet ke + tim kiem + sap xep)
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? filterOn = null,
            string? filterQuery = null, string? sortBy = null, bool isAscending = true)
        {
            List<BookDTO> response = new List<BookDTO>();
            try
            {
                var client = await CreateApiClientAsync();
                var url = $"{ApiBase}/api/Book/get-all-books?filterOn={filterOn}&filterQuery={filterQuery}&sortBy={sortBy}&isAscending={isAscending}&pageNumber=1&pageSize=100";
                var httpResponseMess = await client.GetAsync(url);
                if (!httpResponseMess.IsSuccessStatusCode)
                {
                    var body = await httpResponseMess.Content.ReadAsStringAsync();
                    throw new Exception($"Goi get-all-books that bai ({(int)httpResponseMess.StatusCode}): {body}");
                }
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<BookDTO>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // GET: /Books/addBook
        [HttpGet]
        public async Task<IActionResult> addBook()
        {
            var client = await CreateApiClientAsync();

            List<authorDTO> responseAu = new List<authorDTO>();
            var httpResponseAu = await client.GetAsync($"{ApiBase}/api/Authors/get-all-author");
            httpResponseAu.EnsureSuccessStatusCode();
            responseAu.AddRange(await httpResponseAu.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());
            ViewBag.listAuthor = responseAu;

            List<publisherDTO> responsePu = new List<publisherDTO>();
            var httpResponsePu = await client.GetAsync($"{ApiBase}/api/Publishers/get-all-publisher");
            httpResponsePu.EnsureSuccessStatusCode();
            responsePu.AddRange(await httpResponsePu.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());
            ViewBag.listPublisher = responsePu;

            return View();
        }

        // POST: /Books/addBook
        [HttpPost]
        public async Task<IActionResult> addBook(addBookDTO addBookDTO)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{ApiBase}/api/Book/add-book"),
                    Content = new StringContent(JsonSerializer.Serialize(addBookDTO), Encoding.UTF8,
                        System.Net.Mime.MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();
                var response = await httpResponseMess.Content.ReadFromJsonAsync<addBookDTO>();
                if (response != null)
                {
                    return RedirectToAction("Index", "Books");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return await addBook();   // nap lai danh sach tac gia, NXB cho form
        }

        // GET: /Books/listBook/5
        public async Task<IActionResult> listBook(int id)
        {
            BookDTO response = new BookDTO();
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Book/get-book-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                response = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }

        // GET: /Books/editBook/5
        [HttpGet]
        public async Task<IActionResult> editBook(int id)
        {
            var client = await CreateApiClientAsync();

            BookDTO responseBook = new BookDTO();
            var httpResponseMess = await client.GetAsync($"{ApiBase}/api/Book/get-book-by-id/" + id);
            httpResponseMess.EnsureSuccessStatusCode();
            responseBook = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>();
            ViewBag.Book = responseBook;

            List<authorDTO> responseAu = new List<authorDTO>();
            var httpResponseAu = await client.GetAsync($"{ApiBase}/api/Authors/get-all-author");
            httpResponseAu.EnsureSuccessStatusCode();
            responseAu.AddRange(await httpResponseAu.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>());
            ViewBag.listAuthor = responseAu;

            List<publisherDTO> responsePu = new List<publisherDTO>();
            var httpResponsePu = await client.GetAsync($"{ApiBase}/api/Publishers/get-all-publisher");
            httpResponsePu.EnsureSuccessStatusCode();
            responsePu.AddRange(await httpResponsePu.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>());
            ViewBag.listPublisher = responsePu;

            return View();
        }

        // POST: /Books/editBook/5
        [HttpPost]
        public async Task<IActionResult> editBook([FromRoute] int id, editBookDTO bookDTO)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpRequestMess = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri($"{ApiBase}/api/Book/update-book-by-id/" + id),
                    Content = new StringContent(JsonSerializer.Serialize(bookDTO), Encoding.UTF8,
                        System.Net.Mime.MediaTypeNames.Application.Json)
                };
                var httpResponseMess = await client.SendAsync(httpRequestMess);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return await editBook(id);
        }

        // GET: /Books/delBook/5
        [HttpGet]
        public async Task<IActionResult> delBook([FromRoute] int id)
        {
            try
            {
                var client = await CreateApiClientAsync();
                var httpResponseMess = await client.DeleteAsync($"{ApiBase}/api/Book/delete-book-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index", "Books");
        }
    }
}