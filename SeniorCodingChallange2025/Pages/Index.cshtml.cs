using Microsoft.AspNetCore.Mvc.RazorPages;
using SeniorCodingChallange2025.Data;

namespace SeniorCodingChallange2025.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    /// <summary>
    /// Constructor for IndexModel
    /// </summary>
    /// <param name="logger">ILogger for logging purposes</param>
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    // List of providers loaded from CSV
    public List<Provider> Providers { get; set; } = new();
    
    // List of hospitals loaded from CSV
    public List<Hospital> Hospitals { get; set; } = new();
    
    // Selected hospital from the dropdown
    public string SelectedHospital { get; set; }

    public void OnGet(string? selectedHospital = null)
    {
        // Load providers from CSV
        var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Providers.csv");
        var allProviders = CsvLoader.LoadCsv(csvPath, fields => new Provider
        {
            Name = fields[0].Trim('"'),
            Number = fields[1].Trim('"'),
            Hospital = fields[2].Trim('"'),
            Doctor = fields[3].Trim('"').Equals("Yes", StringComparison.OrdinalIgnoreCase)
        });

        // Only keep valid providers
        Providers = allProviders
            .Where(p => !string.IsNullOrWhiteSpace(p.Name) &&
                        !string.IsNullOrWhiteSpace(p.Number) &&
                        IsValidName(p.Name))
            .ToList();

        // Load hospitals from CSV
        var hospitalsPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Hospitals.csv");
        var allHospitals = CsvLoader.LoadCsv(hospitalsPath, fields => new Hospital
        {
            Name = fields[0].Trim('"'),
            Identity = fields[1].Trim('"')
        });
        
        // All hospitals must have a name and identity.
        Hospitals = allHospitals
            .Where(h => !string.IsNullOrWhiteSpace(h.Name) && !string.IsNullOrWhiteSpace(h.Identity))
            .ToList();

        // Update the selected hospital
        SelectedHospital = selectedHospital;
        
        // All providers must have a name and number.
        // Server-side validation logic, output to console for invalid providers
        foreach (var provider in allProviders)
        {
            // Check if provider has a name and number
            if (string.IsNullOrWhiteSpace(provider.Name) || string.IsNullOrWhiteSpace(provider.Number))
            {
                // If either is missing, output to console
                Console.WriteLine($"Provider missing name or number: {provider.Name} / {provider.Number}");
            }
            // Check if provider name is valid
            else if (!IsValidName(provider.Name))
            {
                // If name is invalid, output to console
                Console.WriteLine($"Invalid provider name: {provider.Name}");
            }
        }

        // All hospitals must have a name and identity.
        foreach (var hospital in allHospitals)
        {
            if (string.IsNullOrWhiteSpace(hospital.Name) || string.IsNullOrWhiteSpace(hospital.Identity))
            {
                Console.WriteLine($"Hospital missing name or identity: {hospital.Name} / {hospital.Identity}");
            }
        }
    }

    // Helper for name validation
    //<remark>
    // The name must be at least 5 characters long, contain at least two words,
    // and only consist of letters, spaces, apostrophes, and hyphens.
    //</remark>
    private bool IsValidName(string name)
    {
        // Check if name is null or whitespace
        if (string.IsNullOrWhiteSpace(name)) return false;
        // Split name into words and check conditions
        var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        // Must have at least 2 words
        if (words.Length < 2) return false;
        // Must be at least 5 characters long
        var alphaCount = name.Count(char.IsLetter);
        if (alphaCount < 5) return false;
        // Check if all characters are valid
        foreach (var c in name)
        {
            // Valid characters are letters, space, apostrophe, and hyphen
            if (!(char.IsLetter(c) || c == ' ' || c == '\'' || c == '-'))
            {
                // If any character is invalid, return false
                return false;
            }
        }
        // If all checks passed, the name is valid
        return true;
    }
}