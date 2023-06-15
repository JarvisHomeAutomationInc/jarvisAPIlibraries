namespace JarvisMainSite.Models.JarvisAPI
{
    public class GeneralResponse<T>
    {
        public T Data { get; set; }
        public ErrorResponseModel Error { get; set; }
    }
}