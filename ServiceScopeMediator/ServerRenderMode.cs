using Microsoft.AspNetCore.Components.Web;

namespace ServiceScopeMediator;

public static class ServerRenderMode
{
    public static readonly InteractiveServerRenderMode Mode = new(false);
}