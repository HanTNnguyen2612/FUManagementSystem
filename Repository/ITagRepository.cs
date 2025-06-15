using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetTags();
        Tag GetTagById(int id);
        void SaveTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(Tag tag);
    }
}