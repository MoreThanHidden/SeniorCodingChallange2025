namespace SeniorCodingChallange2025.Data
{
    /// <summary>
    /// Service class for managing treatment data.
    /// </summary>
    public class TreatmentService
    {
        private readonly string _treatmentsPath;
        private readonly List<Hospital> _hospitals;
        private readonly List<Provider> _providers;
        private readonly List<Patient> _patients;

        /// <summary>
        /// Initializes a new instance of the TreatmentService class.
        /// </summary>
        /// <param name="treatmentsPath">Path to the treatments CSV file.</param>
        /// <param name="hospitals">List of hospitals.</param>
        /// <param name="providers">List of providers.</param>
        /// <param name="patients">List of patients.</param>
        public TreatmentService(string treatmentsPath, List<Hospital> hospitals, List<Provider> providers, List<Patient> patients)
        {
            _treatmentsPath = treatmentsPath;
            _hospitals = hospitals;
            _providers = providers;
            _patients = patients;
        }

        /// <summary>
        /// Loads all treatments from the CSV file and returns a list of valid treatments.
        /// </summary>
        /// <returns>List of valid Treatment objects.</returns>
        public List<Treatment> LoadTreatments()
        {
            var allTreatments = CsvLoader.LoadCsv(_treatmentsPath, fields => new Treatment
            {
                Details = fields[0].Trim('"').Trim(),
                Hospital = fields[1].Trim('"').Trim(),
                Provider = fields[2].Trim('"').Trim(),
                Patient = fields[3].Trim('"').Trim(),
                DateTimeDischarged = DateTime.TryParse(fields[4].Trim('"').Trim(), out var dt) ? dt : (DateTime?)null
            });
            return allTreatments
                .Where(IsValidTreatment)
                .ToList();
        }

        /// <summary>
        /// Saves the provided list of treatments to the CSV file, overwriting existing data.
        /// </summary>
        /// <param name="treatments">List of treatments to save.</param>
        public void SaveTreatments(List<Treatment> treatments)
        {
            using (var writer = new StreamWriter(_treatmentsPath, false))
            {
                foreach (var t in treatments)
                {
                    writer.WriteLine($"\"{t.Details}\",\"{t.Hospital}\",\"{t.Provider}\",\"{t.Patient}\",\"{(t.DateTimeDischarged.HasValue ? t.DateTimeDischarged.Value.ToString("yyyy-MM-dd HH:mm") : "" )}\"");
                }
            }
        }

        /// <summary>
        /// Validates the given treatment object.
        /// </summary>
        /// <param name="t">The treatment object to validate.</param>
        /// <returns>True if the treatment is valid; otherwise, false.</returns>
        public bool IsValidTreatment(Treatment t)
        {
            return !string.IsNullOrWhiteSpace(t.Hospital)
                && !string.IsNullOrWhiteSpace(t.Patient)
                && _hospitals.Any(h => h.Name.Equals(t.Hospital, StringComparison.OrdinalIgnoreCase))
                && _patients.Any(p => p.MedicalReferenceNumber.Equals(t.Patient, StringComparison.OrdinalIgnoreCase))
                && (t.DateTimeDischarged == null || (!string.IsNullOrWhiteSpace(t.Provider) && !string.IsNullOrWhiteSpace(t.Details)))
                && (string.IsNullOrWhiteSpace(t.Provider) || _providers.Any(p => p.Name.Equals(t.Provider, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
