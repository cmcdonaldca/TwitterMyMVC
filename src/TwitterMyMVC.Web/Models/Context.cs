using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TwitterMyMVC.Web.Models
{
    /// <summary>
    /// The context of the data storage
    /// </summary>
    public partial class TwitterMyMVCContext : DbContext
    {
        public TwitterMyMVCContext():
            base("TwitterMyMVC")
        {
        }

        public DbSet<Tweeter> Tweeters { get; set; }
        public DbSet<Search> Searches { get; set; }
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<SearchResult> SearchResults { get; set; }
    }

    /// <summary>
    /// Initialize the DB
    /// </summary>
    public class TwitterMyMVCContextInitializer : DropCreateDatabaseAlways<TwitterMyMVCContext>
    {
        protected override void Seed(TwitterMyMVCContext context)
        {
        }
    }

}