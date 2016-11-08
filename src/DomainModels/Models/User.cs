﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("userdisplayname")]
        public string DisplayName { get; set; }
        [Column("usercreationdate")]
        public DateTime CreationDate { get; set; }
        [Column("userlocation")]
        public string Location { get; set; }
        [Column("userage")]
        public int ?Age { get; set; }
        public List<Post> Posts { get; set; }
        public List<Post> FavouritePosts { get; set; }
    }
}
