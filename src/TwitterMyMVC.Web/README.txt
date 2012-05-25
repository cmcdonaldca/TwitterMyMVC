Twitter My MVC - By Colin McDonald <cmcdonaldca@gmail.com>

1. Installation

This project uses Code First Entity Framework, so all you have to do is create a datebase. To avoid 
editting the web.config, create a database called "TwitterMyMVC" on your localhost SQL server.

You will have to go register an App at Twitter to get a ConsumerKey and ConsumerSeceret.  Once you
have them, put them into the web.config (just search secret).

Oh... make sure that when you execute the app you use 127.0.0.1.  Twitter does not like localhost.


2. Noteworthy

I found a library called Twitteriser and that's what I ended up using to access the Twitter API.  It
turned out to be a very nice and easy-to-use library.

I also use the Twitter Bootstrap framework for HTML/CSS.  Sure makes life easy!

I ended up running out of time, so the "look" of the search results is not all that great.  However, 
semantically it's correct.

