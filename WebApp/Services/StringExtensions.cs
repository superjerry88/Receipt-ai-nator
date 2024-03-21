namespace WebApp.Services
{
    public static class StringExtensions
    {
        public static string FixString(this object? input, bool isCurrency = false)
        {
            if (input == null)
            {
                return "Not Available";
            }

            var word = input.ToString();

            if (isCurrency)
            {
                word = $"RM{String.Format("{0:0.00}", Convert.ToDecimal((double)input * 100) / 100)}";
            }

            return string.IsNullOrEmpty(word) ? "Not Available" : word;
        }
    }
}
