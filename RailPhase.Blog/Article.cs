using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

using MarkdownSharp;
using YamlDotNet.Serialization;
using RailPhase;
using RailPhase.Templates;

namespace RailPhase.Blog
{
    /// <summary>
    /// Represents a blog article.
    /// </summary>
    public class Article
    {
        public string Slug { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public DateTime PublishDate { get; set; }
        
        /// <summary>
        /// View method for articles. Finds the article corresponding to the path in the given request and renders that article.
        /// </summary>
        public static HttpResponse View(HttpRequest request)
        {
            // Get the article slug from the URL pattern match
            var articleSlug = request.PatternMatch["articleSlug"];

            // Return a 404 error if the article is unknown
            if (!Article.ArticlesByFilename.ContainsKey(articleSlug))
                return Pages.NotFound(request);

            // Otherwise, get the article and render the article page
            var article = Article.ArticlesByFilename[articleSlug];
            var render = Template.FromFile("templates/article.html");
            return new HttpResponse(render(article));
        }

        public static Dictionary<string, Article> ArticlesByFilename = new Dictionary<string, Article>();

        /// <summary>
        /// Static Constructor: Load all articles into ArticlesByFilename
        /// </summary>
        static Article()
        {
            // Get all .yml files in the articles/ directory
            var articleFiles = Directory.EnumerateFiles("./articles", "*.yml", SearchOption.AllDirectories);

            // Load all article files
            foreach (var filename in articleFiles)
            {
                var articlePath = Utils.MakeRelativePath(Path.GetFullPath(filename), Path.GetFullPath("./articles/"));
                articlePath = Utils.SystemToUnixPath(articlePath);
                articlePath = articlePath.Replace(".yml", "");
                ArticlesByFilename[articlePath] = FromYaml(articlePath);
            }
        }

        /// <summary>
        /// Loads an article from a .yml file.
        /// </summary>
        static Article FromYaml(string path)
        {
            // Get the file path from the given URL path
            var filename = "articles/" + path + ".yml";

            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            // Load the article with a YamlDotNet deserializer
            var articleFile = File.OpenRead(filename);
            var deserializer = new Deserializer();
            var article = deserializer.Deserialize<Article>(new StreamReader(articleFile));

            // Set some default values if they are not specified in the file
            if (article.Slug == null)
                article.Slug = path;

            if (article.Summary == null)
            {
                var firstLine = article.Content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                article.Summary = firstLine;
            }

            // Convert the Markdown content to HTML
            var markdownConverter = new Markdown();
            article.Content = markdownConverter.Transform(article.Content);
            article.Summary = Utils.StripHtmlTags(markdownConverter.Transform(article.Summary));

            return article;
        }

    }
}
