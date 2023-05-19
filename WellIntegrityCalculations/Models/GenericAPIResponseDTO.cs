namespace WellIntegrityCalculations.Models
{
    /// <summary>
    /// Generic API Response object. Contains an statuscode and a Value of type T
    /// </summary>
    /// <typeparam name="T">Type of the returned results</typeparam>
    public class GenericAPIResponseDTO<T>
    {
        /// <summary>
        /// HTTP Status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Actual value of the response
        /// </summary>
        public T ResponseValue { get; set; }
    }
}
