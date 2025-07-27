using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SeniorCodingChallange2025.Data;

namespace SeniorCodingChallange2025.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private TreatmentService _treatmentService;
    private ProviderService _providerService;
    private HospitalService _hospitalService;
    private PatientService _patientService;

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
        // Load providers, hospitals, patients using services
        var providersPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Providers.csv");
        var hospitalsPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Hospitals.csv");
        var patientsPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Patients.csv");
        _providerService = new ProviderService(providersPath);
        _hospitalService = new HospitalService(hospitalsPath);
        _patientService = new PatientService(patientsPath);
        Providers = _providerService.LoadProviders();
        Hospitals = _hospitalService.LoadHospitals();
        Patients = _patientService.LoadPatients();
        // Initialize TreatmentService
        var treatmentsPath = Path.Combine(Directory.GetCurrentDirectory(), "InputCSV", "Treatments.csv");
        _treatmentService = new TreatmentService(treatmentsPath, Hospitals, Providers, Patients);
        Treatments = _treatmentService.LoadTreatments();
        SelectedHospital = selectedHospital;
        // Validation moved to services
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
        OnGet();
        if (!string.IsNullOrEmpty(editTreatmentId))
        {
            if (int.TryParse(editTreatmentId, out int idx) && idx >= 0 && idx < Treatments.Count)
            {
                Treatments[idx].Details = details;
                Treatments[idx].Hospital = hospital;
                Treatments[idx].Provider = provider;
                Treatments[idx].Patient = patient;
                Treatments[idx].DateTimeDischarged = DateTime.TryParse(dateTimeDischarged, out var dt) ? dt : (DateTime?)null;
            }
        }
        else
        {
            Treatments.Add(new Treatment
            {
                Details = details,
                Hospital = hospital,
                Provider = provider,
                Patient = patient,
                DateTimeDischarged = DateTime.TryParse(dateTimeDischarged, out var dt) ? dt : (DateTime?)null
            });
        }
        _treatmentService.SaveTreatments(Treatments);
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