using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using RazorEngine.Roslyn;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorTemplateEngine
{
    public class TheViewModel
    {
        public string Name;
        public string EncodedImage;
    }
    internal static class RazorTemplateEngine
    {
        public static string Compose(TheViewModel viewModel)
        {
            var service = CreateService();
            var template = ThePresenter.LoadTemplate("TheTemplate.cshtml");
            var html = service.RunCompile(template, "TheTemplate.cshtml", typeof(TheViewModel), viewModel);
            return html;
        }
        internal static IRazorEngineService CreateService()
        {
            var config = new TemplateServiceConfiguration();
            config.Language = Language.CSharp;
            config.CompilerServiceFactory = new RoslynCompilerServiceFactory();
            var service = RazorEngineService.Create(config);
            return service;
        }

        internal static void CompileTemplate<T>(string templateName)
        {
            var text = ThePresenter.LoadTemplate(templateName);
            var service = CreateService();
            service.Compile(text, templateName, typeof(T));
        }
    }

    public static class ThePresenter
    {
        public static string ComposeHtml()
        {
            var service = RazorTemplateEngine.CreateService();

            const string styleTemplateName = "ExampleTemplate.cshtml";
            var styleTempalte = LoadTemplate(styleTemplateName);
            var staticResource = GetEncodedResource();
            var html = service.RunCompile(styleTempalte, styleTemplateName, typeof(TheViewModel), staticResource);
            return html;
        }
        public static string GetEncodedResource()
        {
            using (var pngStream = LoadResource("Capture.PNG"))
            using (var reader = new BinaryReader(pngStream))
            {
                var bytes = reader.ReadAll();
                var encodedPng = System.Convert.ToBase64String(bytes);
                return encodedPng;
            }
        }
        internal static string LoadTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream input = assembly.GetManifestResourceStream($"RazorTemplateEngine.{templateName}"))
            using (StreamReader reader = new StreamReader(input))
            {
                var template = reader.ReadToEnd();
                return template;
            }
        }
        internal static Stream LoadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream($"RazorTemplateEngine.{resourceName}");
        }
        public static byte[] ReadAll(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }
        }
    }


    [TestClass]
    public class ExampleTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var viewModel = new TheViewModel

            {
                Name = "test",
                EncodedImage = ThePresenter.GetEncodedResource(),
            };
            var html = RazorTemplateEngine.Compose(viewModel);
        }
    }
}
