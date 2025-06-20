using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public NewsArticleService(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        public List<NewsArticle> GetArticles()
        {
            return _newsArticleRepository.GetArticles();
        }

        public NewsArticle GetArticleById(string id)
        {
            return _newsArticleRepository.GetArticleById(id);
        }

        public List<NewsArticle> SearchArticles(string keyword)
        {
            return _newsArticleRepository.SearchArticles(keyword);
        }

        public void SaveArticle(NewsArticle article)
        {
            if (!ValidateNewsArticle(article))
                throw new ArgumentException("Invalid article data.");

            _newsArticleRepository.SaveArticle(article);
        }

        public void UpdateArticle(NewsArticle article)
        {
            if (!ValidateNewsArticle(article))
                throw new ArgumentException("Invalid article data.");

            _newsArticleRepository.UpdateArticle(article);
        }

        public void DeleteArticle(NewsArticle article)
        {
            _newsArticleRepository.DeleteArticle(article);
        }

        private bool ValidateNewsArticle(NewsArticle article)
        {
            if (article == null)
                return false;

            if (string.IsNullOrWhiteSpace(article.NewsArticleId) || article.NewsArticleId.Length > 20)
                return false;

            if (string.IsNullOrWhiteSpace(article.Headline) || article.Headline.Length > 150)
                return false;

            if (article.NewsTitle != null && article.NewsTitle.Length > 400)
                return false;

            if (article.NewsContent != null && article.NewsContent.Length > 4000)
                return false;

            if (article.NewsSource != null && article.NewsSource.Length > 400)
                return false;

            return true;
        }
        public List<NewsArticle> GenerateReport(DateTime startDate, DateTime endDate)
        {
            return _newsArticleRepository.GenerateReport(startDate, endDate);
        }
    }
}