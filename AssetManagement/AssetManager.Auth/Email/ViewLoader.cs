using System.IO;
using System.Web.Hosting;
using RazorEngine;
using RazorEngine.Templating;

namespace AssetManager.Auth.Email
{
    public class ViewLoader : IViewLoader
    {
        public string RenderViewToString(string viewPath,
            object model = null)
        {
            var template = File.ReadAllText(HostingEnvironment.MapPath(viewPath));
            var result = Engine.Razor.RunCompile(template, viewPath, null, model);
            return result;
        }
    }
}