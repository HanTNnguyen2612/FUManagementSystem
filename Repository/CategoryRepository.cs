using BusinessObjects;
using DataAccesser;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetCategories()
        {
            return CategoryDAO.GetCategories();
        }

        public Category GetCategoryById(short id)
        {
            return CategoryDAO.GetCategoryById(id);
        }

        public List<Category> SearchCategories(string searchKeyword)
        {
            return CategoryDAO.SearchCategories(searchKeyword);
        }

        public void SaveCategory(Category category)
        {
            CategoryDAO.SaveCategory(category);
        }

        public void UpdateCategory(Category category)
        {
            CategoryDAO.UpdateCategory(category);
        }

        public void DeleteCategory(Category category)
        {
            CategoryDAO.DeleteCategory(category);
        }
    }
}