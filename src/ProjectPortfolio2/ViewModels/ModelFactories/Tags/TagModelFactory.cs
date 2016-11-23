using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModels.Models;
using ProjectPortfolio2.ViewModels.Partials;
using Microsoft.AspNetCore.Mvc;

namespace ProjectPortfolio2.ViewModels
{
    public class TagModelFactory
    {
        public static TagViewModel Map(Tag tag, IUrlHelper url)
        {
            return new TagViewModel
            {
                Title = tag.Title,
                Url = url.Link(Config.PostsRoute, new { searchFor = tag.Title })
            };
        }
    }
}
