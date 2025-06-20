using BusinessObjects;
using DataAccesser;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public List<NewsArticle> GetArticles()
        {
            return NewsArticleDAO.GetArticles();
        }

        public NewsArticle GetArticleById(string id)
        {
            return NewsArticleDAO.GetArticleById(id);
        }

        public List<NewsArticle> SearchArticles(string searchKeyword)
        {
            return NewsArticleDAO.SearchArticles(searchKeyword);
        }

        public void SaveArticle(NewsArticle article)
        {
            NewsArticleDAO.SaveArticle(article);
        }

        public void UpdateArticle(NewsArticle article)
        {
            NewsArticleDAO.UpdateArticle(article);
        }

        public void DeleteArticle(NewsArticle article)
        {
            NewsArticleDAO.DeleteArticle(article);
        }
    }
}