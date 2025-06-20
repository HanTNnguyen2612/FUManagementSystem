using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface INewsArticleRepository
    {
        List<NewsArticle> GetArticles();
        NewsArticle GetArticleById(string id);
        List<NewsArticle> SearchArticles(string keyword);
        void SaveArticle(NewsArticle article);
        void UpdateArticle(NewsArticle article);
        void DeleteArticle(NewsArticle article);
        List<NewsArticle> GenerateReport(DateTime startDate, DateTime endDate);
    }
}