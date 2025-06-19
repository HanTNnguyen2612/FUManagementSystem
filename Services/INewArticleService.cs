using BusinessObjects;
using System.Collections.Generic;

namespace Services
{
    public interface INewsArticleService
    {
        List<NewsArticle> GetArticles();
        NewsArticle GetArticleById(string id);
        List<NewsArticle> SearchArticles(string keyword);
        void SaveArticle(NewsArticle article, List<int> tagIds);
        void UpdateArticle(NewsArticle article, List<int> tagIds);
        void DeleteArticle(NewsArticle article);
    }
}