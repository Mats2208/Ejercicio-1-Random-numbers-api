using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace Ejercicio_1__Random_numbers_API.Controllers
{
    [ApiController]
    [Route("random")]
    public class RandomController : ControllerBase
    {
        [HttpGet("number")]
        public IActionResult GetRandomNumber([FromQuery] int? min, [FromQuery] int? max)
        {
            int result;

            if (min.HasValue && max.HasValue)
            {
                if (max.Value <= min.Value)
                    return BadRequest("max debe ser mayor que min.");
                result = Random.Shared.Next(min.Value, max.Value);
            }
            else
            {
                result = Random.Shared.Next(1, 9999);
            }

            return Ok(new { number = result });
        }

        [HttpGet("decimal")]
        public IActionResult GetRandomDecimal()
        {
            double result = Random.Shared.NextDouble();
            return Ok(new { number = result });
        }

        [HttpGet("string")]
        public IActionResult GetRandomString([FromQuery] int length)
        {
            if (length < 1 || length > 1024)
                return BadRequest("Length debe estar entre 1 y 1024.");

            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = Random.Shared;
            var chars = new char[length];

            for (int i = 0; i < length; i++)
                chars[i] = alphabet[random.Next(alphabet.Length)];

            string result = new string(chars);
            return Ok(new { str = result });
        }


        [HttpPost("custom")]
        public IActionResult GetRandomAnswer([FromBody] JsonElement body)
        {
            var type = body.GetProperty("type").GetString()!;

            switch (type.ToLowerInvariant())
            {
                case "number":
                {
                    int min = body.GetProperty("min").GetInt32();
                    int max = body.GetProperty("max").GetInt32();
                    return Ok(new { result = Random.Shared.Next(min, max) });
                }

                case "decimal":
                {
                    double min = body.GetProperty("min").GetDouble();
                    double max = body.GetProperty("max").GetDouble();
                    int decimals = body.GetProperty("decimals").GetInt32();
                    double value = Random.Shared.NextDouble() * (max - min) + min;
                    return Ok(new { result = Math.Round(value, decimals) });
                }

                case "string":
                {
                    int length = body.GetProperty("length").GetInt32();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var buf = new char[length];

                    for (int i = 0; i < length; i++)
                        buf[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];

                    return Ok(new { result = new string(buf) });
                }

                default:
                    return BadRequest("Type inválido.");
            }
        }
    }
}