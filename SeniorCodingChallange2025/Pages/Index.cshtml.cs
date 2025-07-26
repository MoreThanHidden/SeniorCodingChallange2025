using Microsoft.AspNetCore.Mvc;
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
    
    // List of patients loaded from CSV
    public List<Patient> Patients { get; set; } = new();
    // List of treatments loaded from CSV
    public List<Treatment> Treatments { get; set; } = new();
    
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

        // Load patients from CSV
        var patientsPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Patients.csv");
        var allPatients = CsvLoader.LoadCsv(patientsPath, fields => new Patient
        {
            MedicalReferenceNumber = fields[0].Trim('"').Trim(),
            PatientName = fields[1].Trim('"').Trim()
        });
        
        // Only keep valid patients according to all rules:
        // Must have both a Medical Reference Number and a name.
        // Must abide by the name validation rules.
        Patients = allPatients
            .Where(p =>
                !string.IsNullOrWhiteSpace(p.MedicalReferenceNumber) && // Must have a Medical Reference Number
                !string.IsNullOrWhiteSpace(p.PatientName) && // Must have a name
                IsValidName(p.PatientName) // Name must be valid according to the rules
            )
            .ToList();

        // Load treatments from CSV
        var treatmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Treatments.csv");
        var allTreatments = CsvLoader.LoadCsv(treatmentsPath, fields => new Treatment
        {
            Details = fields[0].Trim('"').Trim(),
            Hospital = fields[1].Trim('"').Trim(),
            Provider = fields[2].Trim('"').Trim(),
            Patient = fields[3].Trim('"').Trim(),
            DateTimeDischarged = fields[4].Trim('"').Trim()
        });

        // Only keep valid treatments according to all rules:
        // All treatments must have a hospital and patient.
        // If a treatment has a dispatched date, it must have a provider and details.
        // All hospitals referenced by treatments must exist in the hospitals data file.
        // All clients referenced by treatments must exist in the patients data file.
        // All providers referenced by treatments must exist in the providers data file.
        Treatments = allTreatments;
            //.Where(t =>
                //!string.IsNullOrWhiteSpace(t.Hospital) && // Must have a hospital
                //!string.IsNullOrWhiteSpace(t.Patient) && // Must have a patient
                //Hospitals.Any(h => h.Name.Equals(t.Hospital, StringComparison.OrdinalIgnoreCase)) && // Hospital must exist
                //Patients.Any(p => p.PatientName.Equals(t.Patient, StringComparison.OrdinalIgnoreCase)) && // Patient must exist
                //Providers.Any(p => p.Name.Equals(t.Provider, StringComparison.OrdinalIgnoreCase)) && // Provider must exist
                //(!string.IsNullOrWhiteSpace(t.DateTimeDischarged) || // If discharged date is present, provider and details must also be present
                // (!string.IsNullOrWhiteSpace(t.Provider) && !string.IsNullOrWhiteSpace(t.Details)))
            //)
            //.ToList();
        
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

    /// <summary>
    /// Handles the form submission for adding or editing a treatment.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var form = Request.Form;
        var editTreatmentId = form["EditTreatmentId"].ToString();
        var details = form["Details"].ToString();
        var hospital = form["Hospital"].ToString();
        var provider = form["Provider"].ToString();
        var patient = form["Patient"].ToString();
        var dateTimeDischarged = form["DateTimeDischarged"].ToString();

        // Reload all data (to keep lists in sync)
        OnGet();
        
        if (!string.IsNullOrEmpty(editTreatmentId))
        {
            // Edit existing treatment
            if (int.TryParse(editTreatmentId, out int idx) && idx >= 0 && idx < Treatments.Count)
            {
                Treatments[idx].Details = details;
                Treatments[idx].Hospital = hospital;
                Treatments[idx].Provider = provider;
                Treatments[idx].Patient = patient;
                Treatments[idx].DateTimeDischarged = dateTimeDischarged;
            }
        }
        else
        {
            // Add new treatment
            Treatments.Add(new Treatment
            {
                Details = details,
                Hospital = hospital,
                Provider = provider,
                Patient = patient,
                DateTimeDischarged = dateTimeDischarged
            });
        }

        // Save all treatments back to CSV
        var treatmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Treatments.csv");
        using (var writer = new StreamWriter(treatmentsPath, false))
        {
            foreach (var t in Treatments)
            {
                // Write each treatment as a CSV line
                await writer.WriteLineAsync($"\"{t.Details}\",\"{t.Hospital}\",\"{t.Provider}\",\"{t.Patient}\",\"{t.DateTimeDischarged}\"");
            }
        }

        return RedirectToPage();
    }

    /// <summary>
    /// Handles the request to edit a treatment.
    /// </summary>
    /// <param name="editIndex">Index of the treatment to edit</param>
    /// <returns>IActionResult to render the page with edit data</returns>
    public IActionResult OnPostEdit(int editIndex)
    {
        // Reload all data
        OnGet();
        if (editIndex >= 0 && editIndex < Treatments.Count)
        {
            var t = Treatments[editIndex];
            ViewData["EditTreatmentId"] = editIndex;
            ViewData["EditDetails"] = t.Details;
            ViewData["EditHospital"] = t.Hospital;
            ViewData["EditProvider"] = t.Provider;
            ViewData["EditPatient"] = t.Patient;
            ViewData["EditDateTimeDischarged"] = t.DateTimeDischarged;
        }
        return Page();
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