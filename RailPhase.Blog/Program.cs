using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

using RailPhase.Templates;
using System.Threading;
using System.Globalization;

namespace RailPhase.Blog
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();

            // Configure the static URLs
            app.AddUrl("/", Pages.Frontpage);
            app.AddUrl("/rss-feed.xml", Pages.RssFeed);
            app.AddUrl("/archive", Pages.Archive);

            // For static pages, you can directly give the path to a template
            app.AddUrl(@"/about", "templates/about.html");

            // Add a URL pattern with a regular expression for the article pages.
            // The expression captures the name of the article so we can access it in Article.View.
            app.AddUrlPattern(@"^/article/(?<articleSlug>[\w\-]+)$", Article.View);
            
            // Serve files from the "/static" folder.
            // In production use, you should add a rule to your webserver configuration that directly
            // serves the static files from this directory, without passing the requests to this app.
            app.AddStaticDirectory("static/");

            // Set a custom 404 error page
            app.NotFoundView = Pages.NotFound;

            // Start accepting requests.
            // This method never returns!
            app.RunTestServer();
        }
    }
}
