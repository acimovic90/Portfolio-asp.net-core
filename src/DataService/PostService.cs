﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using DomainModels.Models;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses;

namespace DataService
{
    public class PostService : IPostService
    {


        public Post GetPostById(int postId)
        {
            using (var db = new SovaContext())
            {
                Post post = db.Posts
                    .FromSql("call getSinglePost({0})", postId).FirstOrDefault();

                db.Users.FirstOrDefault(u => u.Id == post.UserId);

                db.Comments.Where(c => c.PostId == postId).ToList().FirstOrDefault();

                //Get answers
                var answers = GetAnswers(postId);
                //Sort answer by answer and accepted answer
                foreach (var answer in answers)
                {
                    if (answer.PostId == post.AcceptedAnswerId)
                    {
                        post.AcceptedAnswer = answer;
                    }
                    else
                    {
                        post.Answers.Add(answer);
                    }
                }

                post.TagsList = (List<Tag>)GetTags(postId);
                post.RelatedPostsLists = (List<RelatedPost>)GetRelatedPosts(postId);

                return post;
            }

        }


        public IList<Comment> GetComments(int postId)
        {
            using (var db = new SovaContext())
            {
                var result = db.Comments.FromSql("call getComments({0})", postId);

                return result.ToList();

            }
        }

        

        public IList<Post> GetAnswers(int postId)
        {
            using (var db = new SovaContext())
            {
                var result = db.Posts.FromSql("call getAnswers({0})", postId);

                var userIds = new List<int>(result.Select(u => u.UserId));
                var userList = GetListOfUsers(userIds);

                foreach (var post in result)
                {
                    foreach (var user in userList)
                    {
                        try
                        {
                            if (user.Id == post.UserId)
                            {
                                post.User = user;
                            }
                        }
                        catch (Exception)
                        {
                            //catch and skip exception if user is not found
                        }
                    }

                    var commentList = GetComments(Convert.ToInt32(post.PostId));
                    var commentUserIds = new List<int>(commentList.Select(u => u.UserId));

                    var userList2 = GetListOfUsers(commentUserIds);

                    foreach (var comment in commentList)
                    {
                        try
                        {
                            //comment.User = db.Users.FirstOrDefault(u => u.Id == userId);
                            comment.User = userList2.FirstOrDefault(u => u.Id == comment.UserId);
                        }
                        catch (Exception)
                        {
                            //catch exception if user is not found
                        }
                    }

                    post.Comments = (List<Comment>)commentList;
                }


                return result.ToList();

            }

        }



        public IList<Tag> GetTags(int postId)
        {
            using (var db = new SovaContext())
            {
                var result = db.Tags.FromSql("call getPostTags({0})", postId);

                var tagList = new List<Tag>();
                foreach (var tag in result)
                {
                    tagList.Add(tag);
                }
                return tagList;

            }

        }
        public int GetNumberOfPosts()
        {
            using (var db = new SovaContext())
            {

                var postCount = (from p in db.Posts
                    where p.PostTypeId == 1
                    select p).Count();


                return postCount;
            }
        }
        public IList<Post> GetPosts(int page, int pageSize)
        {
            using (var db = new SovaContext())
            {
                return db.Posts 
                    .Where(x=> x.PostTypeId == 1)                 
                    .Skip(page * pageSize)
                    .Take(pageSize)                   
                    .ToList();
            }
        }

        public IList<User> GetListOfUsers(List<int> userIds)
        {
            var userList = new List<User>();
            using (var db = new SovaContext())
            {
                foreach (var userId in userIds)
                {
                    var user = db.Users.FirstOrDefault(c => c.Id == userId);
                    userList.Add(user);
                }
            }
            return userList;
        }
        public IList<RelatedPost> GetRelatedPosts(int postId)
        {
            using (var db = new SovaContext())
            {
                var result = db.RelatedPosts.FromSql("call getRelatedPosts({0})", postId);

                var relatedPostList = new List<RelatedPost>();
                foreach (var post in result)
                {
                    relatedPostList.Add(post);
                }
                return relatedPostList;


            }
        }
    }
}