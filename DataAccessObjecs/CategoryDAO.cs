using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class CategoryDAO
    {
        public static List<Category> GetCategories()
        {
            var listCategories = new List<Category>();
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    listCategories = db.Categories
                        .Include(c => c.ParentCategory)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting categories: " + e.Message);
            }
            return listCategories;
        }

        public static Category GetCategoryById(short id)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.Categories
                        .Include(c => c.ParentCategory)
                        .FirstOrDefault(c => c.CategoryId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting category: " + e.Message);
            }
        }

        public static void SaveCategory(Category category)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    context.Categories.Add(category);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void UpdateCategory(Category category)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (existingCategory != null)
                    {
                        existingCategory.CategoryName = category.CategoryName;
                        existingCategory.CategoryDesciption = category.CategoryDesciption;
                        existingCategory.ParentCategoryId = category.ParentCategoryId;
                        existingCategory.IsActive = category.IsActive;
                        context.Entry(existingCategory).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteCategory(Category category)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingCategory = context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (existingCategory != null)
                    {
                        context.Categories.Remove(existingCategory);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<Category> SearchCategories(string name)
        {
            var listCategories = new List<Category>();
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    listCategories = db.Categories
                        .Where(c => c.CategoryName.Contains(name))
                        .Include(c => c.ParentCategory)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error searching categories: " + e.Message);
            }
            return listCategories;
        }

        public static bool CanDeleteCategory(short id)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return !db.NewsArticles.Any(n => n.CategoryId == id) &&
                           !db.Categories.Any(c => c.ParentCategoryId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error checking category deletion: " + e.Message);
            }
        }
    }
}