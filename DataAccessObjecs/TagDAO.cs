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
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.Tags.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting tags: " + e.Message);
            }
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
                throw new Exception("Error getting tag by ID: " + e.Message);
            }
        }

        public static List<Tag> SearchTags(string keyword)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    return db.Tags
                        .Where(t => t.TagName.Contains(keyword) || t.Note.Contains(keyword))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error searching tags: " + e.Message);
            }
        }

        public static void SaveTag(Tag tag)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    db.Tags.Add(tag);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error saving tag: " + e.Message);
            }
        }

        public static void UpdateTag(Tag tag)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    var existingTag = db.Tags.FirstOrDefault(t => t.TagId == tag.TagId);
                    if (existingTag == null)
                        throw new Exception("Tag not found.");

                    existingTag.TagName = tag.TagName;
                    existingTag.Note = tag.Note;

                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error updating tag: " + e.Message);
            }
        }

        public static void DeleteTag(Tag tag)
        {
            try
            {
                using (var db = new FunewsManagementContext())
                {
                    if (db.NewsArticles.Any(n => n.Tags.Any(t => t.TagId == tag.TagId)))
                        throw new Exception("Cannot delete tag as it is associated with news articles.");

                    var existingTag = db.Tags.FirstOrDefault(t => t.TagId == tag.TagId);
                    if (existingTag == null)
                        throw new Exception("Tag not found.");

                    db.Tags.Remove(existingTag);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error deleting tag: " + e.Message);
            }
        }
    }
}