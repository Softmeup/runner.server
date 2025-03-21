using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class SharedData
{
    public List<WorkspaceFolder> RootFolders { get; } = [];

    public Dictionary<DocumentUri, string> Content { get; } = [];

    public Dictionary<DocumentUri, string> Types { get; } = [];

    public Dictionary<DocumentUri, string> Schema { get; } = [];

    public OmniSharp.Extensions.LanguageServer.Protocol.Server.ILanguageServer? Server { get; set; }
}
