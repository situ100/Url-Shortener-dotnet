using System.Security.Cryptography;
using System.Text;
using Application.Services;

namespace Infrastructure.Services;

// Implementacja serwisu do generowania unikalnych skrótów (slugów) dla URL-i
public class SlugService : ISlugService
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public string GenerateSlug(int length = 7)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);
        foreach (var b in bytes)
        {
            sb.Append(Alphabet[b % Alphabet.Length]);
        }
        return sb.ToString();
    }
}