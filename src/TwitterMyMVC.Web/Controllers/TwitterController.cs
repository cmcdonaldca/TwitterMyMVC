using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twitterizer;
using System.Configuration;
using System.Web.Security;

namespace TwitterMyMVC.Web.Controllers
{
    public class TwitterController : Controller
    {
        [Authorize]
        public ActionResult Search(string queryString)
        {
            try
            {
                // recoord the search results in our DB Contect
                using (var db = new TwitterMyMVC.Web.Models.TwitterMyMVCContext())
                {
                    if (!String.IsNullOrWhiteSpace(queryString))
                    {

                        // For simplicity, only get top 5
                        SearchOptions options = new SearchOptions()
                        {
                            PageNumber = 1,
                            NumberPerPage = 5
                        };

                        var searchResponse = TwitterSearch.Search(queryString, options);
                        if (searchResponse.Result == RequestResult.Success)
                        {

                            // check if this search already exists in our DB
                            Models.Search myMVCSearch = (from s in db.Searches
                                                         where s.QueryString == queryString
                                                         select s).FirstOrDefault();
                            if (myMVCSearch == null)
                            {
                                myMVCSearch = new TwitterMyMVC.Web.Models.Search()
                                {
                                    QueryString = queryString
                                };

                                db.Searches.Add(myMVCSearch);
                            }

                            foreach (var tweet in searchResponse.ResponseObject)
                            {
                                // check if this tweet has already been recorded
                                var myMVCTweet = (from t in db.Tweets
                                                  where t.TwitterId == tweet.Id
                                                  select t).FirstOrDefault();

                                if (myMVCTweet == null)
                                {
                                    var myMVCTweeter = (from tr in db.Tweeters
                                                        where tr.TwitterId == tweet.FromUserId
                                                        select tr).FirstOrDefault();
                                    if (myMVCTweeter == null)
                                    {
                                        myMVCTweeter = new Models.Tweeter()
                                        {
                                            ProfileImageLocation = tweet.ProfileImageLocation,
                                            TwitterId = tweet.FromUserId,
                                            ScreenName = tweet.FromUserScreenName
                                        };
                                        db.Tweeters.Add(myMVCTweeter);
                                    }

                                    myMVCTweet = new TwitterMyMVC.Web.Models.Tweet()
                                    {
                                        Text = tweet.Text,
                                        TwitterId = tweet.Id,
                                        Tweeter = myMVCTweeter,
                                    };
                                    db.Tweets.Add(myMVCTweet);
                                }

                                // add this tweet to the search results if it's not already there
                                if (!myMVCSearch.SearchResults.Any(sr => myMVCTweet.TweetId > 0 && sr.Tweet.TweetId == myMVCTweet.TweetId))
                                {
                                    db.SearchResults.Add(new Models.SearchResult()
                                    {
                                        Tweet = myMVCTweet,
                                        Search = myMVCSearch
                                    });
                                }

                                // forced to save changes in loop because, items are getting added twice.  
                                // I'm sure there's a better way, just getting tired...
                                db.SaveChanges();
                            }


                            ViewBag.twitterSearchResults = db.SearchResults
                                .Include("Tweet")
                                .Include("Tweet.Tweeter")
                                .Where(sr => sr.SearchId == myMVCSearch.SearchId).ToList();
                        }
                        else
                        {
                            //LogErrorDetails(serachResponse.RequestUrl, searchResponse.ErrorMessage, searchResponse.Content);
                            //DisplayErrorMessageToUser(searchResponse.ErrorMessage);
                        }
                    }

                    // get the top 5 tweeters
                    // a little rough... but started running out of time
                    ViewBag.topTweeters = (from tweet in db.Tweets
                                           group tweet by new
                                           {
                                               TweeterId = tweet.TweeterId,
                                               ProfileImageLocation = tweet.Tweeter.ProfileImageLocation,
                                               ScreenName = tweet.Tweeter.ScreenName,
                                           } into Tweet
                                           select new Models.TopTweeter()
                                           {
                                               TweeterId = Tweet.Key.TweeterId,
                                               ProfileImageLocation = Tweet.Key.ProfileImageLocation,
                                               ScreenName = Tweet.Key.ScreenName,
                                               NumTweets = Tweet.Count()
                                           }).OrderByDescending(x => x.NumTweets).Take(5).ToList();

                    return View();
                }
            }
            catch (Exception ex)
            {
                return View("Error");
                //LogException(ex);
                //DisplayErrorMessageToUser("There was a problem performing the Twitter Search.");
            }
        }

        public ActionResult Tweeter(int tweeterId)
        {
            try
            {
                // recoord the search results in our DB Contect
                using (var db = new TwitterMyMVC.Web.Models.TwitterMyMVCContext())
                {
                    ViewBag.tweeter = db.Tweeters.Include("Tweets").Include("Tweets.SearchResults.Search").FirstOrDefault(t => t.TweeterId == tweeterId);
                    return View();
                }
            }
            catch (Exception ex)
            {
                //LogException(ex);
                //DisplayErrorMessageToUser("There was a problem performing the Twitter Search.");
                return View("Error");

            }

        }

        /// <summary>
        /// Logs on and Authenticates with Twitter
        /// </summary>
        /// <param name="oauth_token"></param>
        /// <param name="oauth_verifier"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public ActionResult Logon(string oauth_token, string oauth_verifier, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(oauth_token) || string.IsNullOrEmpty(oauth_verifier))
            {
                // If there is no tokens passed to this action, redirect to get authorization
                UriBuilder builder = new UriBuilder(this.Request.Url);
                builder.Query = string.Concat(
                    builder.Query,
                    string.IsNullOrEmpty(builder.Query) ? string.Empty : "&",
                    "ReturnUrl=",
                    ReturnUrl);

                string token = OAuthUtility.GetRequestToken(
                    ConfigurationManager.AppSettings["TwitterConsumerKey"],
                    ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                    builder.ToString()).Token;

                return Redirect(OAuthUtility.BuildAuthorizationUri(token, true).ToString());
            }

            // now that we have a temp token
            // make the call to get the access token
            var tokens = OAuthUtility.GetAccessToken(
                ConfigurationManager.AppSettings["TwitterConsumerKey"],
                ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                oauth_token,
                oauth_verifier);

            FormsAuthentication.SetAuthCookie(tokens.ScreenName, false);

            if (string.IsNullOrEmpty(ReturnUrl))
                return Redirect("/");
            else
                return Redirect(ReturnUrl);
        }
    }
}
