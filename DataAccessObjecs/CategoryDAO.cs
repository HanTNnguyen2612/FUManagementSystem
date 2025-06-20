using BusinessObjects;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccesser
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
                        .FirstOrDefault(c => c.CategoryId == id);
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
                        .Where(c => c.CategoryName.Contains(keyword) ||
                                  c.CategoryDesciption.Contains(keyword))
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
                    // Validate ParentCategoryId
                    if (category.ParentCategoryId.HasValue &&
                        db.Categories.Find(category.ParentCategoryId) == null)
                    {
                        throw new Exception("Parent category does not exist.");
                    }

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
                    var existingCategory = db.Categories
                        .Include(c => c.InverseParentCategory)
                        .FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (existingCategory == null)
                        throw new Exception("Category not found.");

                    // Validate ParentCategoryId
                    if (category.ParentCategoryId.HasValue &&
                        db.Categories.Find(category.ParentCategoryId) == null)
                    {
                        throw new Exception("Parent category does not exist.");
                    }

                    // Prevent self-reference
                    if (category.ParentCategoryId == category.CategoryId)
                    {
                        throw new Exception("Category cannot be its own parent.");
                    }

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
                    var existingCategory = db.Categories
                        .Include(c => c.InverseParentCategory)
                        .Include(c => c.NewsArticles)
                        .FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (existingCategory == null)
                        throw new Exception("Category not found.");

                    // Check if category is referenced
                    if (existingCategory.InverseParentCategory.Any())
                        throw new Exception("Cannot delete category with subcategories.");
                    if (existingCategory.NewsArticles.Any())
                        throw new Exception("Cannot delete category with associated news articles.");

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