using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public List<NewsArticle> GetArticles() => NewsArticleDAO.GetArticles();

        public NewsArticle GetArticleById(string id) => NewsArticleDAO.GetArticleById(id);

        public List<NewsArticle> SearchArticles(string keyword) => NewsArticleDAO.SearchArticles(keyword);

        public void SaveArticle(NewsArticle article, List<int> tagIds) => NewsArticleDAO.SaveArticle(article, tagIds);

        public void UpdateArticle(NewsArticle article, List<int> tagIds) => NewsArticleDAO.UpdateArticle(article, tagIds);

        public void DeleteArticle(NewsArticle article) => NewsArticleDAO.DeleteArticle(article);
    }
}