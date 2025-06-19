using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
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

        public static void SaveArticle(NewsArticle article, List<int> tagIds)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    db.NewsArticles.Add(article);
                    db.SaveChanges();

                    if (tagIds != null && tagIds.Any())
                    {
                        foreach (var tagId in tagIds)
                        {
                            var tag = db.Tags.Find(tagId);
                            if (tag != null)
                            {
                                article.Tags.Add(tag);
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error saving article: " + e.Message);
            }
        }

        public static void UpdateArticle(NewsArticle article, List<int> tagIds)
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

                    existingArticle.NewsTitle = article.NewsTitle;
                    existingArticle.Headline = article.Headline;
                    existingArticle.NewsContent = article.NewsContent;
                    existingArticle.NewsSource = article.NewsSource;
                    existingArticle.CategoryId = article.CategoryId;
                    existingArticle.NewsStatus = article.NewsStatus;
                    existingArticle.CreatedById = article.CreatedById;
                    existingArticle.UpdatedById = article.UpdatedById;
                    existingArticle.ModifiedDate = article.ModifiedDate;

                    existingArticle.Tags.Clear();
                    if (tagIds != null && tagIds.Any())
                    {
                        foreach (var tagId in tagIds)
                        {
                            var tag = db.Tags.Find(tagId);
                            if (tag != null)
                            {
                                existingArticle.Tags.Add(tag);
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