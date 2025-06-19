using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        public List<Tag> GetTags() => TagDAO.GetTags();

        public Tag GetTagById(int id) => TagDAO.GetTagById(id);

        public List<Tag> SearchTags(string keyword) => TagDAO.SearchTags(keyword);

        public void SaveTag(Tag tag) => TagDAO.SaveTag(tag);

        public void UpdateTag(Tag tag) => TagDAO.UpdateTag(tag);

        public void DeleteTag(Tag tag) => TagDAO.DeleteTag(tag);
    }
}