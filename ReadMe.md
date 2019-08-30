# JasonMorsley.Dev.Users

##### HATEAOS:
Hypermedia As The Engine Of Application State

HTML Represents links with the anchor element

 <a href="uri",rel="type",type="media type">
* href: contains the uri
* rel: describes how the link relates to the resource
* type describes the media type

##### Supporting HATEAOS:

      "links": [
        {
          "href": http://host/api/users/guid,
          "rel": "GetUsers",
          "method": "GET"
        }

* "method" defines the method to use

* "rel" identifies the type of action

* "href" contains the URI to be invoked to execute this action


##### Testing:

To run our functionality tests of our API we use newman in CLI and two commands:

```cd C:\GitHub\JasonMorsley.Dev.Users\Generated Files```

To get into the directory where our test collection and enviroment variable collection are held.

```newman run "User API.postman_test_collection.json" --environment "Users API DEV.postman_test_environment.json" --insecure```

First the newman run then we pass in the collection of tests generated in Postman.

Then the collection of environment variables which currently consist of
out localhost and port for the API in development mode, this would be changed to production
when the API is in use.

And lastly --insecure is simply because the SSL certificate is invalid or nonexistent.
