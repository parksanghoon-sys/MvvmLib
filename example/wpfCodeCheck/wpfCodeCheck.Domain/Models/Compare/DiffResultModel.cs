namespace wpfCodeCheck.Domain.Models.Compare
{
    public record DiffResultModel<T>
    {
        private List<T>? _compareResults = new List<T>();

        public List<T> CompareResults
        {
            get => _compareResults ?? throw new ArgumentNullException(nameof(T), "CompareResults cannot be null");
            set => _compareResults = value ?? throw new ArgumentNullException(nameof(value), "CompareResults cannot be null");
        }
    }
}