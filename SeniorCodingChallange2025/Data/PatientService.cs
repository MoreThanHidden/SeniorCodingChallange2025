namespace SeniorCodingChallange2025.Data
{
    public class PatientService
    {
        private readonly string _patientsPath;
        public PatientService(string patientsPath)
        {
            _patientsPath = patientsPath;
        }

        /// <summary>
        /// Loads all patients from the CSV file and returns a list of valid patients.
        /// </summary>
        /// <returns>List of valid Patient objects.</returns>
        public List<Patient> LoadPatients()
        {
            var allPatients = CsvLoader.LoadCsv(_patientsPath, fields => new Patient
            {
                MedicalReferenceNumber = fields[0].Trim('"').Trim(),
                PatientName = fields[1].Trim('"').Trim()
            });
            return allPatients.Where(IsValidPatient).ToList();
        }

        /// <summary>
        /// Checks if a patient object is valid based on MedicalReferenceNumber, PatientName, and name validity.
        /// </summary>
        /// <param name="p">The patient to validate.</param>
        /// <returns>True if the patient is valid, otherwise false.</returns>
        public bool IsValidPatient(Patient p)
        {
            return !string.IsNullOrWhiteSpace(p.MedicalReferenceNumber)
                && !string.IsNullOrWhiteSpace(p.PatientName)
                && IsValidName(p.PatientName);
        }

        /// <summary>
        /// Validates a patient's name. Name must have at least two words, at least 5 letters, and only contain letters, spaces, apostrophes, or hyphens.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <returns>True if the name is valid, otherwise false.</returns>
        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2) return false;
            var alphaCount = name.Count(char.IsLetter);
            if (alphaCount < 5) return false;
            foreach (var c in name)
            {
                if (!(char.IsLetter(c) || c == ' ' || c == '\'' || c == '-'))
                    return false;
            }
            return true;
        }
    }
}
