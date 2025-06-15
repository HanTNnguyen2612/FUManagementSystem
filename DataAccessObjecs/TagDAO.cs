using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class TagDAO
    {
        public static List<Tag> GetTags()
        {
            var listTags = new List<Tag>();
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    listTags = db.Tags.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting tags: " + e.Message);
            }
            return listTags;
        }

        public static Tag GetTagById(int id)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.Tags.FirstOrDefault(t => t.TagId == id);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting tag: " + e.Message);
            }
        }

        public static void SaveTag(Tag tag)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    context.Tags.Add(tag);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void UpdateTag(Tag tag)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingTag = context.Tags.FirstOrDefault(t => t.TagId == tag.TagId);
                    if (existingTag != null)
                    {
                        existingTag.TagName = tag.TagName;
                        existingTag.Note = tag.Note;
                        context.Entry(existingTag).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void DeleteTag(Tag tag)
        {
            try
            {
                using (var context = new FunewsManagementContext())
                {
                    var existingTag = context.Tags.FirstOrDefault(t => t.TagId == tag.TagId);
                    if (existingTag != null)
                    {
                        context.Tags.Remove(existingTag);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}