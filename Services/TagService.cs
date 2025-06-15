using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public List<Tag> GetTags()
        {
            return _tagRepository.GetTags();
        }

        public Tag GetTagById(int id)
        {
            return _tagRepository.GetTagById(id);
        }

        public void SaveTag(Tag tag)
        {
            if (!ValidateTag(tag))
                throw new ArgumentException("Invalid tag data.");

            _tagRepository.SaveTag(tag);
        }

        public void UpdateTag(Tag tag)
        {
            if (!ValidateTag(tag))
                throw new ArgumentException("Invalid tag data.");

            _tagRepository.UpdateTag(tag);
        }

        public void DeleteTag(Tag tag)
        {
            if (tag == null)
                throw new ArgumentException("Tag not found.");

            _tagRepository.DeleteTag(tag);
        }

        public bool ValidateTag(Tag tag)
        {
            if (tag == null)
                return false;

            if (string.IsNullOrWhiteSpace(tag.TagName) || tag.TagName.Length > 50)
                return false;

            if (tag.Note != null && tag.Note.Length > 400)
                return false;

            return true;
        }
    }
}