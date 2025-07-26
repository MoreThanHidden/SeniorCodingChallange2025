namespace SeniorCodingChallange2025.Data
{
    /// <summary>
    /// CsvLoader class provides functionality to load CSV files and map them to objects.
    /// </summary>
    public static class CsvLoader
    {
        /// <summary>
        /// Loads a CSV file and maps each line to an object of type T using the provided mapping function.
        /// </summary>
        /// <param name="filePath"> Path to the CSV file.</param>
        /// <param name="mapFunc">Function to map each line of the CSV to an object of type T.</param>
        /// <returns>List of objects of type T.</returns>
        public static List<T> LoadCsv<T>(string filePath, Func<string[], T> mapFunc)
        {
            //Read all lines from the CSV file
            var lines = File.ReadAllLines(filePath);
            return lines.Skip(1) // skip header
                .Where(line => !string.IsNullOrWhiteSpace(line))// filter out empty lines
                .Select(line => mapFunc(line.Split(',')))// split by comma and apply mapping function
                .ToList();//Output as a list
        }
    }
    
    /// <remarks>
    /// "Name","Identity"
    /// </remarks>
    public class Hospital
    {
        // Name of the hospital.
        public string Name { get; set; }
        
        // Unique identifier for the hospital.
        public string Identity { get; set; }
    }

    /// <remarks>
    /// "Name","Number","Hospital","Doctor"
    /// </remarks>
    public class Provider
    {
        // Name of the provider.
        public string Name { get; set; }
        
        // Unique number or identifier for the provider.
        public string Number { get; set; }
        
        // Hospital where the provider works.
        public string Hospital { get; set; }
        
        // Is the provider a doctor? Yes or No.
        public bool Doctor { get; set; }
    }

    /// <remarks>
    /// "Medical Reference Number","Patient Name"
    /// </remarks>
    public class Patient
    {
        // Medical reference number for the patient.
        public string MedicalReferenceNumber { get; set; }
        // Name of the patient.
        public string PatientName { get; set; }
    }

    /// <remarks>
    /// "Details","Hospital","Provider","Patient","Date/Time Discharged"
    /// </remarks>
    public class Treatment
    {
        // Description or details of the treatment.
        public string Details { get; set; }
        
        // Hospital name or ID where the treatment was given.
        public string Hospital { get; set; }
        
        // Provider name or ID who gave the treatment.
        public string Provider { get; set; }
        
        // Patient name or ID who received the treatment.
        public string Patient { get; set; }
        
        // Date and time the patient was discharged (as string from CSV).
        public string DateTimeDischarged { get; set; }
    }
}
