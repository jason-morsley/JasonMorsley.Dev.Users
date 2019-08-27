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