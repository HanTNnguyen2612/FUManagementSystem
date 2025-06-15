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

        public List<NewsArticle> GetNewsArticles()
        {
            return _newsArticleRepository.GetNewsArticles();
        }

        public List<NewsArticle> GetNewsArticlesByStaff(short staffId)
        {
            return _newsArticleRepository.GetNewsArticlesByStaff(staffId);
        }

        public NewsArticle GetNewsArticleById(string id)
        {
            return _newsArticleRepository.GetNewsArticleById(id);
        }

        public void SaveNewsArticle(NewsArticle article, List<int> tagIds)
        {
            if (!ValidateNewsArticle(article))
                throw new ArgumentException("Invalid news article data.");

            article.CreatedDate = DateTime.Now;
            article.ModifiedDate = DateTime.Now;
            _newsArticleRepository.SaveNewsArticle(article, tagIds);
        }

        public void UpdateNewsArticle(NewsArticle article, List<int> tagIds)
        {
            if (!ValidateNewsArticle(article))
                throw new ArgumentException("Invalid news article data.");

            article.ModifiedDate = DateTime.Now;
            _newsArticleRepository.UpdateNewsArticle(article, tagIds);
        }

        public void DeleteNewsArticle(NewsArticle article)
        {
            if (article == null)
                throw new ArgumentException("News article not found.");

            _newsArticleRepository.DeleteNewsArticle(article);
        }

        private bool ValidateNewsArticle(NewsArticle article)
        {
            if (article == null)
                return false;

            if (string.IsNullOrWhiteSpace(article.NewsArticleId) || article.NewsArticleId.Length > 20)
                return false;

            if (string.IsNullOrWhiteSpace(article.Headline) || article.Headline.Length > 150)
                return false;

            if (!article.CategoryId.HasValue)
                return false;

            if (article.NewsTitle != null && article.NewsTitle.Length > 400)
                return false;

            if (article.NewsContent != null && article.NewsContent.Length > 4000)
                return false;

            if (article.NewsSource != null && article.NewsSource.Length > 400)
                return false;

            return true;
        }
    }
}