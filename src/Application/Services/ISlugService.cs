namespace Application.Services
{
    // Serwis do generowania unikalnych skrótów (slugów) dla URL-i
    public interface ISlugService
    {
        string GenerateSlug(int length = 7);
    }
}