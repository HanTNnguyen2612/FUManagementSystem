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
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.Categories
                        .Include(c => c.ParentCategory)
                        .Include(c => c.InverseParentCategory)
                        .Where(c => c.IsActive == true)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting categories: " + e.Message);
            }
        }

        public static Category GetCategoryById(short id)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.Categories
                        .Include(c => c.ParentCategory)
                        .Include(c => c.InverseParentCategory)
                        .FirstOrDefault(c => c.CategoryId == id && c.IsActive == true);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting category by ID: " + e.Message);
            }
        }

        public static List<Category> SearchCategories(string keyword)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.Categories
                        .Include(c => c.ParentCategory)
                        .Where(c => c.IsActive == true &&
                                   (c.CategoryName.Contains(keyword) || c.CategoryDesciption.Contains(keyword)))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error searching categories: " + e.Message);
            }
        }

        public static void SaveCategory(Category category)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error saving category: " + e.Message);
            }
        }

        public static void UpdateCategory(Category category)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    var existingCategory = db.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (existingCategory == null)
                        throw new Exception("Category not found.");

                    existingCategory.CategoryName = category.CategoryName;
                    existingCategory.CategoryDesciption = category.CategoryDesciption;
                    existingCategory.ParentCategoryId = category.ParentCategoryId;
                    existingCategory.IsActive = category.IsActive;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error updating category: " + e.Message);
            }
        }

        public static void DeleteCategory(Category category)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    if (db.NewsArticles.Any(n => n.CategoryId == category.CategoryId))
                        throw new Exception("Cannot delete category as it is associated with news articles.");

                    var existingCategory = db.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (existingCategory == null)
                        throw new Exception("Category not found.");

                    db.Categories.Remove(existingCategory);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error deleting category: " + e.Message);
            }
        }
    }
}