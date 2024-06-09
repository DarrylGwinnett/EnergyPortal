namespace EnergyPortalApi.Data.Models;

internal class DbAccount(int id, string firstName, string lastName)
{
    public int Id { get; set; } = id;

    public string FirstName { get; set; } = firstName;

    public string LastName { get; set; } = lastName;
}
