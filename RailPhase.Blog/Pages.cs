using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RailPhase.Templates;

namespace RailPhase.Blog
{
    static class Pages
    {
        public static HttpResponse Frontpage(HttpRequest request)
        {
            var render = Template.FromFile("templates/frontpage.html");

            // Get the 5 latest articles
            var latestArticles =
                (from article in Article.ArticlesByFilename.Values
                 orderby article.PublishDate descending
                 select article)
                .Take(5);

            var context = new ArticleList
            {
                Articles = latestArticles
            };

            return new HttpResponse(render(context));
        }

        public static HttpResponse RssFeed(HttpRequest request)
        {
            var render = Template.FromFile("templates/rss-feed.xml");

            // Get all articles, order them by date
            var articles =
                 from article in Article.ArticlesByFilename.Values
                 orderby article.PublishDate descending
                 select article;

            var context = new ArticleList
            {
                Articles = articles
            };

            return new HttpResponse(render(context), contentType: "application/rss+xml");
        }

        public static HttpResponse Archive(HttpRequest request)
        {
            var render = Template.FromFile("templates/archive.html");
            var articles =
                 from article in Article.ArticlesByFilename.Values
                 orderby article.PublishDate descending
                 select article;

            var context = new ArticleList
            {
                Articles = articles
            };

            return new HttpResponse(render(context));
        }

        public static HttpResponse NotFound(HttpRequest request)
        {
            var render = Template.FromFile("templates/404.html");

            return new HttpResponse(render(request), status: "404 NOT FOUND");
        }
    }

    public class ArticleList
    {
        public IEnumerable<Article> Articles;
    }

}
