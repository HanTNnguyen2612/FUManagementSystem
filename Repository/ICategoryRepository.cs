using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        Category GetCategoryById(short id);
        void SaveCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
        List<Category> SearchCategories(string name);
        bool CanDeleteCategory(short id);
    }
}