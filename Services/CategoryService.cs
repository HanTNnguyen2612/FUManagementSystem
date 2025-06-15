using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> GetCategories()
        {
            return _categoryRepository.GetCategories();
        }

        public Category GetCategoryById(short id)
        {
            return _categoryRepository.GetCategoryById(id);
        }

        public void SaveCategory(Category category)
        {
            if (!ValidateCategory(category))
                throw new ArgumentException("Invalid category data.");

            _categoryRepository.SaveCategory(category);
        }

        public void UpdateCategory(Category category)
        {
            if (!ValidateCategory(category))
                throw new ArgumentException("Invalid category data.");

            _categoryRepository.UpdateCategory(category);
        }

        public void DeleteCategory(Category category)
        {
            if (!_categoryRepository.CanDeleteCategory(category.CategoryId))
                throw new InvalidOperationException("Cannot delete category as it is linked to news articles.");

            _categoryRepository.DeleteCategory(category);
        }

        public bool CanDeleteCategory(short id)
        {
            return _categoryRepository.CanDeleteCategory(id);
        }

        public bool ValidateCategory(Category category)
        {
            if (category == null)
                return false;

            if (string.IsNullOrWhiteSpace(category.CategoryName) || category.CategoryName.Length > 100)
                return false;

            if (string.IsNullOrWhiteSpace(category.CategoryDesciption) || category.CategoryDesciption.Length > 250)
                return false;

            return true;
        }

        public List<Category> SearchCategories(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _categoryRepository.GetCategories();

            return _categoryRepository.SearchCategories(name);
        }
    }
}