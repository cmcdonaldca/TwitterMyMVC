using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

/**
 * 
 * All Data Entities are in this one file for convenience
 * 
 */
namespace TwitterMyMVC.Web.Models
{
    /// <summary>
    /// Someone who authors tweets
    /// </summary>
    public class Tweeter
    {
        public Tweeter()
        {
            this.Tweets = new HashSet<Tweet>();
        }

        /// <summary>
        /// Primary key of this entity
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int TweeterId { get; set; }

        /// <summary>
        /// id of this entity on Twitter
        /// </summary>
        public long? TwitterId { get; set; }

        /// <summary>
        /// Screen Name of the tweeter
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// URL to the Profile Image for this Tweeter
        /// </summary>
        public string ProfileImageLocation { get; set; }

        /// <summary>
        /// This Authors Collections of Tweets
        /// </summary>
        public virtual ICollection<Tweet> Tweets { get; private set; }
    }

    /// <summary>
    /// The tweet of a Tweeter
    /// </summary>
    public class Tweet
    {
        public Tweet()
        {
            this.SearchResults = new HashSet<SearchResult>();
        }

        /// <summary>
        /// Primary Key of this entity
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int TweetId { get; set; }

        /// <summary>
        /// Foreign Key to the Tweeter Entity
        /// </summary>
        public int TweeterId { get; set; }

        /// <summary>
        /// id of this entity on Twitter
        /// </summary>
        public long? TwitterId { get; set; }

        /// <summary>
        /// The contents of the tweet
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The author of the Tweet
        /// </summary>
        public virtual Tweeter Tweeter { get; set; }

        public virtual ICollection<SearchResult> SearchResults { get; private set; }
    }

    /// <summary>
    /// A search of a certain query string
    /// </summary>
    public class Search
    {
        public Search()
        {
            this.SearchResults = new HashSet<SearchResult>();
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int SearchId { get; set; }
        
        public string QueryString { get; set; }

        public virtual ICollection<SearchResult> SearchResults { get; private set; }
    }

    /// <summary>
    /// the results of a search (i.e. the tweets that match a search)
    /// </summary>
    public class SearchResult
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.DatabaseGeneratedOption.Identity)]
        public int SearchResultId { get; set; }
        
        public int TweetId { get; set; }
        public int SearchId { get; set; }

        public virtual Tweet Tweet { get; set; }
        public virtual Search Search { get; set; }
    }


    public class TopTweeter
    {
        /// <summary>
        /// Primary key of this entity
        /// </summary>
        public int TweeterId { get; set; }

        /// <summary>
        /// id of this entity on Twitter
        /// </summary>
        public long? TwitterId { get; set; }

        /// <summary>
        /// Screen Name of the tweeter
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// URL to the Profile Image for this Tweeter
        /// </summary>
        public string ProfileImageLocation { get; set; }

        /// <summary>
        /// Number of Tweets that appeared in search results
        /// </summary>
        public int NumTweets { get; set; }
    }
}