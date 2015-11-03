using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System;

namespace AppFramework.Email
{
    public class ViewLoader : IViewLoader
    {
        public string RenderViewToString(string viewPath,
            object model = null)
        {
            var templatePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                @"bin\EmailTemplates", 
                viewPath);
            var template = File.ReadAllText(templatePath);
            var result = Engine.Razor.RunCompile(template, viewPath, null, model);
            return result;
        }
    }
}