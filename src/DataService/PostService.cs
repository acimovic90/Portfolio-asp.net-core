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



                //Get comments
                //post.Comments = GetComments(postId);


                //Get answers
                var answers = GetAnswers(postId);

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

                return post;
            }

        }

        private List<Comment> GetComments(int postId)
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
                var userList = getListOfUsers(userIds);

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
                            //catch exception if user is not found
                        }
                    }

                    var commentList = GetComments(Convert.ToInt32(post.PostId));
                    var commentUserIds = new List<int>(commentList.Select(u => u.UserId));

                    var userList2 = getListOfUsers(commentUserIds);

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

                    post.Comments = commentList;

                }


                return result.ToList();

            }

        }

        public List<User> getListOfUsers(List<int> userIds)
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

    }
}