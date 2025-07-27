namespace SeniorCodingChallange2025.Data
{
    /// <summary>
    /// Service for loading and validating providers from a CSV file.
    /// </summary>
    public class ProviderService
    {
        private readonly string _providersPath;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderService"/> class.
        /// </summary>
        /// <param name="providersPath">The path to the providers CSV file.</param>
        public ProviderService(string providersPath)
        {
            _providersPath = providersPath;
        }

        /// <summary>
        /// Loads and returns a list of valid providers from the CSV file.
        /// </summary>
        /// <returns>List of valid <see cref="Provider"/> objects.</returns>
        public List<Provider> LoadProviders()
        {
            var allProviders = CsvLoader.LoadCsv(_providersPath, fields => new Provider
            {
                Name = fields[0].Trim('"').Trim(),
                Number = fields[1].Trim('"').Trim(),
                Hospital = fields[2].Trim('"').Trim(),
                Doctor = fields[3].Trim('"').Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase)
            });
            return allProviders.Where(IsValidProvider).ToList();
        }

        /// <summary>
        /// Determines whether the specified provider is valid.
        /// </summary>
        /// <param name="p">The provider to validate.</param>
        /// <returns><c>true</c> if the provider is valid; otherwise, <c>false</c>.</returns>
        public bool IsValidProvider(Provider p)
        {
            return !string.IsNullOrWhiteSpace(p.Name)
                && !string.IsNullOrWhiteSpace(p.Number)
                && IsValidName(p.Name);
        }

        /// <summary>
        /// Determines whether the specified name is valid according to business rules.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <returns><c>true</c> if the name is valid; otherwise, <c>false</c>.</returns>
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
