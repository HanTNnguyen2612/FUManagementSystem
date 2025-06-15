using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public List<NewsArticle> GetNewsArticles() => NewsArticleDAO.GetNewsArticles();

        public List<NewsArticle> GetNewsArticlesByStaff(short staffId) => NewsArticleDAO.GetNewsArticlesByStaff(staffId);

        public NewsArticle GetNewsArticleById(string id) => NewsArticleDAO.GetNewsArticleById(id);

        public void SaveNewsArticle(NewsArticle article, List<int> tagIds) => NewsArticleDAO.SaveNewsArticle(article, tagIds);

        public void UpdateNewsArticle(NewsArticle article, List<int> tagIds) => NewsArticleDAO.UpdateNewsArticle(article, tagIds);

        public void DeleteNewsArticle(NewsArticle article) => NewsArticleDAO.DeleteNewsArticle(article);
    }
}