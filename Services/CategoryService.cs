using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public List<Category> GetCategories()
        {
            return _repository.GetCategories();
        }

        public Category GetCategoryById(short id)
        {
            return _repository.GetCategoryById(id);
        }

        public List<Category> SearchCategories(string searchKeyword)
        {
            return _repository.SearchCategories(searchKeyword);
        }

        public void SaveCategory(Category category)
        {
            _repository.SaveCategory(category);
        }

        public void UpdateCategory(Category category)
        {
            _repository.UpdateCategory(category);
        }

        public void DeleteCategory(Category category)
        {
            _repository.DeleteCategory(category);
        }
    }
}