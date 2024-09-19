namespace AzureBlobWebAPIDemo.DTO
{
    //https://learn.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata
    public class ContainerNameRulesDTO
    {
        public List<string> containernamingrules { get; set; }

        public ContainerNameRulesDTO() { 
            containernamingrules = new List<string>();

            containernamingrules.Add("Container names must start or end with a letter or number, and can contain only letters, numbers, and the hyphen/minus (-) character.");
            containernamingrules.Add("Every hyphen/minus (-) character must be immediately preceded and followed by a letter or number; consecutive hyphens aren't permitted in container names.");
            containernamingrules.Add("All letters in a container name must be lowercase.");
            containernamingrules.Add("Container names must be from 3 through 63 characters long.");
        }
    }
}
