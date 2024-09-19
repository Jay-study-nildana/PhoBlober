namespace AzureBlobWebAPIDemo.DTO
{
    public class GetAllContainersDTO
    {
        public List<string> ContainerIds { get; set; }
        public int ContainerCount { get; set; }

        public GetAllContainersDTO() { 
            ContainerIds = new List<string>();
            ContainerCount = 0;
        }
    }
}
