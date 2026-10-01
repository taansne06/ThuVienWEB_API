using System.Text;
using System.Text.Json;

namespace ThucHanhWEBAPI.Middlewares
{
    
    public class RequiredFieldsMiddleware
    {
        private readonly RequestDelegate _next;

        // Title, Author (authorIds), Publisher (publisherID)
        private static readonly string[] RequiredFields = { "title", "authorIds", "publisherID" };

        public RequiredFieldsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (HttpMethods.IsPost(context.Request.Method) &&
                path.Equals("/api/Book/add-book", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                try
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.ValueKind != JsonValueKind.Object)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new { message = "JSON phải là một object" });
                        return;
                    }

                    var present = doc.RootElement.EnumerateObject()
                        .Select(p => p.Name)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var missing = RequiredFields.Where(f => !present.Contains(f)).ToList();
                    if (missing.Count > 0)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Thiếu trường bắt buộc",
                            missingFields = missing
                        });
                        return;
                    }
                }
                catch (JsonException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { message = "JSON không hợp lệ hoặc rỗng" });
                    return;
                }
            }

            await _next(context);
        }
    }
}