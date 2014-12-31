namespace A4EPARC.ViewModels
{
    public class JsonResultModel
    {
        public GetResult GetResult { get; set; }
    }

    public class GetResult
    {
        public string PreContemplationScore { get; set; }

        public string ContemplationScore { get; set; }

        public string ActionScore { get; set; }

        public string Result { get; set; }

    }
}