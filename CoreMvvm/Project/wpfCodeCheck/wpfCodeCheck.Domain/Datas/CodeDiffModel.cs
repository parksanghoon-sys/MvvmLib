namespace wpfCodeCheck.Domain.Datas
{
    // TODO : 급해서 혹시몰라서 Generic으로 대처하였는데 추후 리펙토링 예정
    // 무조건 Beyond 형식으로 요청으로 인한 변경
    public record CodeDiffReulstModel<T>
    {
        private IList<T>? _compareResults = new List<T>();

        public IList<T> CompareResults
        {
            get => _compareResults ?? throw new ArgumentNullException(nameof(T), "CompareResults cannot be null");
            set => _compareResults = value ?? throw new ArgumentNullException(nameof(value), "CompareResults cannot be null");
        }
    }    
}
