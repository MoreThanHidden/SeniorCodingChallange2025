namespace SeniorCodingChallange2025.Data
{
    /// <summary>
    /// Service for loading and validating hospitals from a CSV file.
    /// </summary>
    public class HospitalService
    {
        private readonly string _hospitalsPath;
        /// <summary>
        /// Initializes a new instance of the <see cref="HospitalService"/> class.
        /// </summary>
        /// <param name="hospitalsPath">The path to the hospitals CSV file.</param>
        public HospitalService(string hospitalsPath)
        {
            _hospitalsPath = hospitalsPath;
        }

        /// <summary>
        /// Loads and returns a list of valid hospitals from the CSV file.
        /// </summary>
        /// <returns>List of valid <see cref="Hospital"/> objects.</returns>
        public List<Hospital> LoadHospitals()
        {
            var allHospitals = CsvLoader.LoadCsv(_hospitalsPath, fields => new Hospital
            {
                Name = fields[0].Trim('"'),
                Identity = fields[1].Trim('"')
            });
            return allHospitals.Where(IsValidHospital).ToList();
        }

        /// <summary>
        /// Determines whether the specified hospital is valid.
        /// </summary>
        /// <param name="h">The hospital to validate.</param>
        /// <returns><c>true</c> if the hospital is valid; otherwise, <c>false</c>.</returns>
        public bool IsValidHospital(Hospital h)
        {
            return !string.IsNullOrWhiteSpace(h.Name) && !string.IsNullOrWhiteSpace(h.Identity);
        }
    }
}
