using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetCategories() => CategoryDAO.GetCategories();

        public Category GetCategoryById(short id) => CategoryDAO.GetCategoryById(id);

        public void SaveCategory(Category category) => CategoryDAO.SaveCategory(category);

        public void UpdateCategory(Category category) => CategoryDAO.UpdateCategory(category);

        public void DeleteCategory(Category category) => CategoryDAO.DeleteCategory(category);

        public List<Category> SearchCategories(string name) => CategoryDAO.SearchCategories(name);

        public bool CanDeleteCategory(short id) => CategoryDAO.CanDeleteCategory(id);
    }
}