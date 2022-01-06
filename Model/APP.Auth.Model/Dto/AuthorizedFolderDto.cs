using APP.Base.Model.Enum;


namespace APP.Auth.Model
{
    public class AuthorizedFolderDto
    {
        public long FolderId { get; set; }
        public Authority Authority { get; set; }

    }
}
