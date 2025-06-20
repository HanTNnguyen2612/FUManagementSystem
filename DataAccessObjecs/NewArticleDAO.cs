using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccesser
{
    public class NewsArticleDAO
    {
        public static List<NewsArticle> GetArticles()
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.NewsArticles
                        .Include(n => n.Category)
                        .Include(n => n.CreatedBy)
                        .Include(n => n.Tags)
                        .Where(n => n.NewsStatus == true)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting articles: " + e.Message);
            }
        }

        public static NewsArticle GetArticleById(string id)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.NewsArticles
                        .Include(n => n.Category)
                        .Include(n => n.CreatedBy)
                        .Include(n => n.Tags)
                        .FirstOrDefault(n => n.NewsArticleId == id && n.NewsStatus == true);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting article by ID: " + e.Message);
            }
        }

        public static List<NewsArticle> SearchArticles(string keyword)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.NewsArticles
                        .Include(n => n.Category)
                        .Include(n => n.CreatedBy)
                        .Include(n => n.Tags)
                        .Where(n => n.NewsStatus == true &&
                                   (n.NewsTitle.Contains(keyword) ||
                                    n.Headline.Contains(keyword) ||
                                    n.NewsContent.Contains(keyword)))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error searching articles: " + e.Message);
            }
        }

        public static void SaveArticle(NewsArticle article)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    // Attach tags to context
                    if (article.Tags != null && article.Tags.Any())
                    {
                        var tagIds = article.Tags.Select(t => t.TagId).ToList();
                        article.Tags = db.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                    }
                    db.NewsArticles.Add(article);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error saving article: " + e.Message);
            }
        }

        public static void UpdateArticle(NewsArticle article)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    var existingArticle = db.NewsArticles
                        .Include(n => n.Tags)
                        .FirstOrDefault(n => n.NewsArticleId == article.NewsArticleId);
                    if (existingArticle == null)
                        throw new Exception("Article not found.");

                    // Update properties
                    existingArticle.NewsTitle = article.NewsTitle;
                    existingArticle.Headline = article.Headline;
                    existingArticle.NewsContent = article.NewsContent;
                    existingArticle.NewsSource = article.NewsSource;
                    existingArticle.CategoryId = article.CategoryId;
                    existingArticle.NewsStatus = article.NewsStatus;
                    existingArticle.CreatedById = article.CreatedById;
                    existingArticle.UpdatedById = article.UpdatedById;
                    existingArticle.ModifiedDate = article.ModifiedDate;
                    existingArticle.CreatedDate = article.CreatedDate;

                    // Clear existing tags
                    existingArticle.Tags.Clear();

                    // Add new tags
                    if (article.Tags != null && article.Tags.Any())
                    {
                        foreach (var tag in article.Tags)
                        {
                            var dbTag = db.Tags.Find(tag.TagId);
                            if (dbTag != null)
                            {
                                existingArticle.Tags.Add(dbTag);
                            }
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error updating article: " + e.Message);
            }
        }

        public static void DeleteArticle(NewsArticle article)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    var existingArticle = db.NewsArticles
                        .Include(n => n.Tags)
                        .FirstOrDefault(n => n.NewsArticleId == article.NewsArticleId);
                    if (existingArticle == null)
                        throw new Exception("Article not found.");

                    existingArticle.Tags.Clear();
                    db.NewsArticles.Remove(existingArticle);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error deleting article: " + e.Message);
            }
        }
    }
}