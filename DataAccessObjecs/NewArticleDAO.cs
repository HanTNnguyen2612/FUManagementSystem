using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class NewsArticleDAO
    {
        public static List<NewsArticle> GetNewsArticles()
        {
            var listArticles = new List<NewsArticle>();
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    listArticles = db.NewsArticles
                        .Include(n => n.Tags)
                        .Include(n => n.Category)
                        .Include(n => n.CreatedBy)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting news articles: " + e.Message);
            }
            return listArticles;
        }

        public static List<NewsArticle> GetNewsArticlesByStaff(short staffId)
        {
            var listArticles = new List<NewsArticle>();
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    listArticles = db.NewsArticles
                        .Where(n => n.CreatedById == staffId)
                        .Include(n => n.Tags)
                        .Include(n => n.Category)
                        .Include(n => n.CreatedBy)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting news articles by staff: " + e.Message);
            }
            return listArticles;
        }

        public static NewsArticle GetNewsArticleById(string id)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.NewsArticles
                        .Include(n => n.Tags)
                        .Include(n => n.Category)
                        .Include(n => n.CreatedBy)
                        .FirstOrDefault(n => n.NewsArticleId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting news article: " + e.Message);
            }
        }

        public static void SaveNewsArticle(NewsArticle article, List<int> tagIds)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    context.NewsArticles.Add(article);
                    if (tagIds != null && tagIds.Any())
                    {
                        var tags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                        article.Tags = tags;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void UpdateNewsArticle(NewsArticle article, List<int> tagIds)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingArticle = context.NewsArticles
                        .Include(n => n.Tags)
                        .FirstOrDefault(n => n.NewsArticleId == article.NewsArticleId);
                    if (existingArticle != null)
                    {
                        existingArticle.NewsTitle = article.NewsTitle;
                        existingArticle.Headline = article.Headline;
                        existingArticle.NewsContent = article.NewsContent;
                        existingArticle.NewsSource = article.NewsSource;
                        existingArticle.CategoryId = article.CategoryId;
                        existingArticle.NewsStatus = article.NewsStatus;
                        existingArticle.UpdatedById = article.UpdatedById;
                        existingArticle.ModifiedDate = DateTime.Now;

                        existingArticle.Tags.Clear();
                        if (tagIds != null && tagIds.Any())
                        {
                            var tags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                            existingArticle.Tags = tags;
                        }

                        context.Entry(existingArticle).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteNewsArticle(NewsArticle article)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingArticle = context.NewsArticles
                        .Include(n => n.Tags)
                        .FirstOrDefault(n => n.NewsArticleId == article.NewsArticleId);
                    if (existingArticle != null)
                    {
                        context.NewsArticles.Remove(existingArticle);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}