using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface INewsArticleRepository
    {
        List<NewsArticle> GetNewsArticles();
        List<NewsArticle> GetNewsArticlesByStaff(short staffId);
        NewsArticle GetNewsArticleById(string id);
        void SaveNewsArticle(NewsArticle article, List<int> tagIds);
        void UpdateNewsArticle(NewsArticle article, List<int> tagIds);
        void DeleteNewsArticle(NewsArticle article);
    }
}