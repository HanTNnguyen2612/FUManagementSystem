using BusinessObjects;
using System.Collections.Generic;

namespace Services
{
    public interface ICategoryService
    {
        List<Category> GetCategories();
        Category GetCategoryById(short id);
        List<Category> SearchCategories(string keyword);
        void SaveCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
    }
}