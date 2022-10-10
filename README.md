# Posterr API

## Description
Strider Web Back-end Assessment API. Project responsible to manage the Posterr users posts, profile and followers.

## Instructions to Run the Project
* After extract the project file, create a SQL Serrver database. The files are available into the folder src\Infra.Data\Scripts. The sequence of execution should be:
   1. CreateDatabase.sql
   2. CreateTableUsers.sql
   3. CreateTablePosts.sql
   4. CreateTableFollowers.sql

* After creating the database, set the connection string into the variable "ConnectionStrings:PosterrDatabase" available into the file src\Api\appsettings.json.
* The API is developed using .Net 5, in order to execute the project, use MS Visual Studio and open the file Posterr.sln.

## Planning

1. Questions

   - The "Posts and Replies" profile would return how many records at first?
   - Would be a button to load older "Replies to post"?
   - Should I return a field that contains the information whether the user is following the poster or not?
   - Probably the main informations that I would save are the message, the date and time, the original user and the referred user. Do you want me to add any more information?
   - The user in his profile would see only his replies or the followers replies as well?

2. Solution

   - I would use the already existing Posts table. I would just add a new type called "reply_to_post".
     - Id
     - Message 
     - OriginalPostId (Post's table Id)
     - Type (add new type "reply_to_post")
     - UserId (User's table Id)
     - CreatedAt (date and time of the creation)
   - In the API layer, I would use the same endpoint available POST /api/user/{username}/posts and I would add validations as:
     - If the type is "reply_to_post" the originalPostId field can't be empty.
     - The Original Post User cannot be same as poster username
   - The GET /api/user/{username}/posts would return all replies and the type "reply_to_post" to show that the post is related to a reply, and not to a regular post.


## Critique

1. Improvements
   - If I had more time, I would add a lot of logs in every step of the API. I would save these logs in Azure Application Insights, the tool that I'm mostly used to.
   - The architecture are okay, I would not change the architecture.
   - I would add a migration mechanism to push the database changes automatically to the database server.

2. Scalling
   - The queries to return posts might start being slow after many records inserted. At first I would add some indexes into the database to return the data.The next step would be use a distributed cache to store the last X Posts records. Probably the user would not see the old posts regularly.
   - To scale the product I would ensure that the Hoster of the API is configured to automatically scale. Also I would run this application in at least 3 containers and increase as it needed.